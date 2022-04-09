using AuthServer.Core.Configuration;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // NOT: Proje içeriisnde DI contenet ekleyp arkasýndan initialize etme iþlemini oradada gerçekleþtirebiliriz...
            // karmaþýk halke gelmemesi için startup tarafýna eklenip düzelmektedir.

            // DI register
            // tek bir istekte bir tane nesne örneði oluþacak, ayný istekte
            // birden fazla interface ile karþýlaþýrsa, ayný nesne örneðini kullanacak: AddScoped
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();

            //generix eklemeleri farklý þekilde tanýmlanmaktadýr.
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //  aþaðýda <,> yazýlmasýnýn amacý iki tane  deðer aldýðý için bir tane virgül yerleþtirdik.
            //  Ancak 3 tane deðer alsaydý 2 tane virgül koymamýz gerekirdi
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));
            services.AddDbContext<AppDbContext>(options =>
            {
                // GetConntectionString: Bizim ConnectionString modumuzu almaktadýr.
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"), sqlOptions =>
                 {
                     //migrations nerede olacaðýný belirtmek için migrations assambly'si aþaðýdaki tanýmadðýmýz yerde olacaktýr
                     sqlOptions.MigrationsAssembly("AuthServer.Data");
                 });

            });

            services.AddIdentity<UserApp, IdentityRole>(Opt =>
            {
                // email veritabanýnda unique olmasýný saðlamak için aþaðýdaki gibi true olmasýný saðlamaktadýr
                Opt.User.RequireUniqueEmail = true;
                // alfanumeric karakter zorunluluðu *?=- hayýr diye belirtiriz. Defaul'u true olarak belirtilmiþtir.
                Opt.Password.RequireNonAlphanumeric = false;
                // þifre üretmek için bir token üretmek gerekmektedir. Bunun içinde AddDefaultTokenProviders tanýmlamamýz gerekmektedir.
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


            // Bu olaya ooptionsPattern denmektedir.
            // aþaðýdaki konfigürasyon ile birlikte genel olarak projede istedeðimiz yerde geçebiliriz"CustomTokenOption"
            services.Configure<CustomTokenOptions>(Configuration.GetSection("TokenOption"));

            // aþaðýdaki yöntem ile Client'e herhangi bir constructor dan eriþebilirim. Bu isme de OpptionPatterns denir.
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthServer.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthServer.API v1"));
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
