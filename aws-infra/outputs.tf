output "s3_bucket_name" {
  description = "Name of the application S3 bucket"
  value       = aws_s3_bucket.app_bucket.bucket
}

output "sns_topic_arn" {
  description = "ARN of the SNS topic"
  value       = aws_sns_topic.app_topic.arn
}

output "sqs_queue_url" {
  description = "URL of the SQS queue"
  value       = aws_sqs_queue.app_queue.id
}

output "sqs_queue_arn" {
  description = "ARN of the SQS queue"
  value       = aws_sqs_queue.app_queue.arn
}

output "sns_to_sqs_subscription_arn" {
  description = "ARN of the SNS subscription to SQS"
  value       = aws_sns_topic_subscription.queue_subscription.arn
}