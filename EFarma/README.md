Comandos para o back:

dotnet ef database update 0 
dotnet ef database update
dotnet ef migrations add ""
dotnet ef migrations remove

Comandos para o Docker:

Subir:

docker login							# Login
docker build -t efarmatcc/efarmaback .	# Builda a imagem docker
docker push efarmatcc/efarmaback		# Sobe a imagem pro Docker Hub

Baixar:

docker login							# Login
docker pull efarmatcc/efarmaback		# Baixa a imagem do Docker Hub

Atualizar:

docker stop efarmaback					# Para a execução do container
docker rm efarmaback					# Remove o container anterior
docker pull efarmatcc/efarmaback		# Baixa a última versão da imagem (se atualizada remotamente)
docker run -d -p 5001:5001 --name efarmaback efarmatcc/efarmaback	# Inicia um novo container com a imagem atualizada

Run:

docker ps								# Lista os containers em execução
docker logs efarmaback					# Printa os logs de execução
docker exec -it efarmaback /bin/bash	# Entra no container em execução para debugging (opcional)

Comandos adicionais:
docker ps -a							# Lista todos os containers (em execução e parados)
docker system prune -a					# Limpa imagens, containers e redes não utilizados para liberar espaço
docker image history efarmatcc/efarma	# Exibe o histórico da imagem para confirmar atualizações