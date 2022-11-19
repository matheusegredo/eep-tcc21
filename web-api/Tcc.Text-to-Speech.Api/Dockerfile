#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tcc.Text-to-Speech.Api/Tcc.Text-to-Speech.Api.csproj", "Tcc.Text-to-Speech.Api/"]
COPY ["Tcc.Text-to-Speech.Application/Tcc.Text-to-Speech.Application.csproj", "Tcc.Text-to-Speech.Application/"]
COPY ["Tcc.Text-to-Speech.Domain/Tcc.Text-to-Speech.Domain.csproj", "Tcc.Text-to-Speech.Domain/"]
COPY ["Tcc.Text-to-Speech.Infrastructure/Tcc.Text-to-Speech.Infrastructure.csproj", "Tcc.Text-to-Speech.Infrastructure/"]
RUN dotnet restore "Tcc.Text-to-Speech.Api/Tcc.Text-to-Speech.Api.csproj"
COPY . .
WORKDIR "/src/Tcc.Text-to-Speech.Api"
RUN dotnet build "Tcc.Text-to-Speech.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tcc.Text-to-Speech.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tcc.Text-to-Speech.Api.dll"]