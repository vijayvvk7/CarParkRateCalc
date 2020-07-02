using System;
using System.ComponentModel.DataAnnotations;

namespace CarParkRateCalc.API.DataContracts
{
    public class Charge : IEquatable<Charge>
    {
        [DataType(DataType.Text)]
        public string Rate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Charge)obj);
        }

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        return (x * 397) ^ y;
        //    }
        //}

        public bool Equals(Charge other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Rate == other.Rate && TotalPrice == other.TotalPrice;
        }
    }
}
