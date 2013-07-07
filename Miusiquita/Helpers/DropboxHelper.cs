using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DropNet;

namespace Miusiquita.Helpers
{
	public class DropboxCredentials
	{
		public string ApiKey { get; private set; }
		public string ApiSecret { get; private set; }
		public string UserToken { get; private set; }
		public string UserSecret { get; private set; }

		public DropboxCredentials(string apiKey, string apiSecret)
		{
			ApiKey = apiKey;
			ApiSecret = apiSecret;
		}
	}

	public class DropboxHelper
	{
		public DropNetClient Client { get; private set; }

		public DropboxHelper(DropboxCredentials credentials)
		{
			Client = new DropNetClient(credentials.ApiKey, credentials.ApiSecret);
		}


		public byte[] GetFile(string path)
		{
			return Client.GetFile(path);
		}

		internal IEnumerable<string> GetFiles(string path)
		{
			var metaData = Client.GetMetaData(path);
			return metaData.Contents.Select(x => x.Path);
		}
	}
}