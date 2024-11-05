import streamlit as st
import random
import string
import os
import json
from flask import Flask, jsonify, request
from threading import Thread

# Configurações de caminhos
CODES_FILE_PATH = 'tag_codes.txt'
CONFIG_FILE_PATH = 'config.json'

# Inicialização do Flask
app = Flask(__name__)

# Função para gerar uma string de código aleatória
def generate_random_string(length=10):
    return ''.join(random.choices(string.ascii_letters + string.digits, k=length))

# Função para carregar ou criar o arquivo de configuração
def load_or_initialize_config():
    if os.path.exists(CONFIG_FILE_PATH):
        with open(CONFIG_FILE_PATH, 'r') as f:
            config = json.load(f)
    else:
        config = {"required_code_count": 5}
        with open(CONFIG_FILE_PATH, 'w') as f:
            json.dump(config, f)
    return config

# Função para salvar a configuração
def save_config(config):
    with open(CONFIG_FILE_PATH, 'w') as f:
        json.dump(config, f)

# Função para carregar, gerar e ajustar os códigos conforme necessário
def load_or_generate_codes(required_code_count):
    if not os.path.exists(CODES_FILE_PATH):
        with open(CODES_FILE_PATH, 'w') as f:
            pass

    with open(CODES_FILE_PATH, 'r') as f:
        tag_codes = [line.strip() for line in f.readlines() if line.strip()]

    # Ajusta a quantidade de códigos conforme o valor desejado
    if len(tag_codes) < required_code_count:
        tag_codes.extend(generate_random_string() for _ in range(required_code_count - len(tag_codes)))
    elif len(tag_codes) > required_code_count:
        tag_codes = tag_codes[:required_code_count]

    with open(CODES_FILE_PATH, 'w') as f:
        f.write('\n'.join(tag_codes) + '\n')
    
    return tag_codes

# Rota Flask para obter os códigos via API
@app.route('/TagCodes', methods=['GET'])
def get_tag_codes():
    config = load_or_initialize_config()
    tag_codes = load_or_generate_codes(config["required_code_count"])
    return jsonify(tag_codes)

# Função para iniciar o servidor Flask em um thread separado
def run_flask():
    app.run(host='0.0.0.0', port=5000)

# Inicializar o Flask em um thread separado
flask_thread = Thread(target=run_flask)
flask_thread.daemon = True
flask_thread.start()

# Interface Streamlit
st.title("Simulação de Armário RFID")

# Carregar ou inicializar a configuração
config = load_or_initialize_config()

# Controle para definir a quantidade de códigos
required_code_count = st.number_input("Número de códigos RFID necessários", min_value=1, value=config["required_code_count"], step=1)

# Salvar alterações na quantidade de códigos
if required_code_count != config["required_code_count"]:
    config["required_code_count"] = required_code_count
    save_config(config)

# Botão para exibir os códigos gerados
if st.button("Gerar/Visualizar Códigos RFID"):
    tag_codes = load_or_generate_codes(config["required_code_count"])
    st.write("Códigos RFID Gerados:")
    st.write(tag_codes)

# Área de texto para editar diretamente o conteúdo do arquivo tag_codes.txt
st.write("Editar tag_codes.txt")
if os.path.exists(CODES_FILE_PATH):
    with open(CODES_FILE_PATH, 'r') as f:
        file_content = f.read()
else:
    file_content = ""

edited_content = st.text_area("Conteúdo do Arquivo", file_content, height=200)

# Botão para salvar as alterações feitas manualmente no arquivo
if st.button("Salvar Alterações"):
    with open(CODES_FILE_PATH, 'w') as f:
        f.write(edited_content)
    st.success("Alterações salvas em tag_codes.txt")
