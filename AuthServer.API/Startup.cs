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
            // NOT: Proje içeriisnde DI contenet ekleyp arkasýndan initialize etme iþlemini oradada gerçekleþtirebiliriz...
            // karmaþýk halke gelmemesi için startup tarafýna eklenip düzelmektedir.

            // DI register
            // tek bir istekte bir tane nesne örneði oluþacak, ayný istekte
            // birden fazla interface ile karþýlaþýrsa, ayný nesne örneðini kullanacak: AddScoped..... Uygun olan AddScoped dir
            // kendi projemizdeki core katmanýmýzdaki interface implemente edilmelidir.
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
            // aþaðýda Configurationdaki TokenOptions'u okuduk sonra onu customTokenOptions'a mapledik ve bize buradan get den cutomTokenOptions gelmektedir.
            var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();

            // aþaðýdaki yöntem ile Client'e herhangi bir constructor dan eriþebilirim. Bu isme de OpptionPatterns denir.
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));


            //----------------------------------//---------------------------------//------------------------------//
            // þema vermemiz gerekmektedir // Þema : Bir üyelik sisteminde  iki farklý üyeklik sistemi olabilir. Mesela bayiler için ayrý üyelik 
            // müþteriler için farklý bir login olabilir. Bu aþamada þema olarak adlandýrýp düzenlemekteyiz.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // Authentication þema da bir  api olduðundan dolayý jwt bazlý bir doðrulama yapacaðýz.
                // Yani requestimize headerýmýza tokenID arayacak. 
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => {
                // þemalarýn ismi ayný fakat birbirlerini bilmemektedir. Bu þemalarý birbirine tanýmlamak için aþaðýdaki gibi tanýmlama yaparýz.
                // bir token ggelidði zaman issues'ini kontrol etmek için aþaðýdaki belirleme yapýlýr.
                // Validation parametlerini belirlemek için öncelikle TokenValidationParameters'i belirlememmiz gerekmektedir.
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters() {
                    // validateIssuer'ler appsetting.json klasöründedir.
                    ValidIssuer = tokenOptions.Issuer,
                    // birden fazla audience verebiliriz.
                    ValidAudience = tokenOptions.Audience[0],
                    //Issue Ýmzasýný doðrulama
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
                    // bu token ömrü için validatelifetime true set edilir.

                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    // Aþaðýdaki deðer Default olarak 5 dakikalýk bir pay iþler.  Bu default 5 dakika Kurduðumuz api'yi farklý serverlara kurabiliriz//
                    //bu debeple token'ý 5 dakika beklemek yerine  bunu 0'a çekebiliriz
                    // Doðrulama iþlemi aþaðýdaki ayarlara göre olacaktýr.

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
