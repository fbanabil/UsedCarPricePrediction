using DataModels.Models;
using Microsoft.AspNetCore.Mvc;
using ServicInterfaces;
using System.Text.Json;


namespace UsedCarPricePrediction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsedCarPricePredictionService _usedCarPricePredictionService;

        public HomeController(ILogger<HomeController> logger, IUsedCarPricePredictionService usedCarPricePredictionService)
        {
            _logger = logger;
            _usedCarPricePredictionService = usedCarPricePredictionService;
        }

        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/result")]
        public async Task<IActionResult> Result([FromBody]PredictionInputs  predictionInputs)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errorMessage);
            }


            //_logger.LogInformation("Received prediction inputs: {@PredictionInputs}", predictionInputs);
            if (predictionInputs == null)
            {
                _logger.LogError("Prediction inputs are null.");
                return BadRequest("Invalid prediction inputs.");
            }

            PredictionResult predictionResult1 = new PredictionResult();
            try
            {
                string predictionResult = await _usedCarPricePredictionService.PredictPrice(predictionInputs);
                ResultJson json = JsonSerializer.Deserialize<ResultJson>(predictionResult);

                PredictionResultSingle predictionResultSingle = new PredictionResultSingle();

                predictionResultSingle.Currency = "Usd";
                predictionResultSingle.Price = Convert.ToDouble(json.predicted_price);

                predictionResult1.Predictions.Add(predictionResultSingle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while processing prediction.");
            }

            return PartialView("_ResultPartialView", predictionResult1);
        }

    }
}
