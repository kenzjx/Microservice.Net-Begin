# Install Mongodb

cd /src/infra/
run docker-compose up -d
--default connect: mongodb://admin:admin@localhost

# Run Catalog.Service

dotnet restore
dotnet run or dotnet watch run

# Run Inventory.Service

dotnet restore
dotnet run

# Package Source local Common

/src/package
