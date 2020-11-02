using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mmt.Api.Models;
using Mmt.Api.services;
using Microsoft.Extensions.Azure;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Azure.Core.Extensions;

namespace Mmt.Api
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
            //here we add our user service to the dependency manager container with scoped because i need an instance of this with each call for IUserService
            services.AddScoped<IUserService, UserService>();


            //cors configuration
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });



            //        services.AddControllers().AddNewtonsoftJson(options =>
            //options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddControllers(config =>
            {
                //applying the authorization globally
                //the user should be at least authenticated
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //configuring the connection string of mysql db
            //services.AddDbContext<MmtContext>(options =>
            //       options.UseMySql(Configuration.GetConnectionString("Mmt")));

            services.AddDbContext<MmtContext>(options =>
       options.UseSqlServer(Configuration.GetConnectionString("Mmt")));

            //adding identity to mmt project, this package from miscrosoft wil help us manage the authentication and authorization of application users
            services.AddIdentity<MmtUser, IdentityRole>(option =>
            {
                //the user name should be unique, no 2 users in the db with same email
                option.User.RequireUniqueEmail = true;
                //configuration for password
                option.Password.RequireDigit = true;        //must have numbers
                option.Password.RequireLowercase = true;    //must have a lower case
                option.Password.RequiredLength = 5;         //length must be 5 or more
            })
                .AddEntityFrameworkStores<MmtContext>()     //here we tel identity which class is our context class so it can use it to add the tables needed in the db
                .AddDefaultTokenProviders(); //to generate tokens for email confirmation en password resetting....


            //we add authentication to the api with some configuration
            services.AddAuthentication(auth =>
            {
                //here we tell asp net core which scheme we wil be using, in our case its json web token or jwt
                //jwt is the beste way to secure an api and it's statless behavior makes his almost the only way to secure an api that respects REST principles
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>      //we add jwt to the auth middelware
                {
                    //here we configure how the token should be validate when received of generate a token
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,

                        //i have set those parametre in the appSettings.json
                        ValidAudience = Configuration["AuthSettings:Audience"],
                        ValidIssuer = Configuration["AuthSettings:Issuer"],
                        RequireExpirationTime = true,
                        //IssuerSigningKey is the key that we gonna use to encrypte the token
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:Key"])),
                        ValidateIssuerSigningKey = true
                    };
                });

            // here we add the role based authorization to the DI container
            services.AddAuthorization(o =>
            {
                o.AddPolicy("USER", policy => policy.RequireRole("USER"));
                o.AddPolicy("SUPERVISOR", policy => policy.RequireRole("SUPERVISOR"));
                o.AddPolicy("DIRECTOR", policy => policy.RequireRole("DIRECTOR"));
                o.AddPolicy("ADMIN", policy => policy.RequireRole("ADMIN"));
            });

            //adding swagger for a personalised live testing of the api
            //if you are unconfortable using swagger you stil can use postman

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                     {
                         new OpenApiSecurityScheme
                         {
                            Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            }, new List<string>()
                     }
                     });
            });


            //adding the mailService to DI container
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IAzureFileService, AzureFileService>();
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["ConnectionStrings:DefaultEndpointsProtocol=https;AccountName=mmtapi;AccountKey=WNss7bt5duCuA5srpda26uQ6PLz2FAdJHnV2kAO1Kkkww+fvqP+YS6+cosEvgPZFxB5loCgf3qqQw76JJ5DVlw==;EndpointSuffix=core.windows.net:blob"], preferMsi: true);
                builder.AddQueueServiceClient(Configuration["ConnectionStrings:DefaultEndpointsProtocol=https;AccountName=mmtapi;AccountKey=WNss7bt5duCuA5srpda26uQ6PLz2FAdJHnV2kAO1Kkkww+fvqP+YS6+cosEvgPZFxB5loCgf3qqQw76JJ5DVlw==;EndpointSuffix=core.windows.net:queue"], preferMsi: true);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseDefaultFiles();

            //use static files middelware so our api can serve photos and files
            app.UseStaticFiles();
            app.UseCors();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            //here we use authentication
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    internal static class StartupExtensions
    {
        public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddBlobServiceClient(serviceUri);
            }
            else
            {
                return builder.AddBlobServiceClient(serviceUriOrConnectionString);
            }
        }
        public static IAzureClientBuilder<QueueServiceClient, QueueClientOptions> AddQueueServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddQueueServiceClient(serviceUri);
            }
            else
            {
                return builder.AddQueueServiceClient(serviceUriOrConnectionString);
            }
        }
    }
}
