using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opt=> 
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
            app.UseMvcWithDefaultRoute(); 
        }
    }
}
