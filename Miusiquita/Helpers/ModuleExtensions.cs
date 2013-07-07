using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Miusiquita.Models;
using Nancy;

namespace Miusiquita.Helpers
{
	public static class ModuleExtensions
	{
		public static bool IsLoggedIn(this NancyContext context)
		{
			return !(context == null || context.CurrentUser == null ||
					 string.IsNullOrWhiteSpace(context.CurrentUser.UserName));
		}

		public static string Username(this NancyContext context)
		{
			return (context == null || context.CurrentUser == null || string.IsNullOrWhiteSpace(context.CurrentUser.UserName)) ? string.Empty : context.CurrentUser.UserName;
		}

		public static string UserEmail(this NancyContext context)
		{
			return (context == null || context.CurrentUser == null) ? string.Empty : (context.CurrentUser as User).Email;
		}

		public static string UserRavenIdString(this NancyContext context)
		{
			return (context == null || context.CurrentUser == null) ? string.Empty : (context.CurrentUser as User).Id;
		}

		public static string UserGravatarUrl(this NancyContext context)
		{
			return (context == null || context.CurrentUser == null) ? string.Empty : (context.CurrentUser as User).Email.ToGravatarUrl();
		}

		public static dynamic Model(this NancyContext context, string title)
		{
			dynamic model = new ExpandoObject();
			model.Title = title;
			model.IsLoggedIn = context.IsLoggedIn();
			model.UserName = context.Username();
			model.Email = context.UserEmail();
			model.GravatarUrl = context.UserGravatarUrl();
			model.RoomActive = "";
			model.MyRoomActive = "";
			model.AboutActive = "";
			return model;
		}
	}
}