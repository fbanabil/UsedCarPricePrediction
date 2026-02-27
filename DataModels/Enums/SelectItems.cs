using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UsedCarPricePrediction.Enums
{
    public enum ConditionSelectItem
    {
        [Display(Name = "good")]
        Good,

        [Display(Name = "new")]
        New,

        [Display(Name = "likenew")]
        LikeNew,

        [Display(Name = "excellent")]
        Excellent,

        [Display(Name = "fair")]
        Fair,

        [Display(Name = "salvage")]
        Salvage
    }

    public enum ManufacturerSelectItem
    {
        [Display(Name = "toyota")]
        Toyota,

        [Display(Name = "gmc")]
        GMC,

        [Display(Name = "chevrolet")]
        Chevrolet,

        [Display(Name = "ford")]
        Ford,

        [Display(Name = "jeep")]
        Jeep,

        [Display(Name = "nissan")]
        Nissan,

        [Display(Name = "ram")]
        Ram,

        [Display(Name = "mazda")]
        Mazda,

        [Display(Name = "cadillac")]
        Cadillac,

        [Display(Name = "honda")]
        Honda
    }

    public enum FuelSelectItem
    {
        [Display(Name = "gas")]
        Gas,

        [Display(Name = "diesel")]
        Diesel,

        [Display(Name = "electric")]
        Electric,

        [Display(Name = "hybrid")]
        Hybrid,

        [Display(Name = "other")]
        Other
    }

    public enum TransmissionSelectItem
    {
        [Display(Name = "automatic")]
        Automatic,

        [Display(Name = "manual")]
        Manual,

        [Display(Name = "other")]
        Other
    }

    public enum CylindersSelectItem
    {
        [Display(Name = "3 cylinders")]
        Three = 3,

        [Display(Name = "4 cylinders")]
        Four = 4,

        [Display(Name = "5 cylinders")]
        Five = 5,

        [Display(Name = "6 cylinders")]
        Six = 6,

        [Display(Name = "8 cylinders")]
        Eight = 8,

        [Display(Name = "10 cylinders")]
        Ten = 10,

        [Display(Name = "12 cylinders")]
        Twelve = 12,

        [Display(Name = "other")]
        Other = 0
    }

    public enum DriveSelectItem
    {
        [Display(Name = "fwd")]
        Fwd,

        [Display(Name = "rwd")]
        Rwd,

        [Display(Name = "4wd")]
        FourWd
    }

    public enum ModelSelectItem
    {
        [Display(Name = "camry")]
        Camry,

        [Display(Name = "sierra_1500_crew_cab_slt")]
        Sierra1500CrewCabSlt,

        [Display(Name = "silverado_1500")]
        Silverado1500,

        [Display(Name = "silverado_1500_crew")]
        Silverado1500Crew,

        [Display(Name = "tundra_double_cab_sr")]
        TundraDoubleCabSr,

        [Display(Name = "f_150_xlt")]
        F150Xlt,

        [Display(Name = "silverado_1500_extended_cab")]
        Silverado1500ExtendedCab,

        [Display(Name = "silverado_1500_double")]
        Silverado1500Double,

        [Display(Name = "tacoma")]
        Tacoma,

        [Display(Name = "colorado_extended_cab")]
        ColoradoExtendedCab,

        [Display(Name = "other")]
        Other
    }

    public enum TypeSelectItem
    {
        [Display(Name = "sedan")]
        Sedan,

        [Display(Name = "pickup")]
        Pickup,

        [Display(Name = "truck")]
        Truck,

        [Display(Name = "suv")]
        SUV,

        [Display(Name = "coupe")]
        Coupe,

        [Display(Name = "hatchback")]
        Hatchback,

        [Display(Name = "mini-van")]
        Minivan,

        [Display(Name = "offroad")]
        Offroad,

        [Display(Name = "bus")]
        Bus,

        [Display(Name = "other")]
        Other
    }

    public enum TitleStatusSelectItem
    {
        [Display(Name = "clean")]
        Clean,

        [Display(Name = "rebuilt")]
        Rebuilt,

        [Display(Name = "lien")]
        Lien,

        [Display(Name = "salvage")]
        Salvage,

        [Display(Name = "missing")]
        Missing,

        [Display(Name = "parts only")]
        PartsOnly
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            if (value == null) return string.Empty;

            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

            var display = member?.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? value.ToString();
        }

        public static string? GetDisplayName<T>(this T? enumValue) where T : struct, Enum
        {
            if (!enumValue.HasValue) return null;
            return enumValue.Value.GetDisplayName();
        }
    }
}