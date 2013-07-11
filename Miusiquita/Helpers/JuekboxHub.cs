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
		public static Dictionary<string, List<string>> MapRoomId = new Dictionary<string, List<string>>();

		public override Task OnConnected()
		{
			return base.OnConnected();
		}

		public override Task OnDisconnected()
		{
			foreach (var item in MapRoomId)
			{
				item.Value.Remove(Context.ConnectionId);
			}

			return base.OnDisconnected();
		}

		public void Join(string roomId)
		{
			Groups.Add(Context.ConnectionId, roomId);
			if (!MapRoomId.ContainsKey(roomId))
				MapRoomId.Add(roomId, new List<string>());

			MapRoomId[roomId].Add(Context.ConnectionId);

			Clients.Group(roomId).UpdateClientsCount(MapRoomId[roomId].Count());
		}

		public void Leave(string roomId)
		{
			Groups.Remove(Context.ConnectionId, roomId);
			var result = 0;
			if (MapRoomId.ContainsKey(roomId))
			{
				MapRoomId[roomId].Remove(Context.ConnectionId);
				result = MapRoomId[roomId].Count;
			}
			
			Clients.Group(roomId).UpdateClientsCount(result);
		}

		public void Start(string roomId)
		{
			var serverDateTime = DateTime.UtcNow;
			var playAtDateTime = serverDateTime.AddSeconds(45);
		
			Clients.Group(roomId).Play(serverDateTime, playAtDateTime);
		}

		public void Stop(string roomId)
		{
			// Call the addMessage method on all clients            
			Clients.Group(roomId).Stop(DateTime.UtcNow.AddSeconds(30));
		}
	}
}