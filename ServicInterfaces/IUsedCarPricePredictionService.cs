using DataModels.Models;

namespace ServicInterfaces
{
    public interface IUsedCarPricePredictionService
    {
        public Task<string> PredictPrice(PredictionInputs predictionInputs);
        public Task<bool> EnsureEnviroment();
    }
}
