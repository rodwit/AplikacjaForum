using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Net;
using WspolnyInterfejs;

namespace AplikacjaSerwer
{
	sealed class Serwer
	{
		private static Serwer _instancja = null;
		private TcpListener _tcpListener = null;
		private bool _uruchomiony = false;

		private List<Polaczenie> _polaczenia = new List<Polaczenie>();

		public static Serwer instancja()
		{
			if (_instancja == null)
				_instancja = new Serwer();
			return _instancja;
		}

		private Serwer()
		{
			XDocument konfiguracja = XDocument.Load("./konfiguracja.xml");
			string adres = konfiguracja.Root.Element("Adres").Value;
			int port = Int32.Parse(konfiguracja.Root.Element("Port").Value);

			_tcpListener = new TcpListener(System.Net.IPAddress.Parse(adres), port);


		}

		public void Start()
		{
			Console.WriteLine("Start Serwer:");
			Console.WriteLine("Adres: " + ((IPEndPoint)_tcpListener.LocalEndpoint).Address.ToString());
			Console.WriteLine("Port: " + ((IPEndPoint)_tcpListener.LocalEndpoint).Port);

			_tcpListener.Start();
			while (true)
			{
				//if (!_uruchomiony)
				//break;
				if (_tcpListener.Pending())
					continue;
				Polaczenie nowePolaczenie = new Polaczenie(_tcpListener.AcceptTcpClient());
				_polaczenia.Add(nowePolaczenie);
				Thread watekPolaczenia = new Thread(new ParameterizedThreadStart(polaczenie));
				watekPolaczenia.Start(_polaczenia[_polaczenia.Count - 1]);
			}
			_tcpListener.Stop();
		}

		private void polaczenie(object objectPolaczenie)
		{
			Polaczenie polaczenie = (Polaczenie)objectPolaczenie;
			polaczenie.Start();
		}

	}
}
