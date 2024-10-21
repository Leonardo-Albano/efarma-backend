Comandos para o back:

dotnet ef database update 0 
dotnet ef database update
dotnet ef migrations add ""
dotnet ef migrations remove

Comandos para o docker:

docker build -t efarma .						# Builda a imagem docker
docker run -p 5001:5001 --name efarma efarma	# Roda a imagem docker

docker ps -a									# Listar as imagens
docker rm ecc779908211							# Remove o id escolhido