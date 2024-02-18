#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy both ApiApplication and ApiApplication.Core project files and restore dependencies
COPY ./ApiApplication/*.csproj ./ApiApplication/
COPY ./ApiApplication.Core/*.csproj ./ApiApplication.Core/
RUN dotnet restore ApiApplication/ApiApplication.csproj

# Copy the full solution and build the application
COPY . .
WORKDIR /app/ApiApplication
RUN dotnet build

# Stage 2: Publish the application
FROM build AS publish
CMD pwd
RUN dotnet publish ./ApiApplication.csproj -c Release -o /app/publish
#

#WORKDIR /src
#COPY ./*.csproj ./ApiApplication/
#COPY ../ApiApplication.Core/*.csproj ./ApiApplication.Core/
#RUN dotnet restore "./ApiApplication.csproj"
#COPY . .
#WORKDIR "/src/ApiApplication/."
#RUN dotnet build "ApiApplication.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "ApiApplication.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiApplication.dll"]