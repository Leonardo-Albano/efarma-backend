Comandos para o back:

dotnet ef database update 0 
dotnet ef database update
dotnet ef migrations add ""
dotnet ef migrations remove

Comandos para o docker:

docker build -t efarma .
docker run -p 5001:5001 --name efarma efarma

docker ps -a # Listar as imagens
docker rm ecc779908211 # Remove o id escolhido