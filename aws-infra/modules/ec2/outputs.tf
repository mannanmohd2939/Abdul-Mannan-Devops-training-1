output "ec2_instance_id" {
  description = "ID of the EC2 instance"
  value       = aws_instance.this.id
}

output "ec2_public_ip" {
  description = "Public IP address of the EC2 instance (Elastic IP)"
  value       = aws_eip.this.public_ip
}

output "secret_name" {
  description = "Name of the Secrets Manager secret containing the SSH private key"
  value       = aws_secretsmanager_secret.ssh_key.name
}

output "ssh_command" {
  description = "Command to SSH into the instance"
  value       = "aws secretsmanager get-secret-value --secret-id ${aws_secretsmanager_secret.ssh_key.name} --query SecretString --output text > key.pem && chmod 400 key.pem && ssh -i key.pem ec2-user@${aws_eip.this.public_ip}"
}