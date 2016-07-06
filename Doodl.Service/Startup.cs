//-----------------------------------------------------------------------
// <copyright file="Startup.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Model;

    /// <summary>
    /// Provides the ASP.NET Core entry point configuration methods.
    /// </summary>
    public class Startup
    {
        private IConfigurationRoot configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The hosting environment to use.</param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            this.configuration = builder.Build();
        }

        /// <summary>
        /// Configures services required for the application.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRouting();

            var azureStorage = this.configuration.GetSection("AzureStorage");

            services.Configure<AzureDoodlStorageOptions>(azureStorage.GetSection("DoodlStorage"));

            services.Configure<List<string>>(prinnies =>
            {
                prinnies.AddRange(this.configuration.GetSection("Prinny").GetChildren().Select(config => config.Value));
            });

            services.AddSingleton<IDoodlStorage, AzureDoodlStorage>();
            services.AddSingleton<IDoodlRenderer, ViewEngineDoodlRenderer>();
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The application builder to use.</param>
        /// <param name="env">The hosting environment to use.</param>
        /// <param name="loggerFactory">The logger factory to use.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(this.configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}");

                routes.MapRoute(
                    name: "get doodl",
                    template: "doodl/{id:guid}",
                    defaults: new { controller = "doodl", action = "GetDoodl" });
            });
        }
    }
}
