FROM python:3.9-slim

# Set the working directory
WORKDIR /app

# Copy application files
COPY app.py /app/
COPY quotes.json /app/

# Install dependencies
RUN pip install flask

# Expose the application port
EXPOSE 5000

# Run the application
CMD ["python3", "app.py"]