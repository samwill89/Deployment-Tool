using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace SharpUpdate
{
	/// <summary>
	/// Contains update information
	/// </summary>
	public class SharpUpdateXml
	{
		/// <summary>
		/// The update version #
		/// </summary>
		/// 
		public string Program { get; }

		public Version Version { get; }

		/// <summary>
		/// The location of the update binary
		/// </summary>
		public Uri Uri { get; }

		/// <summary>
		/// The file path of the binary
		/// for use on local computer
		/// </summary>
		public string FilePath { get; }

		/// <summary>
		/// The MD5 of the update's binary
		/// </summary>
		public string MD5 { get; }

		/// <summary>
		/// The update's description
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// The arguments to pass to the updated application on startup
		/// </summary>
		public string LaunchArgs { get; }

		/// <summary>
		/// Tag to distinguish types of updates
		/// </summary>
		public JobType Tag;

		/// <summary>
		/// Creates a new SharpUpdateXml object
		/// </summary>
		public SharpUpdateXml(Version version, Uri uri, string filePath, string md5, string description, string launchArgs, JobType t,string program)
		{
			Program = program;
			Version = version;
			Uri = uri;
			FilePath = filePath;
			MD5 = md5;
			Description = description;
			LaunchArgs = launchArgs;
			Tag = t;
		}

		/// <summary>
		/// Checks if update's version is newer than the old version
		/// </summary>
		/// <param name="version">Application's current version</param>
		/// <returns>If the update's version # is newer</returns>
		public bool IsNewerThan(Version version)
		{
			return Version > version;
		}

		/// <summary>
		/// Checks the Uri to make sure file exist
		/// </summary>
		/// <param name="location">The Uri of the update.xml</param>
		/// <returns>If the file exists</returns>
		public static bool ExistsOnServer(Uri location)
		{
			if (location.ToString().StartsWith("file"))
			{
				return System.IO.File.Exists(location.LocalPath);
			}
			else
			{
				try
				{
					ServicePointManager.Expect100Continue = true;
					ServicePointManager.DefaultConnectionLimit = 9999;
					//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
					ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
					// Request the update.xml
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create(location.AbsoluteUri);
					// Read for response
					HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
					resp.Close();

					return resp.StatusCode == HttpStatusCode.OK;
				}
				catch (Exception ex)
				{
					var message = ex.Message;
					return false;
				}
			}
		}

		/// <summary>
		/// Parses the update.xml into SharpUpdateXml object
		/// </summary>
		/// <param name="location">Uri of update.xml on server</param>
		/// <returns>The SharpUpdateXml object with the data, or null of any errors</returns>
		public static SharpUpdateXml[] ParseSmart(Uri location)
		{
			List<SharpUpdateXml> result = new List<SharpUpdateXml>();
			Version version = null;
			string url = "", filePath = "", md5 = "", description = "", launchArgs = "", program = "";

			try
			{
				// Load the document
				ServicePointManager.ServerCertificateValidationCallback = (s, ce, ch, ssl) => true;
				XmlDocument doc = new XmlDocument();
				doc.Load(location.AbsoluteUri);

				// Gets the appId's node with the update info
				// This allows you to store all program's update nodes in one file
				// XmlNode updateNode = doc.DocumentElement.SelectSingleNode("//update[@appID='" + appID + "']");
				XmlNodeList updateNodes = doc.DocumentElement.SelectNodes("/Updates/SmartDesignUpdate/update");
				foreach (XmlNode updateNode in updateNodes)
				{
					// If the node doesn't exist, there is no update
					if (updateNode == null)
						return null;

					// Parse data
					program = "Smart Design Update";
					version = Version.Parse(updateNode["version"].InnerText);
					url = updateNode["url"].InnerText;
					filePath = updateNode["filePath"].InnerText;
					md5 = updateNode["md5"].InnerText;
					description = updateNode["description"].InnerText;
					launchArgs = updateNode["launchArgs"].InnerText;

					result.Add(new SharpUpdateXml(version, new Uri(url), filePath, md5, description, launchArgs, JobType.UPDATE,program));
				}

				XmlNodeList addNodes = doc.DocumentElement.SelectNodes("/Updates/SmartDesignUpdate/add");
				foreach (XmlNode addNode in addNodes)
				{
					// If the node doesn't exist, there is no add
					if (addNode == null)
						return null;

					// Parse data
					program = "Smart Design Update";
					version = Version.Parse(addNode["version"].InnerText);
					url = addNode["url"].InnerText;
					filePath = addNode["filePath"].InnerText;
					md5 = addNode["md5"].InnerText;
					description = addNode["description"].InnerText;
					launchArgs = addNode["launchArgs"].InnerText;

					result.Add(new SharpUpdateXml(version, new Uri(url), filePath, md5, description, launchArgs, JobType.ADD,program));
				}

				XmlNodeList removeNodes = doc.DocumentElement.SelectNodes("/Updates/SmartDesignUpdate/remove");
				foreach (XmlNode removeNode in removeNodes)
				{
					// If the node doesn't exist, there is no remove
					if (removeNode == null)
						return null;

					// Parse data
					program = "Smart Design Update";
					filePath = removeNode["filePath"].InnerText;
					description = removeNode["description"].InnerText;
					launchArgs = removeNode["launchArgs"].InnerText;

					result.Add(new SharpUpdateXml(null, null, filePath, null, description, launchArgs, JobType.REMOVE,program));
				}

				return result.ToArray();
			}
			catch (Exception ex) { string test = ex.Message;
				return result.ToArray();
			}
		}

		public static SharpUpdateXml[] ParseDeployer(Uri location)
		{
			List<SharpUpdateXml> result = new List<SharpUpdateXml>();
			Version version = null;
			string url = "", filePath = "", md5 = "", description = "", launchArgs = "", program = "";

			try
			{
				// Load the document
				ServicePointManager.ServerCertificateValidationCallback = (s, ce, ch, ssl) => true;
				XmlDocument doc = new XmlDocument();
				doc.Load(location.AbsoluteUri);

				// Gets the appId's node with the update info
				// This allows you to store all program's update nodes in one file
				// XmlNode updateNode = doc.DocumentElement.SelectSingleNode("//update[@appID='" + appID + "']");
				XmlNodeList updateNodes = doc.DocumentElement.SelectNodes("/Updates/DeployerTool/update");
				foreach (XmlNode updateNode in updateNodes)
				{
					// If the node doesn't exist, there is no update
					if (updateNode == null)
						return null;

					// Parse data
					program = "DeployerTool";
					version = Version.Parse(updateNode["version"].InnerText);
					url = updateNode["url"].InnerText;
					filePath = updateNode["filePath"].InnerText;
					md5 = updateNode["md5"].InnerText;
					description = updateNode["description"].InnerText;
					launchArgs = updateNode["launchArgs"].InnerText;

					result.Add(new SharpUpdateXml(version, new Uri(url), filePath, md5, description, launchArgs, JobType.UPDATE, program));
				}

				XmlNodeList addNodes = doc.DocumentElement.SelectNodes("/Updates/DeployerTool/add");
				foreach (XmlNode addNode in addNodes)
				{
					// If the node doesn't exist, there is no add
					if (addNode == null)
						return null;

					// Parse data
					program = "DeployerTool";
					version = Version.Parse(addNode["version"].InnerText);
					url = addNode["url"].InnerText;
					filePath = addNode["filePath"].InnerText;
					md5 = addNode["md5"].InnerText;
					description = addNode["description"].InnerText;
					launchArgs = addNode["launchArgs"].InnerText;

					result.Add(new SharpUpdateXml(version, new Uri(url), filePath, md5, description, launchArgs, JobType.ADD, program));
				}

				XmlNodeList removeNodes = doc.DocumentElement.SelectNodes("/Updates/DeployerTool/remove");
				foreach (XmlNode removeNode in removeNodes)
				{
					// If the node doesn't exist, there is no remove
					if (removeNode == null)
						return null;

					// Parse data
					program = "DeployerTool";
					filePath = removeNode["filePath"].InnerText;
					description = removeNode["description"].InnerText;
					launchArgs = removeNode["launchArgs"].InnerText;

					result.Add(new SharpUpdateXml(null, null, filePath, null, description, launchArgs, JobType.REMOVE, program));
				}

				return result.ToArray();
			}
			catch (Exception ex)
			{
				string test = ex.Message;
				return result.ToArray();
			}
		}
	}
}
