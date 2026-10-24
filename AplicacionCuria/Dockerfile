# Etapa 1: Compilación (.NET 8 SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /src

COPY ["AplicacionCuria.csproj", "./"]
RUN dotnet restore "AplicacionCuria.csproj"

COPY . .
RUN dotnet publish "AplicacionCuria.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Ejecución (.NET 8 Runtime sobre Ubuntu 22.04 con wkhtmltopdf para PDFs)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS final
WORKDIR /app

# Instalar wkhtmltopdf y fuentes para la generación de certificados PDF con Rotativa
RUN apt-get update && apt-get install -y --no-install-recommends \
    wkhtmltopdf \
    fontconfig \
    fonts-dejavu-core \
    libfreetype6 \
    libx11-6 \
    libxext6 \
    libxrender1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5155
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5155

ENTRYPOINT ["dotnet", "AplicacionCuria.dll"]
