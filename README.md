# SmartNotes — Full-Stack Semantic Note-Taking Platform

A production-grade note-taking application built to demonstrate end-to-end DevOps practices: containerization, Kubernetes orchestration, AWS cloud services, and a fully automated CI/CD pipeline.

---

## Architecture
┌─────────────────────────────────────────────────────────────────┐
│                        Local Machine                            │
│                                                                 │
│   React Frontend (localhost:5173)                               │
│   PostgreSQL 16 + pgvector (localhost:5432)                     │
└──────────────────────┬──────────────────────────────────────────┘
│ HTTP
┌──────────────────────▼──────────────────────────────────────────┐
│                   Minikube Cluster                              │
│                                                                 │
│   ┌─────────────────────┐    ┌──────────────────────────┐       │
│   │  SmartNotes.Api     │    │  SmartNotes.Worker       │       │
│   │  (2 replicas)       │    │  (1 replica)             │       │
│   │  ASP.NET Core 8     │    │  IHostedService          │       │
│   │  Port 8080          │    │  SQS Consumer            │       │
│   └──────────┬──────────┘    └──────────────────────────┘       │
│              │                                                  │
│   Ingress (nginx) → smartnotes-api-svc → API pods              │
└──────────────┬──────────────────────────────────────────────────┘
│
┌──────────────▼──────────────────────────────────────────────────┐
│                      AWS Cloud                                  │
│                                                                 │
│   S3: smartnotes-attachments-mannan  (file storage)            │
│   SQS: smartnotes-processing-mannan  (job queue)               │
│   SNS: smartnotes-events-mannan      (event broadcast)         │
│   IAM: github-oidc role              (CI/CD access)            │
└─────────────────────────────────────────────────────────────────┘
Data Flow:
React → API (Minikube) → PostgreSQL (local) / S3 (AWS)
→ SQS (AWS) → Worker (Minikube) → pgvector → SNS (AWS)

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend API | .NET 8, ASP.NET Core, Entity Framework Core 9 |
| Background Worker | .NET 8 IHostedService |
| Database | PostgreSQL 16 + pgvector extension |
| Frontend | React 19, Vite, Axios |
| File Storage | AWS S3 |
| Message Queue | AWS SQS (long polling) |
| Notifications | AWS SNS |
| Containers | Docker (multi-stage builds) |
| Orchestration | Kubernetes (Minikube) |
| CI/CD | GitHub Actions + self-hosted runner |
| Image Registry | GitHub Container Registry (GHCR) |
| Infrastructure | Terraform (S3, SQS, SNS) |

---

## Prerequisites

```bash
# Verify all tools are installed
docker --version        # 24+
minikube version        # v1.32+
kubectl version --client # v1.29+
aws --version           # aws-cli/2.x
dotnet --version        # 8.0.x
node --version          # v20.x
git --version
```

---

## Local Development Setup

### 1. Clone the repository

```bash
git clone https://github.com/mannanmohd2939/Abdul-Mannan-Devops-training-1.git
cd Abdul-Mannan-Devops-training-1
```

### 2. Start PostgreSQL with pgvector

```bash
docker run --name smartnotes-postgres \
  -e POSTGRES_USER=smartnotes_user \
  -e POSTGRES_PASSWORD=password \
  -e POSTGRES_DB=smartnotes_db \
  -p 5432:5432 \
  -d pgvector/pgvector:latest
```

### 3. Run the API

```bash
cd SmartNotes.Api
dotnet run
# API starts on http://localhost:5092
# Swagger UI: http://localhost:5092/swagger
```

The API automatically runs EF Core migrations and seeds a sample note on first startup.

### 4. Run the Worker

```bash
cd SmartNotes.Worker
dotnet run
# Worker starts polling SQS every 20s
```

### 5. Run the React frontend

```bash
cd smartnotes-web
echo "VITE_API_URL=http://localhost:5092/api" > .env.local
npm install
npm run dev
# Frontend: http://localhost:5173
```

### 6. Run unit tests

