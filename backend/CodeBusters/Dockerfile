# Задаем базовый образ
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# Задаем рабочую директорию
WORKDIR /app

# Копируем файлы проекта в контейнер
COPY . .

# Восстанавливаем зависимости проекта
RUN dotnet restore

# Собираем проект
RUN dotnet publish -c Release -o out

#-------------------

# Задаем образ для запуска приложения
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime-env

# Задаем рабочую директорию
WORKDIR /app

# Копируем скомпилированное приложение из build-env в runtime-env
COPY --from=build-env /app/out .

# Запускаем приложение
ENTRYPOINT ["dotnet", "CodeBusters.dll"]