resource "aws_s3_bucket" "app_bucket" {
  bucket = "smartnotes-attachments-mannan"
  tags = {
    Name        = "smartnotes-attachments-mannan"
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
  name = "smartnotes-events-mannan"
}

resource "aws_sqs_queue" "app_queue" {
  name                       = "smartnotes-processing-mannan"
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
