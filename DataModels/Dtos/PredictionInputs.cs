using System.ComponentModel.DataAnnotations;

namespace DataModels.Models
{
    public class PredictionInputs : IValidatableObject
    {
        public int year { get; set; }
        public string? manufacturer { get; set; }
        public string? model { get; set; }
        public string? condition { get; set; }
        public string? cylinders { get; set; }
        public string? fuel { get; set; }
        public int odometer { get; set; }
        public string? title_status { get; set; }
        public string? transmission { get; set; }
        public string? drive { get; set; }
        public string? type { get; set; }
        public double lat { get; set; }
        public double @long { get; set; }
        public string? description { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (year < 1850 || year > DateTime.Now.Year)
            {
                yield return new ValidationResult("Year must be between 1850 and the current year.", new[] { nameof(year) });
            }
            if (string.IsNullOrWhiteSpace(manufacturer))
            {
                yield return new ValidationResult("Manufacturer is required.", new[] { nameof(manufacturer) });
            }
            if (string.IsNullOrWhiteSpace(model))
            {
                yield return new ValidationResult("Model is required.", new[] { nameof(model) });
            }
            if (string.IsNullOrWhiteSpace(condition))
            {
                yield return new ValidationResult("Condition is required.", new[] { nameof(condition) });
            }
            if (string.IsNullOrWhiteSpace(cylinders))
            {
                yield return new ValidationResult("Cylinders is required.", new[] { nameof(cylinders) });
            }
            if (string.IsNullOrWhiteSpace(fuel))
            {
                yield return new ValidationResult("Fuel type is required.", new[] { nameof(fuel) });
            }
            if (odometer < 0)
            {
                yield return new ValidationResult("Odometer must be a non-negative number.", new[] { nameof(odometer) });
            }
            if (string.IsNullOrWhiteSpace(title_status))
            {
                yield return new ValidationResult("Title status is required.", new[] { nameof(title_status) });
            }
            if (string.IsNullOrWhiteSpace(transmission))
            {
                yield return new ValidationResult("Transmission type is required.", new[] { nameof(transmission) });
            }
            if (string.IsNullOrWhiteSpace(drive))
            {
                yield return new ValidationResult("Drive type is required.", new[] { nameof(drive) });
            }
            if (string.IsNullOrWhiteSpace(type))
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

            if (lat<0 || lat > 1000000000)
            {
                yield return new ValidationResult("Latitude must be a in range 0 to 10000000000.", new[] { nameof(lat) });
            }

            if(@long>0 || @long < -1000000000)
            {
                yield return new ValidationResult("Longitute must be in range from -1000000000 to  0.", new[] { nameof(@long) } );
            }

            

        }
    }
}
