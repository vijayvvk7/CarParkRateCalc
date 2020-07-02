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
using System.Net.Mime;
using System.Net.Http;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace CarParkRateCalc.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/CarParkRateCalc")]//required for default versioning
    [Route("api/v{version:apiVersion}/CarParkRateCalc")]
    [ApiController]
    public class CarParkRateCalcController : Controller
    {
        private readonly ICarParkRateCalcService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CarParkRateCalcController> _logger;

#pragma warning disable CS1591
        public CarParkRateCalcController(ICarParkRateCalcService service, IMapper mapper, ILogger<CarParkRateCalcController> logger)
        {
            
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException("mapper", "mapper is mandatory parameter and cannot be null"); ;
            _logger = logger ?? throw new ArgumentNullException("logger", "logger is mandatory parameter and cannot be null"); ;
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
        [HttpGet("entryTime={entryTime}&exitTime={exitTime}")]
        public async Task<IActionResult> CalculateRate(DateTime entryTime, DateTime exitTime)
        {
            if (entryTime == DateTime.MinValue || exitTime == DateTime.MinValue)
            {
                var message = "Entry and exit times cannot be null";
                _logger.LogDebug($"CarParkRateCalcControllers::return as bad request::{message}");
                return BadRequest(message);
            }
            if(entryTime > exitTime)
            {
                var message = "Entry cannot be less than exit time";
                _logger.LogDebug($"CarParkRateCalcControllers::return as bad request::{message}");
                return BadRequest(message);
            }
            
            
            
                _logger.LogDebug($"CarParkRateCalcControllers::CarParkRate - for ::{entryTime} to {exitTime}");

                var data = await _service.CalculateRate(entryTime, exitTime);

            if (data != null)
                return Ok(_mapper.Map<Charge>(data));
            else                
                return NoContent();
            
        }
        #endregion

       

        
    }
}
