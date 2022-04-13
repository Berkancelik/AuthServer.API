using AuthServer.Core.Configuration;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            // birden fazla interface ile kar��la��rsa, ayn� nesne �rne�ini kullanacak: AddScoped..... Uygun olan AddScoped dir
            // kendi projemizdeki core katman�m�zdaki interface implemente edilmelidir.
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
            // a�a��da Configurationdaki TokenOptions'u okuduk sonra onu customTokenOptions'a mapledik ve bize buradan get den cutomTokenOptions gelmektedir.
            var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();

            // a�a��daki y�ntem ile Client'e herhangi bir constructor dan eri�ebilirim. Bu isme de OpptionPatterns denir.
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));


            //----------------------------------//---------------------------------//------------------------------//
            // �ema vermemiz gerekmektedir // �ema : Bir �yelik sisteminde  iki farkl� �yeklik sistemi olabilir. Mesela bayiler i�in ayr� �yelik 
            // m��teriler i�in farkl� bir login olabilir. Bu a�amada �ema olarak adland�r�p d�zenlemekteyiz.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // Authentication �ema da bir  api oldu�undan dolay� jwt bazl� bir do�rulama yapaca��z.
                // Yani requestimize header�m�za tokenID arayacak. 
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => {
                // �emalar�n ismi ayn� fakat birbirlerini bilmemektedir. Bu �emalar� birbirine tan�mlamak i�in a�a��daki gibi tan�mlama yapar�z.
                // bir token ggelid�i zaman issues'ini kontrol etmek i�in a�a��daki belirleme yap�l�r.
                // Validation parametlerini belirlemek i�in �ncelikle TokenValidationParameters'i belirlememmiz gerekmektedir.
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters() {
                    // validateIssuer'ler appsetting.json klas�r�ndedir.
                    ValidIssuer = tokenOptions.Issuer,
                    // birden fazla audience verebiliriz.
                    ValidAudience = tokenOptions.Audience[0],
                    //Issue �mzas�n� do�rulama
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
                    // bu token �mr� i�in validatelifetime true set edilir.

                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    // A�a��daki de�er Default olarak 5 dakikal�k bir pay i�ler.  Bu default 5 dakika Kurdu�umuz api'yi farkl� serverlara kurabiliriz//
                    //bu debeple token'� 5 dakika beklemek yerine  bunu 0'a �ekebiliriz
                    // Do�rulama i�lemi a�a��daki ayarlara g�re olacakt�r.

                    ClockSkew =TimeSpan.Zero
                    

                };

            
            });








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
