using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MoWebApp.Data;
using MoWebApp.Documents;
using MoWebApp.Services;
using RabbitMQ.Client;

namespace MoWebApp
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
            services.AddOptions();

            var settings = Configuration.GetSection(nameof(AppSettings));
            services.Configure<AppSettings>(settings);
            services.AddSingleton(Configuration);

            var appSettings = new AppSettings();
            settings.Bind(appSettings);

            services.AddSingleton<IConnectionFactory>(new ConnectionFactory { Uri = new Uri(appSettings.EventBusUrl) });
            services.AddSingleton<IMongoClient>(new MongoClient(appSettings.DbUrl));
            services.AddSingleton<IUserService, UserService>();

            services.AddHostedService<ConfigureMongoDbIndexesService>();
            services.AddHostedService<UserCreationEventHandlerService>();

            var mapper = ConfigureMapper();
            services.AddSingleton(mapper);

            var factory = new ConnectionFactory { Uri = new Uri(appSettings.EventBusUrl) };
            services.AddSingleton(factory);

            services.AddHttpContextAccessor();
            services.AddControllers();
        }

        public static IMapper ConfigureMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<User, UserDetails>();
                cfg.CreateMap<User, UserSummary>();
                cfg.CreateMap<UserDetails, User>();
            });

            return mapperConfig.CreateMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var settings = Configuration.GetSection(nameof(AppSettings));
            var appSettings = new AppSettings();
            settings.Bind(appSettings);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
