using AutoMapper;
using CarParkRateCalc.API.DataContracts;
using CarParkRateCalc.API.DataContracts.Requests;
using CarParkRateCalc.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using S = CarParkRateCalc.Services.Model;

namespace CarParkRateCalc.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/CarParkRateCalc")]
    [ApiController]
    public class CarParkRateCalcController : Controller
    {
        private readonly ICarParkRateCalcService _service;
        private readonly IMapper _mapper;

#pragma warning disable CS1591
        public CarParkRateCalcController(ICarParkRateCalcService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
#pragma warning restore CS1591

        #region GET
        /// <summary>
        /// Comments and descriptions can be added to every endpoint using XML comments.
        /// </summary>
        /// <remarks>
        /// XML comments included in controllers will be extracted and injected in Swagger/OpenAPI file.
        /// </remarks>
        /// <param name="entryTime"></param>
        /// <param name="exitTime"></param>
       
        /// <returns>Charge associated with parking service</returns>
        [HttpGet("{id}")]
        public async Task<Charge> CarParkRateCalc(DateTime entryTime, DateTime exitTime)
        {
            var data = await _service.CalculateRate(entryTime, exitTime);

            if (data != null)
                return _mapper.Map<Charge>(data);
            else
                return null;
        }
        #endregion

        
    }
}
