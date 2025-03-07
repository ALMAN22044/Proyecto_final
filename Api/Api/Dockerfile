FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos de la aplicación y compilar
COPY . ./
RUN dotnet publish -c Release -o out

# Crear imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Exponer puerto y ejecutar la aplicación
EXPOSE 80
CMD ["dotnet", "Api.dll"]