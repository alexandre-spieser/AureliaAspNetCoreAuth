using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.AspNet.Authentication.JwtBearer;
using AureliaAspNetCoreAuth.Connections;
using AureliaAspNetCoreAuth.Providers;
using AureliaAspNetCoreAuth.Models;

namespace AureliaAspNetCoreAuth
{

    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            builder.AddEnvironmentVariables();
            Configuration = builder.Build().ReloadOnChanged("appsettings.json");
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication();
            services.AddCaching();
            services.AddSignalR();
            // Add framework services.
            services.Configure<AppSettings>(Configuration);

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
            var config = Configuration.Get<AppSettings>();
            string serverUri = config.UriSettings.ServerURi;
            string clientUri = config.UriSettings.ClientUri;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseIISPlatformHandler();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add a new middleware validating access tokens.
            app.UseJwtBearerAuthentication(options => {
                // Automatic authentication must be enabled
                // for SignalR to receive the access token.
                options.AutomaticAuthenticate = true;

                // Automatically disable the HTTPS requirement for development scenarios.
                options.RequireHttpsMetadata = !env.IsDevelopment();

                // Note: the audience must correspond to the address of the SignalR server.
                options.Audience = clientUri;

                // Note: the authority must match the address of the identity server.
                options.Authority = serverUri;

                options.Events = new JwtBearerEvents
                {
                    // Note: for SignalR connections, the default Authorization header does not work,
                    // because the WebSockets JS API doesn't allow setting custom parameters.
                    // To work around this limitation, the access token is retrieved from the query string.
                    OnReceivingToken = context => {
                        // Note: when the token is missing from the query string,
                        // context.Token is null and the JWT bearer middleware will
                        // automatically try to retrieve it from the Authorization header.
                        context.Token = context.Request.Query["access_token"];

                        return Task.FromResult(0);
                    }
                };
            });

            app.UseWebSockets();

            app.UseSignalR<RawConnection>("/signalr");
            //app.UseSignalR<SimpleConnection>("/signalr");

            // Add a new middleware issuing access tokens.
            app.UseOpenIdConnectServer(options => {
                options.Provider = new AuthenticationProvider();
            });

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
