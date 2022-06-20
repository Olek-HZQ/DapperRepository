#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DapperRepo.Web/DapperRepo.Web.csproj", "DapperRepo.Web/"]
COPY ["DapperRepo.Services/DapperRepo.Services.csproj", "DapperRepo.Services/"]
COPY ["DapperRepo.Data/DapperRepo.Data.csproj", "DapperRepo.Data/"]
COPY ["DapperRepo.Core/DapperRepo.Core.csproj", "DapperRepo.Core/"]
RUN dotnet restore "DapperRepo.Web/DapperRepo.Web.csproj"
COPY . .
WORKDIR "/src/DapperRepo.Web"
RUN dotnet build "DapperRepo.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DapperRepo.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DapperRepo.Web.dll"]