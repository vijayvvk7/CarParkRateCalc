using System;
using System.Collections.Generic;
using System.Text;

namespace CarParkRateCalc.API.DataContracts.Requests
{
    public class CarParkCalcRequest
    {
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
    }
}
