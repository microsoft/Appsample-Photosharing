//-----------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Mobile.Server.Config;
using Owin;
using PhotoSharingApp.AppService.Helpers;
using PhotoSharingApp.AppService.Notifications;
using PhotoSharingApp.AppService.Shared.Caching;
using PhotoSharingApp.AppService.Shared.Context;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.AppService.Shared.Validation;

namespace PhotoSharingApp.AppService
{
    /// <summary>
    /// Class to configure Azure App Service.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configures the webApi dependency injection to start the service.
        /// </summary>
        /// <param name="app">The Azure app service mobile app to configure.</param>
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Services.Add(typeof(IExceptionLogger), new ApplicationInsightsExceptionLogger());

            IContainer container = ConfigureWebApiDependencies();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.MapHttpAttributeRoutes();

            new MobileAppConfiguration()
                .AddMobileAppHomeController()
                .MapApiControllers()
                .ApplyTo(config);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);

            // Configure Application Insights Telemetry.
            TelemetryConfiguration.Active.InstrumentationKey = SystemContext.Current.Environment.InstrumentationKey;

            var rootPath = HttpContext.Current.Server.MapPath("~");
            var serverPath = Path.Combine(rootPath, "bin");

            using (var scope = container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<IRepository>();

                // Do database initialization synchronously
                repository.InitializeDatabaseIfNotExisting(serverPath).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        private static IContainer ConfigureWebApiDependencies()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterType<TelemetryClient>().As<TelemetryClient>();
            containerBuilder.RegisterType<MemoryCacheService>().As<ICacheService>();
            containerBuilder.RegisterType<CachedRepository>()
                .As<IRepository>()
                .WithParameter(new TypedParameter(typeof(IRepository),
                    new DocumentDbRepository(SystemContext.Current.Environment)));
            containerBuilder.RegisterType<IapValidator>().As<IIapValidator>();
            containerBuilder.RegisterType<PhotoValidation>().As<IPhotoValidation>();
            containerBuilder.RegisterType<NotificationHandler>().As<INotificationHandler>();
            containerBuilder.RegisterType<DefaultUserRegistrationReferenceProvider>().As<IUserRegistrationReferenceProvider>();
            containerBuilder.Register(c => SystemContext.Current.Environment);

            IContainer container = containerBuilder.Build();

            return container;
        }
    }
}