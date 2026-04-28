output "igw_id" {
  description = "ID of the Internet Gateway"
  value       = local.igw_id
}

output "route_table_id" {
  description = "ID of the public route table"
  value       = aws_route_table.public.id
}