output "ec2_instance_id" {
  description = "ID of the EC2 instance"
  value       = aws_instance.this.id
}

output "ec2_public_ip" {
  description = "Public IP address of the EC2 instance (Elastic IP)"
  value       = aws_eip.this.public_ip
}

output "key_pair_name" {
  description = "Name of the key pair used"
  value       = var.key_pair_name
}

output "ssh_command" {
  description = "Command to SSH into the instance"
  value       = "ssh -i <path-to-key> ec2-user@${aws_eip.this.public_ip}"
}