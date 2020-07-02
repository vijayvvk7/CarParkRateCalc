using AutoMapper;
using CarParkRateCalc.API.Common.Settings;
using CarParkRateCalc.Services.Contracts;
using CarParkRateCalc.Services.Model;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace CarParkRateCalc.Services
{
    public class CarParkRateCalcService : ICarParkRateCalcService
    {
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public CarParkRateCalcService(IOptions<AppSettings> settings, IMapper mapper)
        {
            _settings = settings?.Value;
            _mapper = mapper;
        }        

        private enum RateType { Normal, Weekend, Standard, EarlyBird, Night};

        public async Task<Charge> CalculateRate(DateTime entryTime, DateTime exitTime)
        {
            RateType PriceTier = DeterminePriceTier(entryTime, exitTime);
            return DetermineCharge(PriceTier, entryTime, exitTime);     
           
        }

        /// <summary>
        /// this method calculates actual billing amount
        /// </summary>
        /// <param name="priceTier">price tier applicable as per the entry n exit times</param>
        /// <param name="entryTime"></param>
        /// <param name="exitTime"></param>
        /// <returns></returns>
        private Charge DetermineCharge(RateType priceTier, DateTime entryTime, DateTime exitTime)
        {
            decimal cost = (decimal)0.0;
            switch (priceTier)
            {
                case RateType.EarlyBird:
                    cost = (decimal) 13.0;
                    break;
                case RateType.Night:
                    cost = (decimal)6.5;
                    break;
                case RateType.Weekend:
                    cost = (decimal)10.0;
                    break;
            }

            var totalHours = exitTime.Subtract(entryTime).TotalHours;
            var totalCalendarDays = exitTime.Day - entryTime.Day == 0 ? 1: exitTime.Day - entryTime.Day;
            decimal standardCost;
            if (totalHours < 1)
                standardCost = (decimal)5.0;
            else if (totalHours < 2)
                standardCost = (decimal)10.0;
            else if (totalHours < 3)
                standardCost = (decimal)15.0;
            else
                standardCost = (decimal)(totalCalendarDays * 20); //flat rate for each calendar day.

            //look for the cheapest option for customer; if standard cost is less, pass on the same
            if (cost != 0 && cost < standardCost)
            {
                return new Charge() { Rate = priceTier.ToString(), TotalPrice = cost };
            }
            else
                return new Charge() { Rate = RateType.Standard.ToString(), TotalPrice = standardCost };

        }

        /// <summary>
        /// this method determines the price tier to be applied.
        /// </summary>
        /// <param name="entryTime"></param>
        /// <param name="exitTime"></param>
        /// <returns></returns>
        private RateType DeterminePriceTier(DateTime entryTime, DateTime exitTime)
        {
            //if less than a day
            if (exitTime.Subtract(entryTime) < new TimeSpan(48, 0, 0))
            {
                //determine if its weekend
                if (
                    (entryTime.DayOfWeek == DayOfWeek.Saturday || entryTime.DayOfWeek == DayOfWeek.Sunday) ||
                    (exitTime.DayOfWeek == DayOfWeek.Saturday || exitTime.DayOfWeek == DayOfWeek.Sunday)
                    )
                {
                    return RateType.Weekend;
                }

                else if (entryTime.TimeOfDay >= new TimeSpan(18, 0, 0) && exitTime.TimeOfDay <= new TimeSpan(23, 59, 59))
                {
                    return RateType.Night;
                }
                else if ((entryTime.TimeOfDay >= new TimeSpan(6, 0, 0) && entryTime.TimeOfDay <= new TimeSpan(9, 0, 0))
                    && (exitTime.TimeOfDay >= new TimeSpan(15, 30, 0) && exitTime.TimeOfDay <= new TimeSpan(23, 30, 0))
                    )
                {
                    return RateType.EarlyBird;
                }
            }
            return RateType.Standard;

        }

    }
}
