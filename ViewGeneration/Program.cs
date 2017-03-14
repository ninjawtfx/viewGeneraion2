using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Extreme.Net;
using OpenQA.Selenium.Internal;
using ViewBot;

namespace ViewGeneration
{
	class Program
	{
		private static string _selfIp;
		private static readonly Regex _reges = new Regex("\\d+.\\d+.\\d+.\\d+");

		static void Main(string[] args)
		{
			_selfIp = getIp(null).Result;

			closeAllChromeInstances();

			var proxyList = Proxies.GetGoodProxies();

			Console.WriteLine("Начинаю работу");

			var engine = new Engine(new List<Searcher>
				{
					new Searcher
					{
						YandexUrl = "https://yandex.ru/search/?text=%D0%BF%D0%B8%D0%BB%D0%BE%D1%82%D0%BD%D0%B0%D1%8F%20%D0%B7%D0%B0%D0%BF%D0%B8%D1%81%D1%8C%20%D0%B2%20%D0%B1%D0%BB%D0%BE%D0%B3%D0%B5&lr=213&p=4",
						ElementToClick = "//a[text()=' | BrutalEveryDay']"
					},

					new Searcher()
					{
						YandexUrl = "https://yandex.ru/search/?msid=1489506224.20821.20941.27535&text=%D0%BC%D1%83%D0%B7%D0%B5%D0%B9%20%D0%B2%D0%BE%D1%81%D1%81%D1%82%D0%B0%D0%BD%D0%B8%D1%8F%20%D0%BC%D0%B0%D1%88%D0%B8%D0%BD&lr=213&p=20",
						ElementToClick = "(//b[text()= 'Восстание'])[1]"
					},

					//new Searcher
					//{
					//	YandexUrl = "https://yandex.ru/search/?text=work%20and%20travel%20%D1%81%D1%88%D0%B0&lr=10738&p=5",
					//	ElementToClick = "//a[text()=': Что взять с собой в ']"
					//},

					//new Searcher()
					//{
					//	YandexUrl = "https://yandex.ru/search/?text=work%20and%20travel%20%D0%B2%D0%B8%D0%B7%D0%B0&lr=213&p=3",
					//	ElementToClick = "//a[text()='Writetravel | Блог о ']"
					//},

					//new Searcher()
					//{
					//	YandexUrl = "https://yandex.ru/search/?text=working%20travel&lr=213&p=6",
					//	ElementToClick = "//a[text()='Writetravel | Блог о ']"
					//},

				//new Searcher()
				//	{
				//		YandexUrl = "https://vk.com/away.php?to=http%3A%2F%2Fbrutaleveryday.ru%2Fkak-poxudet-pravilnoe-pitanie%2F&post=401123499_8&el=snippet",
				//		ElementToClick = "//a[@href='http://brutaleveryday.ru/kak-poxudet/']"
				//	},

				//new Searcher()
				//{
				//	YandexUrl = "https://vk.com/away.php?to=http%3A%2F%2Fbrutaleveryday.ru%2Fkak-poxudet%2F&post=401123499_10&el=snippet",
				//	ElementToClick = "//a[@href='http://brutaleveryday.ru/kak-poxudet-pravilnoe-pitanie/']"
				//}
				});


			var sw = new Stopwatch();
			sw.Start();

			ParallelOptions opt = new ParallelOptions();
			ThreadPool.SetMaxThreads(50, 50);

			bool isRunning = false;

			while (true)
			{
				if (!isRunning)
				{
					isRunning = true;
					Parallel.ForEach(proxyList, (prox, state) =>
					{
						string proxyIp = string.Empty;

						try
						{
							proxyIp = getIp(prox).Result;
						}
						catch (Exception ex)
						{
							return;
						}

						if (sw.Elapsed.Minutes > 100)
						{
							isRunning = false;
							closeAllChromeInstances();
							sw.Restart();
							state.Break();
						}

						if (proxyIp != string.Empty && proxyIp != _selfIp)
							ThreadPool.QueueUserWorkItem(engine.DoWork, prox);
					});
				}
			}
		}

		private static void closeAllChromeInstances()
		{
			var drivers = Process.GetProcessesByName("chromedriver");

			foreach (var driver in drivers)
			{
				try
				{
					driver.Kill();
				}
				catch (Exception)
				{ 
				}
			}

			var chromes = Process.GetProcessesByName("chrome");

			foreach (var chrome in chromes)
			{
				try
				{
					chrome.Kill();
				}
				catch (Exception)
				{

				}
				
			}
		}

		private static async Task<string> getIp(ProxyClient proxy)
		{
			ProxyHandler handler = new ProxyHandler(proxy);

			using (HttpClient client = new HttpClient(handler))
			{
				var direction = client.GetStringAsync("https://www.check-my-ip.net/").Result;

				var ss = direction.Split(new[] { "<h1>", " - " }, StringSplitOptions.RemoveEmptyEntries);

				foreach (var line in ss)
				{
					if (_reges.IsMatch(line))
						return line;
				}

				return _selfIp;
			}
		}

		private static bool checkProxy(ProxyClient proxy)
		{
			var proxyIp = getIp(proxy).Result;

			if (proxyIp == _selfIp)
				return false;
			
			return true;
		}
	}
}
