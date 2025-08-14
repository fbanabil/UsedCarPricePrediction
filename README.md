# Used Car Price Prediction

A machine learning-powered web application that predicts used car prices based on various vehicle characteristics. Built with ASP.NET Core and Python machine learning models.

## 🚗 Overview

This application uses advanced machine learning algorithms to predict used car prices by analyzing multiple factors including:

- Vehicle make and model
- Year and mileage (odometer reading)
- Condition and engine specifications
- Fuel type and transmission
- Geographic location
- Vehicle type and title status

## 🛠️ Technology Stack

- **Frontend**: ASP.NET Core MVC with Razor Pages
- **Backend**: C# .NET 8.0
- **Machine Learning**: Python 3.13.0 with scikit-learn
- **UI Framework**: Bootstrap 5 with custom CSS
- **JavaScript**: Vanilla JS with Select2 for enhanced dropdowns

## 📋 Prerequisites

Before running this project, ensure you have the following installed:

- **Python 3.13.0** (Global environment - **Required**)
- **.NET 8.0 SDK** or later
- **Visual Studio 2022** (recommended) or **Visual Studio Code**
- **Git** (for cloning the repository)

### Python Dependencies

The following Python packages are required (automatically installed via requirements.txt):

- scikit-learn
- pandas
- numpy
- joblib
- Additional ML libraries as specified in `requirements.txt`

## 🚀 Getting Started

### Method 1: Using Visual Studio

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd UsedCarPricePrediction
   ```
2. **Open the solution**

   - Open `UsedCarPricePrediction.sln` in Visual Studio
   - Ensure Python 3.13.0 is installed globally
3. **Build and Run**

   - Press `F5` or click "Start Debugging"
   - The application will launch in your default browser

### Method 2: Using Command Line

1. **Clone and navigate to project**

   ```bash
   git clone <repository-url>
   cd UsedCarPricePrediction/UsedCarPricePrediction
   ```
3. **Run the application**

   ```bash
   dotnet run
   ```
4. **Access the application**

   - Open your browser and navigate to the URL shown in the terminal (typically `https://localhost:5001` or `http://localhost:5000`)

## 📁 Project Structure

```
UsedCarPricePrediction/
├── UsedCarPricePrediction.sln          # Solution file
├── README.md                           # This file
├── DataModels/                         # Data transfer objects
│   ├── Dtos/
│   │   ├── PredictionInputs.cs
│   │   ├── PredictionResult.cs
│   │   └── ResultJson.cs
├── Services/                           # Business logic layer
│   ├── UsedCarPricePredictionService.cs
│   └── ServiceUtilities/
│       ├── PythonRunner.cs
│       ├── car_price_inference.py      # Main ML prediction script
│       ├── predict_car_price.py
│       ├── requirements.txt            # Python dependencies
│       └── ML_Models/                  # Pre-trained ML models
│           ├── best_cat_model.pkl
│           ├── best_lgb_model.pkl
│           ├── meta_model.pkl
│           └── ... (other model files)
├── ServiceInterfaces/                  # Service contracts
│   └── IUsedCarPricePredictionService.cs
└── UsedCarPricePrediction/            # Web application
    ├── Controllers/
    │   └── HomeController.cs
    ├── Views/
    │   └── Home/
    │       └── Index.cshtml            # Main prediction form
    ├── wwwroot/
    │   └── StyleSheet.css              # Custom styling
    └── Program.cs                      # Application entry point
```

## 🎯 Features

### Core Functionality

- **Machine Learning Prediction**: Multiple ensemble models for accurate price estimation
- **Professional UI**: Modern, responsive web interface
- **Real-time Validation**: Client-side form validation with professional error handling
- **Geographic Awareness**: Location-based price adjustments using latitude/longitude

### User Interface

- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- **Enhanced Dropdowns**: Select2 integration for better user experience
- **Loading Indicators**: Professional loading animations during prediction
- **Error Handling**: Contextual error messages with professional styling

### Technical Features

- **Ensemble Learning**: Combines multiple ML algorithms (CatBoost, LightGBM, XGBoost)
- **Data Preprocessing**: Automated feature engineering and data transformation
- **Model Persistence**: Pre-trained models for fast prediction response
- **Cross-platform**: Runs on Windows, macOS, and Linux

## 📊 Machine Learning Models

The application uses an ensemble of machine learning models:

1. **CatBoost**: Gradient boosting with categorical feature support
2. **LightGBM**: Fast gradient boosting framework
3. **XGBoost**: Extreme gradient boosting
4. **Meta-model**: Combines predictions from individual models

### Model Features

- **Feature Engineering**: Automated preprocessing and transformation
- **Geographic Clustering**: K-means clustering for location-based features
- **Power Transformations**: Box-Cox and other transformations for better model performance
- **Imputation**: Handles missing values intelligently

## 🔧 Configuration

### Python Environment Setup

Ensure Python 3.13.0 is installed and accessible globally:

```bash
python --version  # Should output: Python 3.13.0
```

### Application Settings

The application uses standard ASP.NET Core configuration files:

- `appsettings.json`: Production settings
- `appsettings.Development.json`: Development settings

## 🐛 Troubleshooting

### Common Issues

1. **Python not found error**

   - Ensure Python 3.13.0 is installed globally
   - Check that Python is added to your system PATH
2. **Port already in use**

   - The application will automatically find an available port
   - Check the terminal output for the correct URL
3. **Model loading errors**

   - Ensure all `.pkl` files are present in the `ML_Models` directory
   - Check that the Python environment has all required packages

### Development Tips

- Use Visual Studio's debugging features for C# code
- Check browser console for JavaScript errors
- Monitor the terminal output for Python script execution logs

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

📞 Support

For support, please open an issue in the GitHub repository or contact the development team.

**Happy Predicting! 🚗💰**
