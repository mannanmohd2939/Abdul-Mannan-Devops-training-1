data "aws_internet_gateway" "this" {
  filter {
    name   = "attachment.vpc-id"
    values = [var.vpc_id]
  }
}

resource "aws_internet_gateway" "this" {
  count = data.aws_internet_gateway.this.id != "" ? 0 : 1

  vpc_id = var.vpc_id

  tags = {
    Name = "${var.name_prefix}-igw"
  }
}

locals {
  igw_id = data.aws_internet_gateway.this.id != "" ? data.aws_internet_gateway.this.id : aws_internet_gateway.this[0].id
}

# Look up existing route table for the subnet
data "aws_route_table" "existing" {
  subnet_id = var.subnet_id
}

# Ensure existing route table has IGW route
resource "aws_route" "igw_route" {
  count = data.aws_route_table.existing.id != "" ? 1 : 0

  route_table_id            = data.aws_route_table.existing.id
  destination_cidr_block    = "0.0.0.0/0"
  gateway_id                = local.igw_id
}

resource "aws_route_table" "public" {
  count = data.aws_route_table.existing.id != "" ? 0 : 1

  vpc_id = var.vpc_id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = local.igw_id
  }

  tags = {
    Name = "${var.name_prefix}-rt-public"
  }
}

locals {
  route_table_id = data.aws_route_table.existing.id != "" ? data.aws_route_table.existing.id : aws_route_table.public[0].id
}

resource "aws_route_table_association" "public" {
  count = data.aws_route_table.existing.id != "" ? 0 : 1

  subnet_id      = var.subnet_id
  route_table_id = aws_route_table.public[0].id
}