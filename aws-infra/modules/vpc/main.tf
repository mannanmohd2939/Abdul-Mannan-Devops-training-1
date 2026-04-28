# Look up existing VPC by name
data "aws_vpc" "by_name" {
  filter {
    name   = "tag:Name"
    values = ["cmdstk-training-batch-1-vpc"]
  }
}

data "aws_vpc" "by_id" {
  count = var.existing_vpc_id != "" ? 1 : 0

  id = var.existing_vpc_id
}

resource "aws_vpc" "this" {
  count = var.existing_vpc_id != "" ? 0 : (data.aws_vpc.by_name.id != "" ? 0 : 1)

  cidr_block           = var.vpc_cidr
  enable_dns_hostnames = true
  enable_dns_support   = true

  tags = {
    Name = "${var.name_prefix}-vpc"
  }
}

locals {
  vpc_id = var.existing_vpc_id != "" ? var.existing_vpc_id : (
    data.aws_vpc.by_name.id != "" ? data.aws_vpc.by_name.id : aws_vpc.this[0].id
  )
}