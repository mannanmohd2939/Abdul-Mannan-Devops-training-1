# Look up existing IGW by name
data "aws_internet_gateway" "this" {
  filter {
    name   = "tag:Name"
    values = ["cmdstk-training-batch-1-igw"]
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

# Look up existing route table by name
data "aws_route_table" "existing" {
  filter {
    name   = "tag:Name"
    values = ["cmdstk-training-batch-1-rt-public"]
  }
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

# Note: We don't create route table association since subnet is already associated with existing RT
# The existing route table should already have IGW route