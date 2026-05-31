# Abdul-Mannan-Devops-training-1

This repository contains:

- `SmartNotes/` — the local application codebase.
- `aws-infra/` — Terraform configuration to provision AWS resources.

The Terraform stack now deploys:

- S3 bucket `smartnotes-attachments-mannan`
- SNS topic `smartnotes-events-mannan`
- SQS queue `smartnotes-processing-mannan`
- SNS subscription to SQS

Use the `aws-infra/` folder to run `terraform init`, `terraform plan`, and `terraform apply`.

The current AWS resources are configured to use exact names:

- S3 bucket: `smartnotes-attachments-abdulmateen`
- SNS topic: `smartnotes-events`
- SQS queue: `smartnotes-processing`

## GitHub Actions

This repository includes two workflows:

- `.github/workflows/terraform.yml` — validates, plans, and applies Terraform using the configured GitHub OIDC role.
- `.github/workflows/dotnet-ef.yml` — starts a pgvector-enabled PostgreSQL container and applies EF Core migrations for `SmartNotes.Api`.

## Local PostgreSQL

If your PostgreSQL is containerized, run it before starting the API.
A matching container image with `pgvector` support is recommended.

Example:

```bash
docker run --name smartnotes-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=smartnotes -p 5432:5432 -d pgvector/pgvector:latest
```

Then configure `DefaultConnection` in `SmartNotes.Api/appsettings.json` or environment variables.

## EF Core startup seed

The API is now configured to automatically apply migrations and seed a sample note with one attachment and one tag on startup.

After the DB is running, launch the app and it will create the schema and test row automatically.

## Test SNS/SQS delivery

After AWS credentials are configured, test the SNS->SQS path with:

```powershell
aws sns publish --topic-arn <topic-arn> --message "Test message"
aws sqs receive-message --queue-url <queue-url>
```

// "SqsQueueUrl": "https://sqs.us-east-1.amazonaws.com/679336465006/smartnotes-processing-mannan",
// "SnsTopicArn": "arn:aws:sns:us-east-1:679336465006:smartnotes-events-mannan",
using Amazon.SQS;
using Amazon.SimpleNotificationService;
using Microsoft.EntityFrameworkCore;
using SmartNotes.Worker;
using SmartNotes.Worker.Data;
using SmartNotes.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<SmartNotesDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();

builder.Services.AddSingleton<SqsListenerService>();
builder.Services.AddSingleton<SnsService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();