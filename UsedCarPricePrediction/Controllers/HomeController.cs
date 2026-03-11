using DataModels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicInterfaces;
using System.Text.Json;
using UsedCarPricePrediction.Enums;


namespace UsedCarPricePrediction.Controllers
{
    [Route("[controller]/[action]")]
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
        [HttpGet]
        public IActionResult Index()
        {
            return View(new PredictionInputs());
        }



        [HttpPost]
        //[Route("/result")]
        public async Task<IActionResult> Result([FromForm]PredictionInputs predictionInputs)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errorMessage);
            }


            _logger.LogInformation("Received prediction inputs: {@PredictionInputs}", predictionInputs);
            if (predictionInputs == null)
            {
                _logger.LogError("Prediction inputs are null.");
                return BadRequest("Invalid prediction inputs.");
            }

            PredictionResult predictionResult1 = new PredictionResult();
            try
            {
                string predictionResult = await _usedCarPricePredictionService.PredictPrice(predictionInputs);
                ResultJson? json = JsonSerializer.Deserialize<ResultJson>(predictionResult);

                if (json == null)
                {
                    throw new InvalidOperationException("Failed to deserialize prediction result");
                }

                PredictionResultSingle predictionResultSingle = new PredictionResultSingle();
                predictionResultSingle.Currency = "Usd";
                predictionResultSingle.Price = Convert.ToDouble(json.predicted_price);

                predictionResult1.Predictions.Add(predictionResultSingle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while processing prediction. Details : "+ex.Message);
            }

            return PartialView("_ResultPartialView", predictionResult1);
        }

    }
}
