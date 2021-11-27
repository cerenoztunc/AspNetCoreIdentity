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

            services.AddIdentity<AppUser, AppRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.User.AllowedUserNameCharacters = "abc�defg�h�ijklmno�pqrs�tuvwxyzABC�DEFG�HI�JKLMNO�PQRS�TUVWXYZ0123456789 -._";

                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
            }).AddPasswordValidator<CustomPasswordValidator>().AddUserValidator<CustomUserValidator>().AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

            
            CookieBuilder cookieBuilder = new CookieBuilder();
            cookieBuilder.Name = "MyBlog";
            cookieBuilder.HttpOnly = false;
            cookieBuilder.SameSite = SameSiteMode.Lax;  //bir cookie'yi kaydettikten sonra sadece o site �zerinden bu cookie'ye ula�abilmek i�in bu �zelli�in strict yap�lmas� gerekir b�ylece ba�ka herhangi bir siteden eri�ilemezler..Ancsk e�er banka uygulamas� de�ilse bu app o zaman default olan lax'te b�rak�labilir. 
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = new PathString("/Home/Login"); //Login olmayan bir kullan�c�n�n loginkinken eri�ilebilecek bir sayfaya eri�meye �al��t���nda y�nlendirilecek path
                opt.LogoutPath = new PathString("/Member/LogOut");
                opt.Cookie = cookieBuilder;
                opt.SlidingExpiration = true; //e�er kullan�c� siteyi s�rekli ziyaret ediyorsa ve yukar�daki expiration g�n�n�n yar�s�ndan sonra da giri� yapm��sa login durumunu bir 60 g�n daha uzat�r... 
                //kullan�c� bilgisi cookie'de 60 g�n tutulacak..
                opt.ExpireTimeSpan = TimeSpan.FromDays(60);
                opt.AccessDeniedPath = new PathString("/Member/AccessDenied"); //Yukar�dakinin aksine Login olan bir kullan�c�n�n eri�im izni olmayan bir sayfaya y�nlendirmesini sa�layacak path
            });
            services.AddMvc(opt =>
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
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute(); //en altta olmas� gerekir..

        }
    }
}
