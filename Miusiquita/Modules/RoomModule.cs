using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Ionic.Zip;
using Miusiquita.Helpers;
using Nancy;
using Raven.Client;
using Nancy.ModelBinding;
using Miusiquita.Models;
using Nancy.Security;

namespace Miusiquita.Modules
{
	public class RoomModule : RavenModule
	{
		DropboxHelper _dropboxHelper;

		public RoomModule(DropboxHelper dropboxHelper) : base("rooms")
		{
			_dropboxHelper = dropboxHelper;

			Get["/play/{id}"] = _ => 
			{
				return View["jukebox/play"]; 
			};

			Get["/download/{id}"] = p =>
		
			{
				if (!p.id.HasValue)
					return HttpStatusCode.NotFound;

				var roomId = string.Format("{0}/{1}", this.ModulePath, p.id);
				Room room = RavenSession.Load<Room>(roomId);
				var user = RavenSession.Load<User>(room.User.Id);

				_dropboxHelper.Client.UserLogin = new DropNet.Models.UserLogin { Secret = user.DropboxApiUserSecret, Token = user.DropboxApiUserToken };

				var filesName = _dropboxHelper.GetFiles(room.DropboxPath);

				var files = new List<Tuple<string, byte[]>>();

				foreach (var item in filesName)
				{
					var bytes = _dropboxHelper.GetFile(item);
					files.Add(new Tuple<string, byte[]>(Path.GetFileName(item), bytes));
				}

				using (var zipStream = new MemoryStream())
				{
					using (ZipFile zip = new ZipFile())
					{
						foreach (var item in files)
						{
							zip.AddEntry(item.Item1, item.Item2);
						}

						zip.Save(zipStream);

						var responseStream = new MemoryStream(zipStream.ToArray());
						return Response.FromStream(responseStream, "application/zip").AsAttachment("play.zip");
					}
				}
			};

			
			Get["/all"] = _ =>
			{
				var m = Context.Model("Rooms");
				m.RoomActive = "active";

				return View["room/all.html", m];
			};

			Get["/all-rooms"] = _ =>
			{
				int skip = (Request.Query.queries.HasValue ? Request.Query.skip : 0);
				int take = (Request.Query.queries.HasValue ? Request.Query.take : 10);

				var rooms = RavenSession.Query<Room>()
											.Skip(skip)
											.Take(take)
											.ToList();

				return rooms;
			};

			Get["/{id}"] = p =>
			{
				if(!p.id.HasValue)
					return HttpStatusCode.NotFound;

				//var filesName = dropboxHelper.GetFiles(room.DropboxPath);
				
				var roomId = string.Format("{0}/{1}", this.ModulePath, p.id);
				Room room = RavenSession.Load<Room>(roomId);

				if (room == null)
					return HttpStatusCode.NotFound;

				if (Context.CurrentUser != null)
				{
					var user = Context.CurrentUser as User;
					_dropboxHelper.Client.UserLogin = new DropNet.Models.UserLogin { Token = user.DropboxApiUserToken, Secret = user.DropboxApiUserSecret };
				}

				var m = Context.Model("Room " + room.Name);
				m.MyRoomActive = "active";
				m.RoomId = room.Id;
				m.RoomName = room.Name;
				m.Files = (dropboxHelper.Client.UserLogin != null ? dropboxHelper.GetFiles(room.DropboxPath).ToList() : room.Songs);
				m.IsOwner = room.User.Id == Context.UserRavenIdString();

				return View["room/view.html", m];
			};

			Get["/modified-date/{id}"] = p =>
			{
				if (!p.id.HasValue)
					return HttpStatusCode.NotFound;

				var roomId = string.Format("{0}/{1}", this.ModulePath, p.id);
				Room room = RavenSession.Load<Room>(roomId);
				var user = RavenSession.Load<User>(room.User.Id);

				_dropboxHelper.Client.UserLogin = new DropNet.Models.UserLogin { Secret = user.DropboxApiUserSecret, Token = user.DropboxApiUserToken };
				var metaData = _dropboxHelper.Client.GetMetaData(room.DropboxPath);

				return metaData.ModifiedDate;
			};
		}
	}
}