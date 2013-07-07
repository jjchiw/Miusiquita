using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Miusiquita.Models.Helpers;

namespace Miusiquita.Models
{
	public class Room
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string DropboxPath { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime LastUpdated { get; set; }
		public UserDenormalized<User> User { get; set; }
		public List<string> Songs { get; set; }
	}
}