using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using AureliaAspNetCoreAuth.Connections;
using AureliaAspNetCoreAuth.Providers;
using AureliaAspNetCoreAuth.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using AspNet.Security.OAuth.Validation;
using Microsoft.Framework.DependencyInjection;
//using Mvc.Server.Extensions;
//using Mvc.Server.Models;
//using Mvc.Server.Providers;

namespace AureliaAspNetCoreAuth
{

    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");

            builder.AddEnvironmentVariables();
            Configuration = builder.Build().ReloadOnChanged("appsettings.json");
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            services.AddAuthentication();

            services.AddSignalR();
            // Add framework services.
            //services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            // Add MVC services to the services container.
            services.AddMvc()
              .AddJsonOptions(opts =>
              {
                  opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
              });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            var configurationSection = Configuration.GetSection("AppSettings");
            //var title = configurationSection.Get<string>("ApplicationTitle");
            //var topItmes = configurationSection.Get<int>("TopItemsOnStart");
            //var showLink = configurationSection.Get<bool>("ShowEditLink");
            //var config = Configuration.GetSection("UriSettings");
            //string serverUri = config.UriSettings.ServerURi;
            //string clientUri = config.UriSettings.ClientUri;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add a new middleware validating access tokens.
            app.UseOAuthValidation(options =>
            {
                // Automatic authentication must be enabled
                // for SignalR to receive the access token.
                options.AutomaticAuthenticate = true;

                options.Events = new OAuthValidationEvents
                {
                    // Note: for SignalR connections, the default Authorization header does not work,
                    // because the WebSockets JS API doesn't allow setting custom parameters.
                    // To work around this limitation, the access token is retrieved from the query string.
                    OnRetrieveToken = context =>
                    {
                        // Note: when the token is missing from the query string,
                        // context.Token is null and the JWT bearer middleware will
                        // automatically try to retrieve it from the Authorization header.
                        context.Token = context.Request.Query["access_token"];

                        return Task.FromResult(0);
                    }
                };
            });

            app.UseSignalR<RawConnection>("/signalr");
            //app.UseSignalR<SimpleConnection>("/signalr");

            // Add a new middleware issuing access tokens.
            app.UseOpenIdConnectServer(options =>
            {
                options.Provider = new AuthenticationProvider();
                // Enable the authorization, logout, token and userinfo endpoints.
                //options.AuthorizationEndpointPath = "/connect/authorize";
                //options.LogoutEndpointPath = "/connect/logout";
                options.TokenEndpointPath = "/connect/token";
                //options.UserinfoEndpointPath = "/connect/userinfo";

                // Note: if you don't explicitly register a signing key, one is automatically generated and
                // persisted on the disk. If the key cannot be persisted, an exception is thrown.
                // 
                // On production, using a X.509 certificate stored in the machine store is recommended.
                // You can generate a self-signed certificate using Pluralsight's self-cert utility:
                // https://s3.amazonaws.com/pluralsight-free/keith-brown/samples/SelfCert.zip
                // 
                // options.SigningCredentials.AddCertificate("7D2A741FE34CC2C7369237A5F2078988E17A6A75");
                // 
                // Alternatively, you can also store the certificate as an embedded .pfx resource
                // directly in this assembly or in a file published alongside this project:
                // 
                // options.SigningCredentials.AddCertificate(
                //     assembly: typeof(Startup).GetTypeInfo().Assembly,
                //     resource: "Nancy.Server.Certificate.pfx",
                //     password: "Owin.Security.OpenIdConnect.Server");

                // Note: see AuthorizationController.cs for more
                // information concerning ApplicationCanDisplayErrors.
                options.ApplicationCanDisplayErrors = true;
                options.AllowInsecureHttp = true;
            });

            //app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
