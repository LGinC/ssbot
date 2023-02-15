FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
EXPOSE 80
COPY . .
ENTRYPOINT ["dotnet", "ssbot.dll"]