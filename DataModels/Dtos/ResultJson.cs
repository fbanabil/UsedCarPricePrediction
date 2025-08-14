using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Models
{
    public class ResultJson
    {
        public float predicted_price { get; set; }
        public string formatted_price { get; set; } = string.Empty;
    }
}
