variable "name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "vpc_id" {
  description = "ID of the VPC"
  type        = string
}

variable "subnet_id" {
  description = "ID of the public subnet"
  type        = string
}

variable "instance_type" {
  description = "EC2 instance type"
  type        = string
}

variable "allowed_ssh_cidr" {
  description = "CIDR block allowed for SSH access"
  type        = string
}

variable "key_pair_name" {
  description = "Name of existing key pair to use"
  type        = string
  default     = "test-mannan"
}

variable "app_port" {
  description = "Port for the application to run on"
  type        = number
}

variable "app_type" {
  description = "Type of application to deploy (nginx, nodejs, python, static)"
  type        = string
}