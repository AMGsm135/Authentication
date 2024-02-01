#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Dgab.Authentication.Host/Dgab.Authentication.Host.csproj", "Dgab.Authentication.Host/"]
COPY ["Dgab.Authentication.Application.Contract/Dgab.Authentication.Application.Contract.csproj", "Dgab.Authentication.Application.Contract/"]
COPY ["Dgab.Authentication.Command/Dgab.Authentication.Command.csproj", "Dgab.Authentication.Command/"]
COPY ["Dgab.Authentication.Infrastructure/Dgab.Authentication.Infrastructure.csproj", "Dgab.Authentication.Infrastructure/"]
COPY ["Dgab.Authentication.DomainModel/Dgab.Authentication.DomainModel.csproj", "Dgab.Authentication.DomainModel/"]
COPY ["Dgab.Authentication.Configuration/Dgab.Authentication.Configuration.csproj", "Dgab.Authentication.Configuration/"]
COPY ["Dgab.Authentication.Application/Dgab.Authentication.Application.csproj", "Dgab.Authentication.Application/"]
COPY ["Dgab.Authentication.Application.Events/Dgab.Authentication.Application.Events.csproj", "Dgab.Authentication.Application.Events/"]
COPY ["Dgab.Authentication.Shared/Dgab.Authentication.Shared.csproj", "Dgab.Authentication.Shared/"]
COPY ["Dgab.Authentication.CommandHandler/Dgab.Authentication.CommandHandler.csproj", "Dgab.Authentication.CommandHandler/"]
COPY ["Dgab.Authentication.Persistence/Dgab.Authentication.Persistence.csproj", "Dgab.Authentication.Persistence/"]
COPY ["Dgab.Authentication.QueryModel/Dgab.Authentication.QueryModel.csproj", "Dgab.Authentication.QueryModel/"]
COPY ["Dgab.Authentication.QueryService/Dgab.Authentication.QueryService.csproj", "Dgab.Authentication.QueryService/"]
COPY ["Dgab.Authentication.EventHandler/Dgab.Authentication.EventHandler.csproj", "Dgab.Authentication.EventHandler/"]
RUN dotnet restore "Dgab.Authentication.Host/Dgab.Authentication.Host.csproj"
COPY . .
WORKDIR "/src/Dgab.Authentication.Host"
RUN dotnet build "Dgab.Authentication.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dgab.Authentication.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Install SkiaSharp Dependencies
RUN apt-get update && apt-get install -y libfontconfig1 libgdiplus
ENTRYPOINT ["dotnet", "Dgab.Authentication.Host.dll"]