using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Miusiquita.Helpers;
using Nancy;
using Miusiquita.Helpers;
namespace Miusiquita.Modules
{
	public class HomeModule : NancyModule
	{
		public HomeModule()
		{
			Get["/"] = _ => 
			{
				if (Context.IsLoggedIn())
					return Response.AsRedirect("/rooms/my");

				var m = Context.Model("Miusiquita");
				return View["/home/index.html", m];
			};
		}
	}
}