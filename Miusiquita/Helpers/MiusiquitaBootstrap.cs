using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Routing;
using Miusiquita.Helpers;
using Nancy;
using Nancy.Conventions;
using Nancy.TinyIoc;


namespace Miusiquita.Helpers
{
	public class MiusiquitaBootstrap : DefaultNancyBootstrapper
	{
		protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			base.ApplicationStartup(container, pipelines);

			//RouteTable.Routes.MapHubs();
		}

		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			base.ConfigureRequestContainer(container, context);

			var apiKey = WebConfigurationManager.AppSettings["dropboxApiKey"].ToString();
			var apiSecret = WebConfigurationManager.AppSettings["dropboxApiSecret"].ToString();
			var userToken = WebConfigurationManager.AppSettings["dropboxApiUserToken"].ToString();
			var userSecret = WebConfigurationManager.AppSettings["dropboxApiUserSecret"].ToString();

			var dropboxCredentials = new DropboxCredentials(apiKey, apiSecret, userToken, userSecret);

			container.Register<DropboxCredentials>(dropboxCredentials);
		}

		protected override void ConfigureConventions(NancyConventions conventions)
		{
			base.ConfigureConventions(conventions);

			conventions.StaticContentsConventions.Add(
				StaticContentConventionBuilder.AddDirectory("js", @"Content/js")
			);
		}
	}
}