data "aws_subnet" "existing" {
  filter {
    name   = "tag:Name"
    values = ["cmdstk-training-batch-1-subnet-public"]
  }
}

resource "aws_subnet" "public" {
  count = data.aws_subnet.existing.id != "" ? 0 : 1

  vpc_id                  = var.vpc_id
  cidr_block              = var.subnet_cidr
  availability_zone       = var.availability_zone
  map_public_ip_on_launch = true

  tags = {
    Name = "${var.name_prefix}-subnet-public"
  }
}

locals {
  subnet_id = data.aws_subnet.existing.id != "" ? data.aws_subnet.existing.id : aws_subnet.public[0].id
}