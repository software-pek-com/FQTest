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
            services.AddSingleton<IMongoClient>(new MongoClient(appSettings.DbUrl));
            services.AddSingleton<IHostedService, ConfigureMongoDbIndexesService>();
            services.AddSingleton<IUserService, UserService>();

            var mapper = ConfigureMapper();
            services.AddSingleton(mapper);

            services.AddControllers();
        }

        public static IMapper ConfigureMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<User, UserDetails>();
                cfg.CreateMap<User, UserSummary>();
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
