# Estágio de Compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar tudo e restaurar as dependências
COPY . ./
RUN dotnet restore

# Compilar a aplicação em modo Release
RUN dotnet publish -c Release -o out

# Estágio de Execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Comando para rodar a API configurando a porta que o Render espera
ENTRYPOINT ["dotnet", "EcommerseEscalavel.dll"]