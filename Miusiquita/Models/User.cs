using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Miusiquita.Models.Helpers;
using Nancy.Security;

namespace Miusiquita.Models
{
	public class User : IUserIdentity, IUserDenormalized
	{
		public string Id { get; set; }
		public Guid Guid { get; set; }
		public string Email { get; set; }

		public string Token { get; set; }

		public string DropboxApiUserToken { get; set; }
		public string DropboxApiUserSecret { get; set; }


		#region IUserIdentity Members

		public string UserName { get; set; }
		public IEnumerable<string> Claims { get; set; }

		public int StrikeCount { get; set; }

		#endregion

	}
}