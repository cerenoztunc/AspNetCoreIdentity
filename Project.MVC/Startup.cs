using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.MVC.CustomValidations;
using Project.MVC.Models;
using Project.MVC.Models.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC
{
    public class Startup
    {
        public IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppIdentityDbContext>(opt =>
            {
                opt.UseSqlServer(_configuration["ConnectionStrings:DefaultConnectionString"]);
            });

            services.AddIdentity<AppUser, AppRole>(opt=> 
            {
                opt.User.RequireUniqueEmail = true;
                opt.User.AllowedUserNameCharacters = "abc�defg�h�ijklmno�pqrs�tuvwxyzABC�DEFG�HI�JKLMNO�PQRS�TUVWXYZ0123456789 -._";
                 opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
            }).AddPasswordValidator<CustomPasswordValidator>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();

            services.AddMvc(opt=> 
            {
                opt.EnableEndpointRouting = false;
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage(); //al�nan hatalarla ilgili a��klay�c� bilgiler sunar..
            app.UseStatusCodePages(); //�zellikle herhangi bir content d�nmeyen sayfalarda bilgilendirici yaz�lar g�sterir..
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.UseAuthentication();
        }
    }
}
