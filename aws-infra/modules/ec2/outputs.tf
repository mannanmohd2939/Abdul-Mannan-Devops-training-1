output "ec2_instance_id" {
  description = "ID of the EC2 instance"
  value       = aws_instance.this.id
}

output "ec2_public_ip" {
  description = "Public IP address of the EC2 instance (Elastic IP)"
  value       = aws_eip.this.public_ip
}