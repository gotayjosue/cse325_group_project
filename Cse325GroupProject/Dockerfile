# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Cse325GroupProject.csproj", "./"]
RUN dotnet restore "./Cse325GroupProject.csproj"

# Copy the rest of the application code
COPY . .

# Build the application
RUN dotnet build "Cse325GroupProject.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Cse325GroupProject.csproj" -c Release -o /app/publish

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cse325GroupProject.dll"]
