import streamlit as st
import random
import string
import os
from flask import Flask, jsonify, request
from threading import Thread

# Set up Flask app
app = Flask(__name__)

# Define default values
DEFAULT_CODE_LENGTH = 10
CODES_FILE_PATH = 'tag_codes.txt'
REQUIRED_CODE_COUNT = 13  # Default required code count (can be updated in Streamlit)

# Function to generate a random code string
def generate_random_string(length=DEFAULT_CODE_LENGTH):
    return ''.join(random.choices(string.ascii_letters + string.digits, k=length))

# Function to get or generate the necessary tag codes
def get_or_generate_tag_codes(required_code_count):
    # Ensure the file exists
    if not os.path.exists(CODES_FILE_PATH):
        with open(CODES_FILE_PATH, 'w') as f:
            pass

    # Read the existing codes from the file
    with open(CODES_FILE_PATH, 'r') as f:
        tag_codes = [line.strip() for line in f.readlines()]

    # Generate additional codes if the total is less than required
    if len(tag_codes) < required_code_count:
        additional_codes = [generate_random_string() for _ in range(required_code_count - len(tag_codes))]
        tag_codes.extend(additional_codes)
        
        # Save new codes to the file
        with open(CODES_FILE_PATH, 'a') as f:
            for code in additional_codes:
                f.write(code + '\n')
    
    return tag_codes[:required_code_count]

# Flask endpoint for external requests
@app.route('/TagCodes', methods=['GET'])
def get_tag_codes():
    code = request.args.get('code')
    print(f"Received string: {code}")
    
    # Use the global REQUIRED_CODE_COUNT from Streamlit input
    tag_codes = get_or_generate_tag_codes(REQUIRED_CODE_COUNT)
    return jsonify(tag_codes)

# Function to run Flask in a separate thread
def run_flask():
    app.run(host='0.0.0.0', port=5000)

# Start Flask in a separate thread
flask_thread = Thread(target=run_flask)
flask_thread.daemon = True
flask_thread.start()

# Streamlit app interface
st.title("Tag Code Generator and Editor")

# Input for configuring the required code count
REQUIRED_CODE_COUNT = st.number_input("Set the number of required tag codes", min_value=1, value=REQUIRED_CODE_COUNT, step=1)

# Button to generate or read codes
if st.button("Generate/View Tag Codes"):
    # Get or generate the codes using the updated REQUIRED_CODE_COUNT
    tag_codes = get_or_generate_tag_codes(REQUIRED_CODE_COUNT)
    st.write("Generated Tag Codes:")
    st.write(tag_codes)

# Text area for editing tag_codes.txt
st.write("Edit tag_codes.txt")
if os.path.exists(CODES_FILE_PATH):
    with open(CODES_FILE_PATH, 'r') as f:
        file_content = f.read()
else:
    file_content = ""

# Text area for displaying and editing file content
edited_content = st.text_area("File Content", file_content, height=200)

# Save button to update the file content
if st.button("Save Changes"):
    with open(CODES_FILE_PATH, 'w') as f:
        f.write(edited_content)
    st.success("Changes saved to tag_codes.txt")
