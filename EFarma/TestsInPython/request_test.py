import requests
import os
import time

def get_tag_count():
    url = "http://157.230.224.194:5002/get_tags"
    try:
        response = requests.get(url)
        response.raise_for_status()  # Check for HTTP errors
        data = response.json()
        
        # Count the number of items in the "tags" list
        tag_count = len(data.get("tags", []))
        
        print(f"Number of tags: {tag_count}")
    except requests.exceptions.RequestException as e:
        print(f"An error occurred: {e}")

# Run the function in a loop with a 1-second delay and clear every 10 iterations
iteration = 0
while True:
    if iteration % 10 == 0:
        os.system('cls' if os.name == 'nt' else 'clear')  # Clear console every 10 iterations
    get_tag_count()
    iteration += 1
    time.sleep(2)
