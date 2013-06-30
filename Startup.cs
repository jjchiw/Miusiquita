using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;

namespace Miusiquita
{
	public class Startup
	{
		// The name *MUST* be Configuration
		public void Configuration(IAppBuilder app)
		{
			app.MapHubs();
		}
	}
}