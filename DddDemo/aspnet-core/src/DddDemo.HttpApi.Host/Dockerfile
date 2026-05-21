# 基于官方 ASP.NET Core 镜像的多阶段构建
FROM mcr.microsoft.com/dotnet/aspnet@sha256:8c0b6857eab7b2aa57884c839bf4678414606bd7d173729130a31ec6ef57d892 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk@sha256:c0790639332692a0d56cdd81ed581cfd24d040d9839764c114c6c2a1b7c10ce4 AS build
WORKDIR /workspace
COPY . .
RUN dotnet restore src/DddDemo.HttpApi.Host/DddDemo.HttpApi.Host.csproj
RUN dotnet publish src/DddDemo.HttpApi.Host/DddDemo.HttpApi.Host.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DddDemo.HttpApi.Host.dll"]
