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
            // NOT: Proje i�eriisnde DI contenet ekleyp arkas�ndan initialize etme i�lemini oradada ger�ekle�tirebiliriz...
            // karma��k halke gelmemesi i�in startup taraf�na eklenip d�zelmektedir.

            // DI register
            // tek bir istekte bir tane nesne �rne�i olu�acak, ayn� istekte
            // birden fazla interface ile kar��la��rsa, ayn� nesne �rne�ini kullanacak: AddScoped
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();

            //generix eklemeleri farkl� �ekilde tan�mlanmaktad�r.
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //  a�a��da <,> yaz�lmas�n�n amac� iki tane  de�er ald��� i�in bir tane virg�l yerle�tirdik.
            //  Ancak 3 tane de�er alsayd� 2 tane virg�l koymam�z gerekirdi
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));
            services.AddDbContext<AppDbContext>(options =>
            {
                // GetConntectionString: Bizim ConnectionString modumuzu almaktad�r.
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"), sqlOptions =>
                 {
                     //migrations nerede olaca��n� belirtmek i�in migrations assambly'si a�a��daki tan�mad��m�z yerde olacakt�r
                     sqlOptions.MigrationsAssembly("AuthServer.Data");
                 });

            });

            services.AddIdentity<UserApp, IdentityRole>(Opt =>
            {
                // email veritaban�nda unique olmas�n� sa�lamak i�in a�a��daki gibi true olmas�n� sa�lamaktad�r
                Opt.User.RequireUniqueEmail = true;
                // alfanumeric karakter zorunlulu�u *?=- hay�r diye belirtiriz. Defaul'u true olarak belirtilmi�tir.
                Opt.Password.RequireNonAlphanumeric = false;
                // �ifre �retmek i�in bir token �retmek gerekmektedir. Bunun i�inde AddDefaultTokenProviders tan�mlamam�z gerekmektedir.
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


            // Bu olaya ooptionsPattern denmektedir.
            // a�a��daki konfig�rasyon ile birlikte genel olarak projede istede�imiz yerde ge�ebiliriz"CustomTokenOption"
            services.Configure<CustomTokenOptions>(Configuration.GetSection("TokenOption"));

            // a�a��daki y�ntem ile Client'e herhangi bir constructor dan eri�ebilirim. Bu isme de OpptionPatterns denir.
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
