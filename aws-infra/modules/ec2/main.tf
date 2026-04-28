data "aws_ami" "amazon_linux" {
  most_recent = true
  owners      = ["amazon"]

  filter {
    name   = "name"
    values = ["amzn2-ami-hvm-*-x86_64-gp2"]
  }
}

# Use existing key pair
data "aws_key_pair" "this" {
  key_name = "test-mannan"
}

resource "aws_security_group" "this" {
  name        = "${var.name_prefix}-ec2-sg"
  description = "Security group for EC2 instance"
  vpc_id      = var.vpc_id

  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = [var.allowed_ssh_cidr]
  }

  ingress {
    from_port   = var.app_port
    to_port     = var.app_port
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "${var.name_prefix}-ec2-sg"
  }
}

resource "aws_instance" "this" {
  ami           = data.aws_ami.amazon_linux.id
  instance_type = var.instance_type
  subnet_id     = var.subnet_id
  key_name      = data.aws_key_pair.this.key_name
  vpc_security_group_ids = [aws_security_group.this.id]

  # Associate with public IP (or use existing subnet's setting)
  associate_public_ip_address = true

  user_data = templatefile("${path.module}/templates/user_data.tftpl", {
    app_type = var.app_type
    app_port = var.app_port
  })

  tags = {
    Name = "${var.name_prefix}-ec2-instance"
  }
}

# Allocate Elastic IP to ensure public IP even if subnet doesn't assign one
resource "aws_eip" "this" {
  instance = aws_instance.this.id
  domain   = "vpc"

  tags = {
    Name = "${var.name_prefix}-eip"
  }
}