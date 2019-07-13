﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Server.Application;
using Server.Domain.Model.Users;
using Server.Domain.Service;
using Server.Infrastructure.Auth;
using Server.Infrastructure.Persistence;
using Server.Infrastructure.Users;
using Server.Interfaces.Users.Facade;

namespace Server
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<DodderContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //facade
            services.AddScoped<IUsersFacade, UsersFacade>();
            //application services
            services.AddScoped<IUsersApplicationService, UsersApplicationService>();
            //domain services
            services.AddScoped<IUsersDomainService, UsersDomainService>();
            //infrastructure services
            services.AddScoped<IAuthService, AuthService>();
            //repositories
            services.AddScoped<IUsersRepository, UsersRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            UpdateDatabase(app);
        }

        private void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                      .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DodderContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
