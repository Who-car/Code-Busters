FROM mcr.microsoft.com/dotnet/runtime:latest

# Задаем ./app как рабочую директорию
WORKDIR /app

# Копируем csproj и восстанавливаем все зависимости
COPY CodeBusters/*.csproj ./CodeBusters/
RUN dotnet restore CodeBusters/

# Собираем приложение
COPY CodeBusters/ ./CodeBusters/
RUN dotnet build CodeBusters/ -c Release -o out

EXPOSE 8081

CMD ["dotnet", "CodeBusters/out/Program.dll"]