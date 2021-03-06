﻿using Commissionor.WebApi.Hubs;
using Commissionor.WebApi.Models;
using Commissionor.WebApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Commissionor.WebApi
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
            services.AddMvc();
            services.AddCors();
            services.AddSignalR();
            services.AddTransient<IEventSource, SignalREventSource>();
            services.AddDbContext<CommissionorDbContext>(options => options.UseSqlite(Configuration["ConnectionStrings:Commissionor"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            }

            app.UseMvc();
            app.UseStaticFiles();
            app.UseSignalR(routes => routes.MapHub<EventHub>("/api/events"));
        }
    }
}
