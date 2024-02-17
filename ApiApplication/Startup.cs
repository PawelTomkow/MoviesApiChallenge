using System;
using ApiApplication.Clients;
using ApiApplication.Clients.Cache;
using ApiApplication.Configurations;
using ApiApplication.Configurations.Extensions;
using ApiApplication.Core.Services;
using ApiApplication.Database;
using ApiApplication.Database.Repositories;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiApplication
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
            services.Configure<ApiClientConfiguration>(Configuration.GetSection(ApiClientConfiguration.Name));
            services.Configure<CacheConfiguration>(Configuration.GetSection(CacheConfiguration.Name));
            services.Configure<ReservationConfiguration>(Configuration.GetSection(ReservationConfiguration.Name));

            services.AddAutoMapper(typeof(Startup));
            services.AddValidatorsFromAssemblyContaining(typeof(Startup));

            services.AddScoped<ICacheRepository, RedisCacheRepository>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services
                .AddCache(Configuration)
                .AddApiClient(Configuration);
            
            services.AddTransient<IShowtimesRepository, ShowtimesRepository>();
            services.AddTransient<ITicketsRepository, TicketsRepository>();
            services.AddTransient<IAuditoriumsRepository, AuditoriumsRepository>();

            services.AddScoped<IMovieService, MovieService>();
            
            services.AddDbContext<CinemaContext>(options =>
            {
                options.UseInMemoryDatabase("CinemaDb")
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            services.AddControllers();

            services.AddHttpClient();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (!Configuration.GetValue<bool>("IsTest"))
            {
                SampleData.Initialize(app);

            }
        }      
    }
}
