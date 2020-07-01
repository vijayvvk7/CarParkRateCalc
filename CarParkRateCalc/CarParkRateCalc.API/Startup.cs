using CarParkRateCalc.API.Common.Attributes;
using CarParkRateCalc.API.Common.Middlewares;
using CarParkRateCalc.API.Common.Settings;
using CarParkRateCalc.API.Swagger;
using CarParkRateCalc.IoC.Configuration.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;

#pragma warning disable CS1591
namespace CarParkRateCalc.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; private set; }

        private AppSettings _appSettings;

        private readonly ILogger _logger;
        private IServiceProvider _serviceProvider;

        public Startup(IConfiguration configuration, IWebHostEnvironment env, IServiceProvider serviceProvider, ILogger<Startup> logger)
        {
            HostingEnvironment = env;
            Configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;

            //AppSettings
            _appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            _logger.LogDebug("Startup::Constructor::Settings loaded");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogTrace("Startup::ConfigureServices");

            try
            {
                if (_appSettings.IsValid())
                {
                    _logger.LogDebug("Startup::ConfigureServices::valid AppSettings");

                    services.AddControllers(
                        opt =>
                        {
                            //Custom filters, if needed
                            //opt.Filters.Add(typeof(CustomFilterAttribute));
                            opt.Filters.Add(new ProducesAttribute("application/json"));
                        }
                        ).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

                    //API versioning
                    services.AddApiVersioning(
                        o =>
                        {
                            //o.Conventions.Controller<CarParkRateCalcController>().HasApiVersion(1, 0);
                            o.ReportApiVersions = true;
                            o.AssumeDefaultVersionWhenUnspecified = true;
                            o.DefaultApiVersion = new ApiVersion(1, 0);
                            o.ApiVersionReader = new UrlSegmentApiVersionReader();
                        }
                        );

                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    services.AddVersionedApiExplorer(
                    options =>
                    {
                        options.GroupNameFormat = "'v'VVV";

                        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                        // can also be used to control the format of the API version in route templates
                        options.SubstituteApiVersionInUrl = true;
                    });


                    //SWAGGER
                    if (_appSettings.Swagger.Enabled)
                    {
                        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

                        services.AddSwaggerGen(options =>
                        {
                            options.OperationFilter<SwaggerDefaultValues>();
                            options.IncludeXmlComments(XmlCommentsFilePath);
                        });
                    }

                    //Mappings
                    services.ConfigureMappings();

                    //Business settings            
                    services.ConfigureBusinessServices(Configuration);

                    _logger.LogDebug("Startup::ConfigureServices::ApiVersioning, Swagger and DI settings");
                }
                else
                    _logger.LogDebug("Startup::ConfigureServices::invalid AppSettings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            _logger.LogTrace("Startup::Configure");
            _logger.LogDebug($"Startup::Configure::Environment:{env.EnvironmentName}");

            try
            {
                if (env.IsDevelopment())
                    app.UseDeveloperExceptionPage();
                else
                {
                    //Both alternatives are usable for general error handling:
                    // - middleware
                    // - UseExceptionHandler()

                    //app.UseMiddleware(typeof(ErrorHandlingMiddleware));

                    app.UseExceptionHandler(a => a.Run(async context =>
                    {
                        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = feature.Error;
                        var code = HttpStatusCode.InternalServerError;

                        if (exception is ArgumentNullException) code = HttpStatusCode.BadRequest;
                        else if (exception is ArgumentException) code = HttpStatusCode.BadRequest;
                        else if (exception is UnauthorizedAccessException) code = HttpStatusCode.Unauthorized;

                        _logger.LogError($"GLOBAL ERROR HANDLER::HTTP:{code}::{exception.Message}");

                        //Known issue for now in System.Text.Json
                        //var result = JsonSerializer.Serialize<Exception>(exception, new JsonSerializerOptions { WriteIndented = true });

                        //Newtonsoft.Json serializer (should be replaced once the known issue in System.Text.Json will be solved)
                        var result = JsonConvert.SerializeObject(exception, Formatting.Indented);

                        context.Response.Clear();
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(result);
                    }));

                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
                app.UseRequestLocalization();

                //SWAGGER
                if (_appSettings.IsValid())
                {
                    if (_appSettings.Swagger.Enabled)
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI(options =>
                        {
                            foreach (var description in provider.ApiVersionDescriptions)
                            {
                                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                                //options.RoutePrefix = string.Empty;
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}
