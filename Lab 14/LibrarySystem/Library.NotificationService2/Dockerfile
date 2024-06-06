FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -c release -o /out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /out

COPY --from=build /out ./
EXPOSE 92

ENTRYPOINT ["dotnet", "Library.NotificationService2.dll"]