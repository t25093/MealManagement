# ===== שלב 1: בנייה =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# קודם רק קבצי הפרויקטים, כדי ש-restore יישמר ב-cache בין בניות
COPY MealManagement.API/MealManagement.API.csproj MealManagement.API/
COPY MealManagement.Core/MealManagement.Core.csproj MealManagement.Core/
COPY MealManagement.Data/MealManagement.Data.csproj MealManagement.Data/
COPY MealManagement.Service/MealManagement.Service.csproj MealManagement.Service/
RUN dotnet restore MealManagement.API/MealManagement.API.csproj

COPY . .
RUN dotnet publish MealManagement.API/MealManagement.API.csproj -c Release -o /app/publish --no-restore

# ===== שלב 2: הרצה =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Render שולח את הבקשות לפורט 10000
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000

ENTRYPOINT ["dotnet", "MealManagement.API.dll"]
