from flask import Flask, jsonify, request
import random
import string

app = Flask(__name__)

def generate_random_string(length=10):
    return ''.join(random.choices(string.ascii_letters + string.digits, k=length))

@app.route('/TagCodes', methods=['GET'])
def get_tag_codes():
    unique_id = request.args.get('uniqueId')
    print(unique_id)
    tag_codes = [generate_random_string() for _ in range(5)]
    return jsonify(tag_codes)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
