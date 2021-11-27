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
                opt.User.AllowedUserNameCharacters = "abcçdefgðhýijklmnoöpqrsþtuvwxyzABCÇDEFGÐHIÝJKLMNOÖPQRSÞTUVWXYZ0123456789 -._";

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
            cookieBuilder.SameSite = SameSiteMode.Lax;  //bir cookie'yi kaydettikten sonra sadece o site üzerinden bu cookie'ye ulaþabilmek için bu özelliðin strict yapýlmasý gerekir böylece baþka herhangi bir siteden eriþilemezler..Ancsk eðer banka uygulamasý deðilse bu app o zaman default olan lax'te býrakýlabilir. 
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = new PathString("/Home/Login"); //Login olmayan bir kullanýcýnýn loginkinken eriþilebilecek bir sayfaya eriþmeye çalýþtýðýnda yönlendirilecek path
                opt.LogoutPath = new PathString("/Member/LogOut");
                opt.Cookie = cookieBuilder;
                opt.SlidingExpiration = true; //eðer kullanýcý siteyi sürekli ziyaret ediyorsa ve yukarýdaki expiration gününün yarýsýndan sonra da giriþ yapmýþsa login durumunu bir 60 gün daha uzatýr... 
                //kullanýcý bilgisi cookie'de 60 gün tutulacak..
                opt.ExpireTimeSpan = TimeSpan.FromDays(60);
                opt.AccessDeniedPath = new PathString("/Member/AccessDenied"); //Yukarýdakinin aksine Login olan bir kullanýcýnýn eriþim izni olmayan bir sayfaya yönlendirmesini saðlayacak path
            });
            services.AddMvc(opt =>
            {
                opt.EnableEndpointRouting = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage(); //alýnan hatalarla ilgili açýklayýcý bilgiler sunar..
            app.UseStatusCodePages(); //özellikle herhangi bir content dönmeyen sayfalarda bilgilendirici yazýlar gösterir..
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute(); //en altta olmasý gerekir..

        }
    }
}
