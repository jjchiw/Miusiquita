using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;
using Miusiquita.Helpers;
using Miusiquita.Models;
using Nancy.ModelBinding;
using Nancy;
using DropNet;
using System.Web.Configuration;
using DropNet.Models;

namespace Miusiquita.Modules
{
	public class RoomModuleSecure : RavenModule
	{

		DropboxHelper _dropboxHelper;
		
		public RoomModuleSecure(DropboxHelper dropboxHelper) : base("rooms")
		{
			this.RequiresAuthentication();

			_dropboxHelper = dropboxHelper;

			Get["/my"] = _ =>
			{
				var m = Context.Model("My rooms");
				m.MyRoomActive = "active";
				m.HaveDropboxConfiguration = ((Context.CurrentUser as User).DropboxApiUserToken != null);
				return View["room/my.html", m];
			};

			Get["/my-rooms"] = _ =>
			{
				int skip = (Request.Query.queries.HasValue ? Request.Query.skip : 0);
				int take = (Request.Query.queries.HasValue ? Request.Query.take : 10);

				var rooms = RavenSession.Query<Room>()
											.Where(x => x.User.Id == Context.UserRavenIdString())
											.Skip(skip)
											.Take(take)
											.ToList();

				return rooms;
			};

			Get["/dropbox-access"] = _ =>
			{
				var dropboxCallback = WebConfigurationManager.AppSettings["dropboxCallback"].ToString();
				var token = _dropboxHelper.Client.GetToken();
				var url = _dropboxHelper.Client.BuildAuthorizeUrl(dropboxCallback);

				Session["DropNetUserLoginSecret"] = token.Secret;
				Session["DropNetUserLoginToken"] = token.Token;
				
				return Response.AsRedirect(url);

			};

			Get["/dropbox-token"] = _ =>
			{
				_dropboxHelper.Client.UserLogin = new UserLogin { 
																	Secret = Session["DropNetUserLoginSecret"].ToString(), 
																	Token = Session["DropNetUserLoginToken"].ToString()
																};
				var accessToken = _dropboxHelper.Client.GetAccessToken();

				var user = RavenSession.Load<User>(Context.UserRavenIdString());
				user.DropboxApiUserToken = accessToken.Token;
				user.DropboxApiUserSecret = accessToken.Secret;

				RavenSession.Store(user);

				return Response.AsRedirect("/rooms/my");

			};

			Post["/"] = _ =>
			{
				var room = this.Bind<Room>();
				room.LastUpdated = DateTime.UtcNow;
				room.User = Context.CurrentUser as User;
				if (room.Id == null)
				{
					room.DateCreated = room.LastUpdated;
				}

				var user = Context.CurrentUser as User;
				_dropboxHelper.Client.UserLogin = new DropNet.Models.UserLogin { Token = user.DropboxApiUserToken, Secret = user.DropboxApiUserSecret };
				room.Songs = _dropboxHelper.GetFiles(room.DropboxPath).ToList();

				RavenSession.Store(room);

				return Response.AsJson<Room>(room, HttpStatusCode.Created);
			};

			Put["/"] = _ =>
			{
				this.RequiresAuthentication();

				var room = this.Bind<Room>();
				room.LastUpdated = DateTime.UtcNow;
				if (room.Id == null)
				{
					room.DateCreated = room.LastUpdated;
				}
				RavenSession.Store(room);

				return Response.AsJson<Room>(room);
			};

			Delete["/"] = _ =>
			{
				this.RequiresAuthentication();

				var room = this.Bind<Room>();
				room = RavenSession.Load<Room>(room.Id);

				if (room == null)
					return HttpStatusCode.NotFound;

				RavenSession.Delete<Room>(room);

				return Response.AsJson<Room>(room);
			};
		}
	}
}