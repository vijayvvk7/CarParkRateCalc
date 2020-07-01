using System.ComponentModel.DataAnnotations;

namespace CarParkRateCalc.API.DataContracts
{
    public class Charge
    {
        [DataType(DataType.Text)]
        public string Rate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        

    }
}
