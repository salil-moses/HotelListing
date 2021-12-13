using AspNetCoreRateLimit;
using HotelListing.Configurations;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Repository;
using HotelListing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // services.AddControllers().AddNewtonsoftJson(options =>
            // options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddDbContext<DatabaseContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
            );
            // services.AddControllers();

            services.AddMemoryCache(); // for rate limiting

            services.ConfigureRateLimiting();  // for rate limiting
            services.AddHttpContextAccessor();  // for rate limiting

            //services.AddResponseCaching(); // shifted to the extension class - check line below
            services.ConfigureHttpCacheHeaders();

            //IdentityUser identity
            services.AddAuthentication();
            services.ConfigureIdentity();  // from the class extension
            //var jwtSettings = Configuration["Jwt:Issuer"];            
            // var configSections = Configuration.GetChildren();
            // var keyL = Environment.GetEnvironmentVariables();
            // var key = Environment.GetEnvironmentVariable("USERDOMAIN");
            services.ConfigureJWT(Configuration);
            
            services.AddCors(o => {
                o.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddAutoMapper(typeof(MapperInitializer));
            services.AddTransient<IUnitOfWork, UnitOfWork>(); /*Create new instance every time*/
            services.AddScoped<IAuthManager, AuthManager>();

            
            // services.AddControllers();
            services.AddControllers(config =>
            {
                // create a custom caching profile that may be applied to the controllers
                config.CacheProfiles.Add("120SecondDuration", new CacheProfile { 
                    Duration = 120
                });
            }).AddNewtonsoftJson(options => 
             options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.ConfigureAPIVersioning();  // check serviceextensions for implementation
            /*
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
            });
            */
            AddSwaggerDoc(services);
        }

        private void AddSwaggerDoc(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { 
                    Description = @"JWT Authorization Header using the Bearer Scheme.
                    Enter 'Bearer'[space] and your token in the text input below.
                    Example: 'Bearer abert343'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "OAuth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Moving swagger out of the dev will allow it to be published
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));
            }

            app.ConfigureExceptionHandler(); // Our custom global error handler

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseResponseCaching();  // for api caching
            app.UseHttpCacheHeaders(); // for api caching 

            app.UseIpRateLimiting();   // for api rate limiting -- requires library AspNetCoreRateLimit

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
