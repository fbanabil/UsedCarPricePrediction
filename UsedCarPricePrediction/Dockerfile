# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root

# Install Python 3.13.0 and dependencies
RUN apt-get update && apt-get install -y \
    wget \
    build-essential \
    libssl-dev \
    zlib1g-dev \
    libbz2-dev \
    libreadline-dev \
    libsqlite3-dev \
    libncursesw5-dev \
    libffi-dev \
    liblzma-dev \
    && rm -rf /var/lib/apt/lists/*

# Download and install Python 3.13.0
RUN wget https://www.python.org/ftp/python/3.13.0/Python-3.13.0.tgz \
    && tar -xf Python-3.13.0.tgz \
    && cd Python-3.13.0 \
    && ./configure --enable-optimizations \
    && make -j$(nproc) \
    && make altinstall \
    && cd .. \
    && rm -rf Python-3.13.0.tgz Python-3.13.0

# Create symbolic links for python3 and pip3
RUN ln -s /usr/local/bin/python3.13 /usr/local/bin/python3 \
    && ln -s /usr/local/bin/pip3.13 /usr/local/bin/pip3

# Upgrade pip
RUN pip3 install --upgrade pip

# Install Python ML dependencies
RUN pip3 install --no-cache-dir \
    pandas \
    numpy \
    scikit-learn \
    scipy \
    joblib \
    xgboost

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
ENTRYPOINT ["dotnet", "UsedCarPricePrediction.dll"]