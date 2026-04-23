terraform {
  backend "s3" {
    bucket = "cmdstk-terraform-state-batch-1"
    key    = "mannan/tfstate/terraform.tfstate"
    region = "us-east-1"
    encrypt = true
  }
}