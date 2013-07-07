using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Routing;
using Miusiquita.Helpers;
using Miusiquita.Models;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Session;
using Nancy.TinyIoc;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using FormsAuthenticationConfiguration = Nancy.Authentication.Forms.FormsAuthenticationConfiguration;


namespace Miusiquita.Helpers
{
	public class MiusiquitaBootstrap : DefaultNancyBootstrapper
	{
		protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
		{
			// At request startup we modify the request pipelines to
			// include forms authentication - passing in our now request
			// scoped user name mapper.
			//
			// The pipelines passed in here are specific to this request,
			// so we can add/remove/update items in them as we please.
			var formsAuthConfiguration =
				new FormsAuthenticationConfiguration()
				{
					RedirectUrl = "/",
					UserMapper = requestContainer.Resolve<IUserMapper>(),
				};
			FormsAuthentication.Enable(pipelines, formsAuthConfiguration);

			
		}

		protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			base.ApplicationStartup(container, pipelines);

			CookieBasedSessions.Enable(pipelines);
		}

		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			base.ConfigureRequestContainer(container, context);

			var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionStringName("RavenDB");
			parser.Parse();

			var documentStore = new DocumentStore
			{
				ApiKey = parser.ConnectionStringOptions.ApiKey,
				Url = parser.ConnectionStringOptions.Url
			};

			documentStore.Initialize();

			var apiKey = WebConfigurationManager.AppSettings["dropboxApiKey"].ToString();
			var apiSecret = WebConfigurationManager.AppSettings["dropboxApiSecret"].ToString();

			var dropboxCredentials = new DropboxCredentials(apiKey, apiSecret);

			container.Register<DropboxCredentials>(dropboxCredentials);

			container.Register<IDocumentStore>(documentStore);
			container.Register<IUserMapper, UserMapper>();

			context.Items["RavenDocumentStore"] = documentStore;
		}

		protected override void ConfigureConventions(NancyConventions conventions)
		{
			base.ConfigureConventions(conventions);

			conventions.StaticContentsConventions.Add(
				StaticContentConventionBuilder.AddDirectory("js", @"Content/js")
			);

			conventions.StaticContentsConventions.Add(
				StaticContentConventionBuilder.AddDirectory("css", @"Content/css")
			);
		}
	}
}