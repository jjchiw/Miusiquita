using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Miusiquita.Helpers
{
	public class JukeboxHub : Hub
	{
		public void Start()
		{
			// Call the addMessage method on all clients            
			Clients.All.Play(DateTime.UtcNow.AddSeconds(30));
		}
	}
}