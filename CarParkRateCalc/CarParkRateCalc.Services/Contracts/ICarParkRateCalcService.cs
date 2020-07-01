using CarParkRateCalc.Services.Model;
using System;
using System.Threading.Tasks;

namespace CarParkRateCalc.Services.Contracts
{
    public interface ICarParkRateCalcService
    {
        

        Task<Charge> CalculateRate(DateTime entryTime, DateTime exitTime);
    }
}
