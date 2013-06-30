using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Miusiquita.Helpers;
using Nancy;

namespace Miusiquita.Modules
{
	public class HomeModule : NancyModule
	{
		public HomeModule()
		{
			Get["/"] = _ => { return "Hello"; };
		}
	}
}