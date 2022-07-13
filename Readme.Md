#Install Mongodb
cd /src/Catalog.Service/docker-mongodb
run docker-compose up -d
--default connect: mongodb://admin:admin@localhost

#Run Catalog.Service
dotnet restore
dotnet run or dotnet watch run
