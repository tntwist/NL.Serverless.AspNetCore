using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NL.Serverless.AspNetCore.WebApp.Hubs;
using NL.Serverless.AspNetCore.WebApp.Middelwares;
using NL.Serverless.AspNetCore.WebApp.ORM;

namespace NL.Serverless.AspNetCore.WebApp
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
            services.AddLogging();

            services.AddApplicationInsightsTelemetry();

            services.AddMvc()
                .AddApplicationPart(typeof(Startup).Assembly);

            services.AddRazorPages();

            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.WithOrigins("http://localhost")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            services.AddSignalR();

            services.AddOpenApiDocument();

            services.AddDbContext<WebAppDbContext>(options => options.UseInMemoryDatabase("WebAppDb"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WebAppDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseMiddleware<TestMiddleware>();

            app.UseEndpoints(options => 
            {
                options.MapDefaultControllerRoute();
                options.MapRazorPages();
                options.MapHub<TestHub>("/testhub");
            });

            DataSeeder.Seed(dbContext);
        }
    }
}
