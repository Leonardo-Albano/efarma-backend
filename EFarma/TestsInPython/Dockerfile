# Use the official Python image
FROM python:3.9

# Set the working directory
WORKDIR /app

# Copy the current directory contents into the container at /app
COPY . /app

# Install required packages
RUN pip install streamlit Flask

# Expose the ports for Flask and Streamlit
EXPOSE 5000
EXPOSE 8501

# Run both Flask and Streamlit servers
CMD ["sh", "-c", "python -u simulate_tags.py & streamlit run simulate_tags.py"]
