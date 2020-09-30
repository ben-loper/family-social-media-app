using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilySocialMediaApp.DAO;
using FamilySocialMediaApp.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FamilySocialMediaApp
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
            services.AddControllers();

            // Add CORS policy allowing any origin
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            // Enables automatic authentication token.
            // The token is expected to be included as a bearer authentication token.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // The rules for token validation
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,                 // issuer not required
                        ValidateAudience = false,               // audience not required
                        ValidateLifetime = true,                // token must not have expired
                        ValidateIssuerSigningKey = true,        // token signature must match so as not to be tampered with
                        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,   // allows us to use sub for username
                        RoleClaimType = "rol",                  // allows us to put the role in rol
                        IssuerSigningKey = new SymmetricSecurityKey(    // each token is signed with a private key so as to ensure its validity
                            Encoding.UTF8.GetBytes(Configuration["JwtSecret"]))
                    };
                });

            // Dependency Injection configuration
            services.AddSingleton<ITokenGenerator>(tk => new JwtGenerator(Configuration["JwtSecret"]));
            services.AddSingleton<IPasswordHasher>(ph => new PasswordHasher());
            services.AddTransient<IUserDAO>(m => new UserDAO(Configuration.GetConnectionString("Default")));
            //services.AddTransient<ISnippetDAO>(m => new SnippetSqlDAO(Configuration.GetConnectionString("Default")));
            //services.AddTransient<IPortfolioDAO>(m => new PortfolioSqlDAO(Configuration.GetConnectionString("Default")));
            //services.AddTransient<ITransactionDAO>(m => new TransactionDAO(Configuration.GetConnectionString("Default")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();         

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors("CorsPolicy");

            // Enables the middleware to check the incoming request headers.
            app.UseAuthentication();
           
        }
    }
}
