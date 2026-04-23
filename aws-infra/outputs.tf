output "vpc_id" {
  description = "ID of the VPC"
  value       = module.vpc.vpc_id
}

output "subnet_id" {
  description = "ID of the public subnet"
  value       = module.subnet.subnet_id
}

output "ec2_instance_id" {
  description = "ID of the EC2 instance"
  value       = module.ec2.ec2_instance_id
}

output "ec2_public_ip" {
  description = "Public IP address of the EC2 instance"
  value       = module.ec2.ec2_public_ip
}

output "secret_name" {
  description = "Name of the Secrets Manager secret containing the SSH private key"
  value       = module.ec2.secret_name
}

output "ssh_command" {
  description = "Command to SSH into the instance"
  value       = module.ec2.ssh_command
}