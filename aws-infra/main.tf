resource "random_id" "resource_suffix" {
  byte_length = 4
}

resource "aws_s3_bucket" "app_bucket" {
  bucket = "${var.name_prefix}-${random_id.resource_suffix.hex}"
  tags = {
    Name        = "${var.name_prefix}-bucket"
    Environment = "dev"
  }
}

resource "aws_s3_bucket_public_access_block" "app_bucket_block" {
  bucket                  = aws_s3_bucket.app_bucket.id
  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

resource "aws_sns_topic" "app_topic" {
  name = "${var.name_prefix}-topic"
}

resource "aws_sqs_queue" "app_queue" {
  name                       = "${var.name_prefix}-queue"
  visibility_timeout_seconds = 30
  message_retention_seconds  = 86400
}

resource "aws_sns_topic_subscription" "queue_subscription" {
  topic_arn            = aws_sns_topic.app_topic.arn
  protocol             = "sqs"
  endpoint             = aws_sqs_queue.app_queue.arn
  raw_message_delivery = true
}

resource "aws_sqs_queue_policy" "allow_sns" {
  queue_url = aws_sqs_queue.app_queue.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Service = "sns.amazonaws.com"
        }
        Action = "sqs:SendMessage"
        Resource = aws_sqs_queue.app_queue.arn
        Condition = {
          ArnEquals = {
            "aws:SourceArn" = aws_sns_topic.app_topic.arn
          }
        }
      }
    ]
  })
}
