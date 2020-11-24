FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5001 5002
ENV ASPNETCORE_URLS=http://*:5001

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["CarApi.csproj", "./"]
RUN dotnet restore "CarApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CarApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarApi.dll"]
