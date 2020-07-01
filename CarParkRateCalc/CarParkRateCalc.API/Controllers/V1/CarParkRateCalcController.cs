using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CarParkRateCalc.API.DataContracts;
using CarParkRateCalc.API.DataContracts.Requests;
using CarParkRateCalc.Services.Contracts;
using System;
using System.Threading.Tasks;
using S = CarParkRateCalc.Services.Model;

namespace CarParkRateCalc.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/users")]//required for default versioning
    [Route("api/v{version:apiVersion}/users")]
    [ApiController]
    public class CarParkRateCalcController : Controller
    {
        private readonly ICarParkRateCalcService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CarParkRateCalcController> _logger;

#pragma warning disable CS1591
        public CarParkRateCalcController(ICarParkRateCalcService service, IMapper mapper, ILogger<CarParkRateCalcController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }
#pragma warning restore CS1591

        #region GET
        /// <summary>
        /// Returns charge entity according to the provided entry and exit times.
        /// </summary>
        /// <remarks>
        /// XML comments included in controllers will be extracted and injected in Swagger/OpenAPI file.
        /// </remarks>
        /// <param name="entryTime">Time at which vehicle entered car park</param>
        /// <param name="exitTime">Time at which vehicle exited car park</param>

        /// <returns>
        /// Returns a user entity according to the provided Id.
        /// </returns>
        /// <response code="201">Returns the newly created item.</response>
        /// <response code="204">If the item is null.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Charge))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Charge))]
        [HttpGet("{entryTime}, {exitTime}")]
        public async Task<Charge> CalculateRate(DateTime entryTime, DateTime exitTime)
        {
            _logger.LogDebug($"CarParkRateCalcControllers::CarParkRate - for ::{entryTime} to {exitTime}");

            var data = await _service.CalculateRate(entryTime, exitTime);

            if (data != null)
                return _mapper.Map<Charge>(data);
            else
                return null;
        }
        #endregion

       

        #region Excepions
        [HttpGet("exception/{message}")]
        [ProducesErrorResponseType(typeof(Exception))]
        public async Task RaiseException(string message)
        {
            _logger.LogDebug($"CarParkRateCalcControllers::RaiseException::{message}");

            throw new Exception(message);
        }

        
        #endregion
    }
}
