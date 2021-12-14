#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Dx29.MedicalHistory.WebAPI/Dx29.MedicalHistory.WebAPI.csproj", "Dx29.MedicalHistory.WebAPI/"]
COPY ["Dx29.MedicalHistory/Dx29.MedicalHistory.csproj", "Dx29.MedicalHistory/"]
COPY ["Dx29/Dx29.csproj", "Dx29/"]
COPY ["Dx29.Cosmos/Dx29.Cosmos.csproj", "Dx29.Cosmos/"]
RUN dotnet restore "Dx29.MedicalHistory.WebAPI/Dx29.MedicalHistory.WebAPI.csproj"
COPY . .
WORKDIR "/src/Dx29.MedicalHistory.WebAPI"
RUN dotnet build "Dx29.MedicalHistory.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dx29.MedicalHistory.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dx29.MedicalHistory.WebAPI.dll"]