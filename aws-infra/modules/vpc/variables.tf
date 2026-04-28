variable "name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "vpc_cidr" {
  description = "CIDR block for the VPC"
  type        = string
}

variable "existing_vpc_id" {
  description = "ID of existing VPC to use (optional)"
  type        = string
  default     = ""
}