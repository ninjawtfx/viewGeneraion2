using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Extreme.Net;

namespace ViewBot
{
	internal static class Proxies
	{
		public static List<ProxyClient> ProxyList = new List<ProxyClient>();
		public static List<string> GoodProxyList = new List<string>();

		private static List<ProxyClient> getProxiesFromFile(ProxyType type, string fileName)
		{
			List<ProxyClient> proxiesList = new List<ProxyClient>();

			foreach (var line in File.ReadAllLines(fileName))
			{
				ProxyClient client;

				try
				{
					client = ProxyClient.Parse(type, line.Trim());
				}

				catch (Exception)
				{
					continue;
				}

				if (!proxiesList.Contains(client))
					proxiesList.Add(client);

			}

			return proxiesList;
		}

		private static void WriteInFileGoodProxy()
		{
			GoodProxyList.Add(DateTime.Now.ToLongTimeString());

			File.WriteAllLines("GoodProxySocks5.txt", GoodProxyList.ToArray());
		}

		public static List<ProxyClient> GetGoodProxies()
		{
			return getProxiesFromFile(ProxyType.Http, @"Proxies\fileForTopProxy.txt");
		}

		public static void GetProxies()
		{
			//ProxyList.AddRange(getProxiesFromFile(ProxyType.Http, "GoodProxySocks4.txt"));
			//ProxyList.AddRange(getProxiesFromFile(ProxyType.Socks4, "Socks4.txt"));
			ProxyList.AddRange(getProxiesFromFile(ProxyType.Http, @"fileForTopProxy.txt"));
		}
	}
}

	

