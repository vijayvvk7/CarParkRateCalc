﻿using AutoMapper;
using CarParkRateCalc.API.Controllers.V1;
using CarParkRateCalc.API.DataContracts.Requests;
using CarParkRateCalc.API.DataContracts;
using CarParkRateCalc.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace CarParkRateCalc.API.Tests.Controllers.ControllerTests.V1
{
    [TestClass]
    public class CarParkRateCalcControllerTests : TestBase
    {
        //NOTE: should be replaced by an interface
        CarParkRateCalcController _controller;

        public CarParkRateCalcControllerTests() : base()
        {
            var businessService = _serviceProvider.GetRequiredService<ICarParkRateCalcService>();
            var mapper = _serviceProvider.GetRequiredService<IMapper>();
            var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<CarParkRateCalcController>();

            _controller = new CarParkRateCalcController(businessService, mapper, logger);
        }

        [TestMethod]
        public async Task CheckEarlyBird_Nominal_OK()
        {
            //Simple test
            var charge = await _controller.CalculateRate(DateTime.Today.AddHours(7), DateTime.Today.AddDays(15));

            Assert.IsNotNull(charge);
        }

        [TestMethod]
        public async Task CheckEarlyBird_SameDay_OK()
        {
            //earlybird charge of 13
            var charge = await _controller.CalculateRate(DateTime.Parse("2020-01-01T07:00"), DateTime.Parse("2020-01-01T17:00"));
            Assert.IsNotNull(charge);
            Assert.IsTrue( (charge.TotalPrice == (decimal)13.0 && charge.Rate == "EarlyBird"));
            
        }

        [TestMethod]
        public async Task CheckNightRate_SameDay_OK()
        {
            //earlybird charge of 13
            var charge = await _controller.CalculateRate(DateTime.Parse("2020-01-01T18:00"), DateTime.Parse("2020-01-01T23:30"));
            Assert.IsNotNull(charge);
            Assert.IsTrue((charge.TotalPrice == (decimal)6.5 && charge.Rate == "Night"));
            
        }

        [TestMethod]
        public async Task CheckWeekendRate_SameDay_OK()
        {
            //weekend charge of 10
            var charge = await _controller.CalculateRate(DateTime.Parse("2020-01-04T18:00"), DateTime.Parse("2020-01-04T23:30"));
            Assert.IsNotNull(charge);
            Assert.IsTrue((charge.TotalPrice == (decimal)10 && charge.Rate == "Weekend"));
            
        }
        [TestMethod]
        public async Task CheckWeekendRate_For1Hour_OK()
        {
            //weekend charge of 10
            var charge = await _controller.CalculateRate(DateTime.Parse("2020-01-04T18:00"), DateTime.Parse("2020-01-04T23:30"));
            Assert.IsNotNull(charge);
            Assert.IsTrue((charge.TotalPrice == (decimal)10 && charge.Rate == "Weekend"));

        }


    }
}