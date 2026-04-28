data "aws_internet_gateway" "this" {
  filter {
    name   = "attachment.vpc-id"
    values = [var.vpc_id]
  }
}

resource "aws_internet_gateway" "this" {
  count = length(data.aws_internet_gateway.this.ids) > 0 ? 0 : 1

  vpc_id = var.vpc_id

  tags = {
    Name = "${var.name_prefix}-igw"
  }
}

locals {
  igw_id = length(data.aws_internet_gateway.this.ids) > 0 ? data.aws_internet_gateway.this.ids[0] : aws_internet_gateway.this[0].id
}

resource "aws_route_table" "public" {
  vpc_id = var.vpc_id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = local.igw_id
  }

  tags = {
    Name = "${var.name_prefix}-rt-public"
  }
}

resource "aws_route_table_association" "public" {
  subnet_id      = var.subnet_id
  route_table_id = aws_route_table.public.id
}