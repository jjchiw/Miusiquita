using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miusiquita.Models.Helpers
{
	public interface IUserDenormalized
	{
		string Id { get; set; }
		string UserName { get; set; }
		string Email { get; set; }
	}
}
