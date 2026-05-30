# Abdul-Mannan-Devops-training-1

This repository contains:

- `SmartNotes/` — the local application codebase.
- `aws-infra/` — Terraform configuration to provision AWS resources.

The Terraform stack now deploys:

- S3 bucket
- SNS topic
- SQS queue
- SNS subscription to SQS

Use the `aws-infra/` folder to run `terraform init`, `terraform plan`, and `terraform apply`.
