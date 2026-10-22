FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto y restaurar dependencia
COPY AplicacionCuria.csproj ./
RUN dotnet restore

# Copiar todo el código fuente y compilar la aplicación
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto que la aplicación escucha
EXPOSE 8080
EXPOSE 8081

# Punto de entrada para la aplicación
ENTRYPOINT ["dotnet", "AplicacionCuria.dll"]



