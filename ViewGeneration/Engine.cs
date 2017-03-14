using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Extreme.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace ViewGeneration
{
	public class Engine
	{
		private readonly List<Searcher> _searches;

		[ThreadStatic] private Random _r;

		public Engine(List<Searcher> searches)
		{
			_searches = searches;
		}

		public void DoWork(object prox)
		{
			_r = new Random();

			var proxy = (ProxyClient)prox;

			var ch = _r.Next(0, _searches.Count);

			getDriver(proxy, _searches[_r.Next(0, _searches.Count)]);
		}

		private void getDriver(ProxyClient proxy, Searcher searcher)
		{
			ChromeDriver webDriver = null;

			try
			{
				var chromeOptions = new ChromeOptions();

				Proxy proxyChrome = new Proxy();
				proxyChrome.SslProxy = proxy.Host + ":" + proxy.Port;
				proxyChrome.HttpProxy = proxy.Host + ":" + proxy.Port;

				chromeOptions.Proxy = proxyChrome;
				chromeOptions.AddArgument($"--proxy-server=\"{proxy.Host}:{proxy.Port}\"");
				chromeOptions.AddArgument("--ignore-certificate-errors");
				chromeOptions.AddArgument("--ignore-ssl-errors");

				webDriver = new ChromeDriver(chromeOptions);

				webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(5);

				webDriver.Navigate().GoToUrl(searcher.YandexUrl);

				Thread.Sleep(_r.Next(10000, 12000));

				webDriver.FindElementByXPath(searcher.ElementToClick).Click();

				Thread.Sleep(_r.Next(150000, 300000));

				File.AppendAllText(@"F:\Proxies\topProxies.txt", $"{proxy.Host}:{proxy.Port}\n");

				webDriver.FindElementByXPath("//a[@href='http://brutaleveryday.ru/']").Click();

				Thread.Sleep(_r.Next(100000, 150000));
			}
			catch (Exception)
			{
				try
				{
					webDriver.Quit();
				}
				catch (Exception)
				{
					return;
				}
			}

			try
			{
				webDriver.Quit();
			}
			catch (Exception)
			{
				
			}
		}
	}
}
