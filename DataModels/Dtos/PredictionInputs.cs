using System.ComponentModel.DataAnnotations;
using UsedCarPricePrediction.Enums;

namespace DataModels.Models
{
    public class PredictionInputs : IValidatableObject
    {
        public int? year { get; set; }
        public ManufacturerSelectItem? manufacturer { get; set; }
        public ModelSelectItem? model { get; set; }
        public ConditionSelectItem? condition { get; set; }
        public CylindersSelectItem? cylinders { get; set; }
        public FuelSelectItem? fuel { get; set; }
        public int odometer { get; set; }
        public TitleStatusSelectItem? title_status { get; set; }
        public TransmissionSelectItem? transmission { get; set; }
        public DriveSelectItem? drive { get; set; }
        public TypeSelectItem? type { get; set; }
        public double? Lat { get; set; }
        public double? Long { get; set; }
        public string? description { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (year < 1850 || year > DateTime.Now.Year)
            {
                yield return new ValidationResult("Year must be between 1850 and the current year.", new[] { nameof(year) });
            }
            if (string.IsNullOrWhiteSpace(manufacturer.ToString()))
            {
                yield return new ValidationResult("Manufacturer is required.", new[] { nameof(manufacturer) });
            }
            if (string.IsNullOrWhiteSpace(model.ToString()))
            {
                yield return new ValidationResult("Model is required.", new[] { nameof(model) });
            }
            if (string.IsNullOrWhiteSpace(condition.ToString()))
            {
                yield return new ValidationResult("Condition is required.", new[] { nameof(condition) });
            }
            if (string.IsNullOrWhiteSpace(cylinders.ToString()))
            {
                yield return new ValidationResult("Cylinders is required.", new[] { nameof(cylinders) });
            }
            if (string.IsNullOrWhiteSpace(fuel.ToString()))
            {
                yield return new ValidationResult("Fuel type is required.", new[] { nameof(fuel) });
            }
            if (odometer < 0)
            {
                yield return new ValidationResult("Odometer must be a non-negative number.", new[] { nameof(odometer) });
            }
            if (string.IsNullOrWhiteSpace(title_status.ToString()))
            {
                yield return new ValidationResult("Title status is required.", new[] { nameof(title_status) });
            }
            if (string.IsNullOrWhiteSpace(transmission.ToString()))
            {
                yield return new ValidationResult("Transmission type is required.", new[] { nameof(transmission) });
            }
            if (string.IsNullOrWhiteSpace(drive.ToString()))
            {
                yield return new ValidationResult("Drive type is required.", new[] { nameof(drive) });
            }
            if (string.IsNullOrWhiteSpace(type.ToString()))
            {
                yield return new ValidationResult("Type is required.", new[] { nameof(type) });
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                yield return new ValidationResult("Description is required.", new[] { nameof(description) });
            }
            if (description?.Length > 500)
            {
                yield return new ValidationResult("Description must be less than 500 characters.", new[] { nameof(description) });
            }

            if (Lat>90 || Lat < -90)
            {
                yield return new ValidationResult("Latitude must be a in range -90 to +90.", new[] { nameof(Lat) });
            }

            if(Long<-180 || Long > 180)
            {
                yield return new ValidationResult("Longitute must be in range from -180 to  +180.", new[] { nameof(Long) } );
            }
        }
    }
}
