
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FactoryAPI.Models;
using Microsoft.AspNetCore.SignalR;
using FactoryAPI.SignalR;

namespace FactoryAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
 
            services.AddCors(options =>
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder
                        .WithOrigins("http://localhost:4200", 
                                "https://factoryui.azurewebsites.net",
                                "http://factoryui.azurewebsites.net")
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .AllowAnyHeader();
                    }));

        
            services.AddControllers();
            services.AddSignalR();


            services.AddDbContext<DB1Context>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DB1Context")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("_myAllowSpecificOrigins");
            
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotifyHub>("/NotifyHub");
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
