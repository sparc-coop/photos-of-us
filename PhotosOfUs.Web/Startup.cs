using System;
using System.Threading.Tasks;
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
using Kuvio.Kernel.Auth;
using Kuvio.Kernel.Architecture;
using PhotosOfUs.Model;
using AutoMapper;
using PhotosOfUs.Connectors.Storage;
using PhotosOfUs.Connectors.Database;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Claims;
using PhotosOfUs.Model.Photos.Commands;
using Microsoft.AspNetCore.Authentication;

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
            services.Configure<AzureAdB2COptions>(Configuration.GetSection("Authentication:AzureAdB2C"));

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            //.AddAzureAdB2C(options => Configuration.Bind("Authentication:AzureAdB2C", options))
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    // TODO: Refactor into Kuvio Kernel
                    options.ClientId = Configuration["Authentication:AzureAdB2C:ClientId"];
                    options.Authority = $"https://login.microsoftonline.com/tfp/{Configuration["Authentication:AzureAdB2C:Tenant"]}/{Configuration["Authentication:AzureAdB2C:SignUpSignInPolicyIdPhotographer"]}/v2.0/";
                    options.UseTokenLifetime = true;
                    options.SignInScheme = "B2C";
                    options.TokenValidationParameters = new TokenValidationParameters { NameClaimType = "name" };
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = OnRedirectToIdentityProviderAsync,
                        OnTokenValidated = OnTokenValidatedAsync,
                        OnRemoteFailure = OnRemoteFailure
                    };
                })
            .AddCookie("B2C");

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("B2C")
                .Build();
            });


            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

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

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());

            services.AddDbContext<PhotosOfUsContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Database"]));
            services.AddScoped<DbContext, PhotosOfUsContext>();
            services.AddScoped<StorageContext>(options => new StorageContext(Configuration["ConnectionStrings:Storage"]));
            services.AddTransient(typeof(IRepository<>), typeof(DbRepository<>));
            services.AddTransient(typeof(IMediaRepository<>), typeof(MediaRepository<>));

            services.AddScoped<LoginCommand>();
            services.AddScoped<UploadPhotoCommand>();
            services.AddScoped<UploadProfileImageCommand>();
            services.AddScoped<UserProfileUpdateCommand>();

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

            services.Configure<RazorViewEngineOptions>(o =>
            {
                // {2} is area, {1} is controller,{0} is the action    
                o.ViewLocationFormats.Clear();
                //o.ViewLocationFormats.Add("/{2}/Views/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("~/{1}/Views/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("/Home/Views/Shared/{0}" + RazorViewEngine.ViewExtension);

                o.AreaViewLocationFormats.Clear();
                o.AreaViewLocationFormats.Add("/{2}/Views/{0}" + RazorViewEngine.ViewExtension);

            });

            services.AddScoped<IViewRenderService, ViewRenderService>();

            // For injecting the user data into repositories
            // Hack: Testing. I'm not sure if this is working
            services.AddTransient<ClaimsPrincipal>(s => s.GetService<IHttpContextAccessor>()?.HttpContext?.User);
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

            //app.UseHttpsRedirection();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapAreaRoute("usersRoute", "Users", "{controller}/{action=Index}/{id?}");
                routes.MapAreaRoute("ordersRoute", "Orders", "{controller}/{action=Index}/{id?}");
                //routes.MapRoute("photRoute", "{controller=Photographer}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private Task OnTokenValidatedAsync(TokenValidatedContext context)
        {
            context.HttpContext.RequestServices.GetRequiredService<LoginCommand>().Execute(context.Principal);
            //context.HttpContext.RequestServices.GetService<LoginCommand>().Execute(context.Principal);
            return Task.FromResult(0);
        }

        private Task OnRedirectToIdentityProviderAsync(RedirectContext context)
        {
            var defaultPolicy = Configuration["Authentication:AzureAdB2C:SignUpSignInPolicyIdPhotographer"];
            if (context.Properties.Items.TryGetValue("Policy", out var policy) && !policy.Equals(defaultPolicy))
            {
                context.ProtocolMessage.Scope = OpenIdConnectScope.OpenIdProfile;
                context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
                context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.ToLower().Replace(defaultPolicy.ToLower(), policy.ToLower());
                context.Properties.Items.Remove("Policy");
            }

            return Task.FromResult(0);
        }

        public Task OnRemoteFailure(RemoteFailureContext context)
        {
            context.HandleResponse();
            // Handle the error code that Azure AD B2C throws when trying to reset a password from the login page
            // because password reset is not supported by a "sign-up or sign-in policy"
            if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("AADB2C90118"))
            {
                // If the user clicked the reset password link, redirect to the reset password route
                context.Response.Redirect("/Session/ResetPassword");
            }
            else if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied"))
            {
                context.Response.Redirect("/");
            }
            else
            {
                context.Response.Redirect("/Home/Error?message=" + context.Failure.Message);
            }
            return Task.FromResult(0);
        }
    }
}
