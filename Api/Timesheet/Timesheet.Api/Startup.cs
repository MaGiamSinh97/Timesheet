using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text;
using Timesheet.Api.Services;
using Timesheet.Infrastructure.Persistence;

namespace Timesheet.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string specificOrigins = "LOCALHOST";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var contextAssembly = typeof(TimesheetContext).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContextPool<TimesheetContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(nameof(TimesheetContext)), b => b.MigrationsAssembly(contextAssembly));
                //options.UseSqlServer(connection, b => b.MigrationsAssembly("Project.Api"))
            }
                );
            services.AddScoped<EmployeeService>();
            services.AddScoped<TimesheetService>();
            services.AddScoped<ProjectService>();
            services.AddScoped<ProjectEmployeeService>();
            services.AddScoped<TimeworkService>();

            services.AddCors(options =>
            {
                options.AddPolicy(name: specificOrigins,
                        builder =>
                        {
                            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                        });
            });

            services.AddControllers(options =>
            {
                // default is false
                // if the accept-header is not supported, we do not return the default(which is json), we return 406 
                options.ReturnHttpNotAcceptable = true;
            })
            // We need to add NewtonsoftJson for Newtonsoft, because the default JSON serializer is not yet compatible with patch document as of today
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            // We have to move AddXmlDataContractSerializerFormatters after AddNewtonsoftJson, because if we don't it will override the default JSON as response(XML will be returned if no accept header is supplied)
            .AddXmlDataContractSerializerFormatters()// add xml, note that json is still the default
              .ConfigureApiBehaviorOptions(options =>
              {
                  options.InvalidModelStateResponseFactory = context =>
                  {
                      // create a problem details object
                      var problemDetailsFactory = context.HttpContext.RequestServices
                          .GetRequiredService<ProblemDetailsFactory>();
                      var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                              context.HttpContext,
                              context.ModelState);

                      // add additional info not added by default
                      problemDetails.Detail = "See the errors field for details.";
                      problemDetails.Instance = context.HttpContext.Request.Path;

                      // find out which status code to use
                      var actionExecutingContext =
                            context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                      // if there are modelstate errors & all keys were correctly
                      // found/parsed we're dealing with validation errors
                      if ((context.ModelState.ErrorCount > 0) &&
                          (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                      {
                          problemDetails.Type = "https://Timesheet.com/modelvalidationproblem";
                          problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                          problemDetails.Title = "One or more validation errors occurred.";

                          return new UnprocessableEntityObjectResult(problemDetails)
                          {
                              ContentTypes = { "application/problem+json" }
                          };
                      }

                      // if one of the keys wasn't correctly found / couldn't be parsed
                      // we're dealing with null/unparsable input
                      problemDetails.Status = StatusCodes.Status400BadRequest;
                      problemDetails.Title = "One or more errors on input occurred.";
                      return new BadRequestObjectResult(problemDetails)
                      {
                          ContentTypes = { "application/problem+json" }
                      };
                  };
              });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Timesheet.Api", Version = "v1" });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.SeedDataTimesheetContext();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Timesheet.Api v1"));
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    // Customize the message display when internal server error happens
                    appBuilder.Run(async context =>
                    {
                        // log error here
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(specificOrigins);

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
