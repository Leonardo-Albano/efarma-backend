from flask import Flask, jsonify, request
import random
import string
import os

app = Flask(__name__)

# Define o tamanho padrão dos códigos e a quantidade mínima necessária
DEFAULT_CODE_LENGTH = 10
REQUIRED_CODE_COUNT = 13
CODES_FILE_PATH = 'tag_codes.txt'

def generate_random_string(length=DEFAULT_CODE_LENGTH):
    return ''.join(random.choices(string.ascii_letters + string.digits, k=length))

def get_or_generate_tag_codes():
    # Verifica se o arquivo existe, caso contrário, cria um novo vazio
    if not os.path.exists(CODES_FILE_PATH):
        with open(CODES_FILE_PATH, 'w') as f:
            pass

    # Lê os códigos do arquivo
    with open(CODES_FILE_PATH, 'r') as f:
        tag_codes = [line.strip() for line in f.readlines()]

    # Gera novos códigos se o total for inferior ao necessário
    if len(tag_codes) < REQUIRED_CODE_COUNT:
        additional_codes = [generate_random_string() for _ in range(REQUIRED_CODE_COUNT - len(tag_codes))]
        tag_codes.extend(additional_codes)
        
        # Grava os novos códigos no arquivo
        with open(CODES_FILE_PATH, 'a') as f:
            for code in additional_codes:
                f.write(code + '\n')
    
    return tag_codes[:REQUIRED_CODE_COUNT]

# Define o endpoint usando query string para o código
@app.route('/TagCodes', methods=['GET'])
def get_tag_codes():
    # Obtém o parâmetro 'code' da query string
    code = request.args.get('code')
    
    # Exibe a string recebida no console
    print(f"Received string: {code}")
    
    # Gera ou lê os códigos
    tag_codes = get_or_generate_tag_codes()
    return jsonify(tag_codes)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
