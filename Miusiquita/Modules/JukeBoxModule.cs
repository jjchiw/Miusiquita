using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Ionic.Zip;
using Miusiquita.Helpers;
using Nancy;

namespace Miusiquita.Modules
{
	public class JukeBoxModule : NancyModule
	{
		public JukeBoxModule(DropboxHelper dropboxHelper) : base("jukebox")
		{
			Get["/play/{id}"] = _ => 
			{
				return View["jukebox/play"]; 
			};

			Get["/download/{id}"] = _ =>
			{
				//Buscamos los urls de miusiquita
				//var filesName = new List<string>
				//{
				//	"Miusiquita/01 No es en serio.mp3",
				//	"Miusiquita/02 Bla bla bla.mp3",
				//};
				//var files = new List<Tuple<string, byte[]>>();

				//foreach (var item in filesName)
				//{
				//	var bytes = dropboxHelper.GetFile(item);
				//	files.Add(new Tuple<string, byte[]>(Path.GetFileName(item), bytes));
				//}

				//using (var zipStream = new MemoryStream())
				//{
				//	using (ZipFile zip = new ZipFile())
				//	{
				//		foreach (var item in files)
				//		{
				//			zip.AddEntry(item.Item1, item.Item2);
				//		}

				//		zip.Save(zipStream);

				//		var responseStream = new MemoryStream(zipStream.ToArray());
				//		return Response.FromStream(responseStream, "application/zip").AsAttachment("play.zip");
				//	}
				//}

				var f = File.ReadAllBytes(@"C:\tmp\a.zip");

				var responseStream = new MemoryStream(f);
				return Response.FromStream(responseStream, "application/zip").AsAttachment("play.zip");

			};
		}
	}
}