```bash
cd SmartNotes.Tests
dotnet test
# 6 tests, all passing
```

---

## AWS Setup

### Provision infrastructure with Terraform

```bash
cd aws-infra
terraform init
terraform plan
terraform apply
```

This creates:
- S3 bucket: `smartnotes-attachments-mannan`
- SQS queue: `smartnotes-processing-mannan`
- SNS topic: `smartnotes-events-mannan`
- SNS → SQS subscription

### Configure AWS credentials locally

```bash
aws configure
# Enter your IAM user access key and secret
# Region: us-east-1
```

### Test AWS connectivity

```bash
aws sts get-caller-identity
aws sqs receive-message --queue-url https://sqs.us-east-1.amazonaws.com/679336465006/smartnotes-processing-mannan
```

---

## Minikube Deployment

### 1. Start Minikube

```bash
minikube start --cpus=4 --memory=4096 --driver=docker
minikube addons enable ingress
```

### 2. Create the Kubernetes secret (never committed to git)

```bash
kubectl apply -f k8s/namespace.yaml

kubectl create secret generic smartnotes-secrets \
  --namespace=smartnotes \
  --from-literal=ConnectionStrings__DefaultConnection="Host=host.minikube.internal;Port=5432;Database=smartnotes_db;Username=smartnotes_user;Password=password" \
  --from-literal=AWS_ACCESS_KEY_ID=your_access_key \
  --from-literal=AWS_SECRET_ACCESS_KEY=your_secret_key
```

### 3. Apply all manifests

```bash
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/worker-deployment.yaml
kubectl apply -f k8s/ingress.yaml
```

### 4. Verify pods are running

```bash
kubectl get pods -n smartnotes
# NAME                                 READY   STATUS    RESTARTS
# smartnotes-api-xxx                   1/1     Running   0
# smartnotes-api-xxx                   1/1     Running   0
# smartnotes-worker-xxx                1/1     Running   0

kubectl get ingress -n smartnotes
```

### 5. Expose via tunnel (Windows)

```bash
minikube tunnel
# Access API at http://127.0.0.1/api
# Access health at http://127.0.0.1/health
```

### 6. Watch logs

```bash
kubectl logs -f deployment/smartnotes-api -n smartnotes
kubectl logs -f deployment/smartnotes-worker -n smartnotes
```

---

## CI/CD Pipeline

The pipeline has three jobs defined in `.github/workflows/ci-cd.yml`:
Push to main
│
▼
┌─────────┐
│   CI    │  Runs on GitHub (ubuntu-latest)
│         │  • dotnet restore
│         │  • dotnet build
│         │  • dotnet test (6 tests)
└────┬────┘
│ on success
▼
┌─────────┐
│ Docker  │  Runs on GitHub (ubuntu-latest)
│         │  • Build SmartNotes.Api image
│         │  • Build SmartNotes.Worker image
│         │  • Push to ghcr.io/mannanmohd2939/
└────┬────┘
│ on success
▼
┌─────────┐
│ Deploy  │  Runs on self-hosted runner (your machine)
│         │  • kubectl apply all manifests
│         │  • Wait for rollout
│         │  • Smoke test /health endpoint
└─────────┘

### Setting up the self-hosted runner

1. Go to: `https://github.com/mannanmohd2939/Abdul-Mannan-Devops-training-1/settings/actions/runners/new`
2. Select **Windows**
3. Download and configure:

```powershell
cd actions-runner
.\config.cmd --url https://github.com/mannanmohd2939/Abdul-Mannan-Devops-training-1 --token YOUR_TOKEN
.\run.cmd
```

Keep this running during deployments. The runner must be active for the deploy job to execute.

### Triggering the pipeline

```bash
# Any push to main triggers CI → Docker → Deploy
git add -A
git commit -m "your change"
git push origin main
```

### Blocking a deploy with a failing test

