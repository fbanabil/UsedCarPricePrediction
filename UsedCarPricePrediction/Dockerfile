# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root

# Install Python 3 and pip
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        python3 \
        python3-pip \
        libgomp1 \
        python3-venv && \
    rm -rf /var/lib/apt/lists/*

# Set working directory for Python setup
WORKDIR /app/Services/ServiceUtilities

# Copy Python dependencies and install them
COPY ["Services/ServiceUtilities/requirements.txt", "./requirements.txt"]
RUN pip3 install --no-cache-dir --break-system-packages -r requirements.txt

# Copy Python scripts
COPY ["Services/ServiceUtilities/predict_car_price.py", "./"]
COPY ["Services/ServiceUtilities/car_price_inference.py", "./"]

# Copy ML Models directory
COPY ["Services/ServiceUtilities/ML_Models/", "./ML_Models/"]

# Grant read & execute access to the Python files
RUN chmod -R 755 /app/Services

USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UsedCarPricePrediction/UsedCarPricePrediction.csproj", "UsedCarPricePrediction/"]
COPY ["DataModels/DataModels.csproj", "DataModels/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["ServicInterfaces/ServiceInterfaces.csproj", "ServicInterfaces/"]
RUN dotnet restore "./UsedCarPricePrediction/UsedCarPricePrediction.csproj"
COPY . .
WORKDIR "/src/UsedCarPricePrediction"
RUN dotnet build "./UsedCarPricePrediction.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UsedCarPricePrediction.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN pip3 install --no-cache-dir \
    pandas \
    numpy \
    scikit-learn \
    scipy \
    joblib \
    xgboost \
    lightgbm \
    catboost
ENTRYPOINT ["dotnet", "UsedCarPricePrediction.dll"]