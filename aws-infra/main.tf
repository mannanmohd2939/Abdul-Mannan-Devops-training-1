# Add Terraform workspace configuration
terraform {
  backend "s3" {
    bucket         = "my-terraform-state-bucket"
    key            = "path/to/terraform.tfstate"
    region         = "us-east-1"
    dynamodb_table = "terraform-lock"
  }
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

module "vpc" {
  source = "./modules/vpc"

  name_prefix       = var.name_prefix
  vpc_cidr          = var.vpc_cidr
  existing_vpc_id   = var.existing_vpc_id
}

module "subnet" {
  source = "./modules/subnet"

  name_prefix      = var.name_prefix
  vpc_id           = module.vpc.vpc_id
  subnet_cidr      = var.subnet_cidr
  availability_zone = var.availability_zone
}

module "igw" {
  source = "./modules/igw"

  name_prefix = var.name_prefix
  vpc_id      = module.vpc.vpc_id
  subnet_id   = module.subnet.subnet_id
}

module "ec2" {
  source = "./modules/ec2"

  name_prefix       = var.name_prefix
  vpc_id            = module.vpc.vpc_id
  subnet_id         = module.subnet.subnet_id
  instance_type     = var.instance_type
  allowed_ssh_cidr  = var.allowed_ssh_cidr
  key_pair_name     = var.key_pair_name
  app_port          = var.app_port
  app_type          = var.app_type
}