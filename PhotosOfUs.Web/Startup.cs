using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Model.Models;
using Stripe;
using PhotosOfUs.Web.Extensions;
using Microsoft.AspNetCore.Http;
using PhotosOfUs.Web.Utilities;
using Newtonsoft.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PhotosOfUs.Model.Repositories;
using Kuvio.Kernel.Azure;
using PhotosOfUs.Web.Auth;

namespace PhotosOfUs.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("azurekeyvault.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            builder.AddAzureKeyVault(
                $"https://{config["azureKeyVault:vault"]}.vault.azure.net/",
                config["azureKeyVault:clientId"],
                config["azureKeyVault:clientSecret"]
            );

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddOpenIdConnect("B2CWeb", options =>
                {
                    options.ClientId = Configuration["AzureAdB2C:ClientId"];
                    options.Authority = $"https://login.microsoftonline.com/tfp/{Configuration["AzureAdB2C:Tenant"]}/{Configuration["AzureAdB2C:Policy"]}/v2.0/";
                    options.UseTokenLifetime = true;
                    options.SignInScheme = "B2C";
                    options.TokenValidationParameters = new TokenValidationParameters { NameClaimType = "name" };
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = OnRedirectToIdentityProviderAsync,
                        OnTokenValidated = OnTokenValidatedAsync
                    };
                })
            .AddCookie();

            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); ;

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
            });

            services.AddCors(o => o.AddPolicy("LoginPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials();
            }));

            services.AddDbContext<PhotosOfUsContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Database"]));
            services.AddScoped<StorageContext>(options => new StorageContext(Configuration["ConnectionStrings:Storage"]));
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.Configure<AzureAdB2COptions>(Configuration.GetSection("Authentication:AzureAdB2C"));

            services.AddScoped<IViewRenderService, ViewRenderService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseCors("LoginPolicy");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Homepage}/{id?}");
            });
        }

        private Task OnTokenValidatedAsync(Microsoft.AspNetCore.Authentication.OpenIdConnect.TokenValidatedContext context)
        {
            context.HttpContext.RequestServices.GetRequiredService<UserRepository>().Login(context.Principal);
            return Task.FromResult(0);
        }

        private Task OnRedirectToIdentityProviderAsync(RedirectContext context)
        {
            var defaultPolicy = Configuration["AzureAdB2C:Policy"];
            if (context.Properties.Items.TryGetValue("Policy", out var policy) && !policy.Equals(defaultPolicy))
            {
                context.ProtocolMessage.Scope = OpenIdConnectScope.OpenIdProfile;
                context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
                context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.ToLower().Replace(defaultPolicy.ToLower(), policy.ToLower());
                context.Properties.Items.Remove("Policy");
            }

            return Task.FromResult(0);
        }
    }
}