```bash
# Add a failing test to verify pipeline protection
# Example: change Assert.Equal("Test Note", ...) to Assert.Equal("Wrong", ...)
# Push → CI fails → Docker and Deploy jobs are skipped
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Health check (used by Kubernetes probes) |
| GET | `/api/notes` | List all notes with tags |
| GET | `/api/notes/{id}` | Get a single note |
| POST | `/api/notes` | Create a note (publishes to SQS) |
| PUT | `/api/notes/{id}` | Update a note (publishes to SQS) |
| DELETE | `/api/notes/{id}` | Delete a note |
| POST | `/api/upload` | Upload file attachment to S3 |
| GET | `/swagger` | Swagger UI (development) |

### Example: Create a note

```bash
curl -X POST http://localhost:5092/api/notes \
  -H "Content-Type: application/json" \
  -d '{"title":"My Note","content":"Note content","tags":["devops","aws"]}'
```

---

## Async Processing Flow

1. Note is created/updated via the API
2. API publishes a message to SQS: `{ NoteId, Action }`
3. Worker polls SQS every 20 seconds (long polling)
4. Worker picks up the message, generates an embedding (placeholder: `float[1536]`)
5. Worker updates the `Embedding` column in PostgreSQL via pgvector
6. Worker publishes completion event to SNS
7. Message is deleted from SQS

---

## Project Structure
Abdul-Mannan-Devops-training-1/
├── .github/workflows/
│   ├── ci-cd.yml          # Main CI/CD pipeline
│   └── terraform.yml      # Terraform validate/plan/apply
├── SmartNotes.Api/        # ASP.NET Core Web API
│   ├── Controllers/       # REST endpoints
│   ├── Data/              # EF Core DbContext + migrations
│   ├── DTOs/              # Request/response models
│   ├── Models/            # Entity models
│   ├── Services/          # S3Service, SqsService
│   └── Dockerfile
├── SmartNotes.Worker/     # Background SQS consumer
│   ├── Data/              # Worker DbContext
│   ├── NoteProcessorWorker.cs
│   └── Dockerfile
├── SmartNotes.Core/       # Shared entities
│   └── Entities/          # Note, Attachment, Tag
├── SmartNotes.Tests/      # xUnit test project (6 tests)
├── smartnotes-web/        # React 19 + Vite frontend
│   └── src/
│       ├── App.jsx        # Main app, note list
│       ├── api.js         # Axios API client
│       └── components/    # NoteCard, NoteEditor, SearchBar
├── k8s/                   # Kubernetes manifests
│   ├── namespace.yaml
│   ├── configmap.yaml
│   ├── api-deployment.yaml
│   ├── worker-deployment.yaml
│   └── ingress.yaml
├── aws-infra/             # Terraform for AWS resources
│   ├── main.tf            # S3, SQS, SNS, IAM
│   ├── variables.tf
│   └── outputs.tf
├── .dockerignore
├── SmartNotes.sln
└── README.md

---

## Known Limitations

- **Embeddings**: The worker currently stores a placeholder `float[1536]` array. Semantic search requires real embeddings (e.g. from OpenAI `text-embedding-3-small`). To enable: add `OPENAI_API_KEY` to Kubernetes secrets and implement the embedding call in `NoteProcessorWorker.cs`.
- **AWS credentials in Minikube**: Pods use static IAM keys stored as Kubernetes secrets. In production, use IAM Roles for Service Accounts (IRSA) with EKS instead.
- **Search**: The `/api/notes/search` endpoint requires embeddings to be populated. Notes created without real embeddings will not return meaningful vector similarity results.
- **Self-hosted runner**: The deploy job requires the GitHub Actions runner to be running on your local machine. If `run.cmd` is not active, the deploy job will queue indefinitely.

---

## Demo Checklist

- [ ] `kubectl get pods -n smartnotes` — all pods Running
- [ ] Create a note via React UI — appears in list
- [ ] Upload a file — returns URL (S3 with real credentials)
- [ ] SQS message processed — check Worker logs
- [ ] Semantic search — returns results
- [ ] Push code change → GitHub Actions runs → Minikube updated
- [ ] Introduce failing test → pipeline blocks deploy
