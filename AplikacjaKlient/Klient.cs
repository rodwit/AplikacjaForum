using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Xml.Linq;
using System.Threading;
using WspolnyInterfejs;

namespace AplikacjaKlient
{
	public sealed class Klient
	{
		private static Klient _instancja = null;
		TcpClient _klient = null;
		TcpAdapter _tcpAdapter = null;

		Thread _licznikThread = null;
		Licznik _licznik;

		public static Klient instancja()
		{
			if (_instancja == null)
				_instancja = new Klient();
			return _instancja;
		}


		private Klient()
		{
			XDocument konfiguracja = XDocument.Load("./konfiguracja.xml");
			string adres = konfiguracja.Root.Element("Adres").Value;
			int port = Int32.Parse(konfiguracja.Root.Element("Port").Value);

			
			try
			{
				_klient = new TcpClient(adres, port);
			}
			catch(Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
				Environment.Exit(-100);
			}
			_tcpAdapter = new TcpAdapter(_klient);

			_licznik = new Licznik(this);

			_licznikThread = new Thread(_licznik.Start);
			_licznikThread.Start();
		}

		public void Stop()
		{
			_licznik.Wylacz();
		}

		~Klient()
		{
			_klient.Close();
			_licznik.Wylacz();
		}


		private sealed class Licznik
		{
			private bool stan = true;
			private const int _DANY_CZAS = 5;  // 60 s
			private int _licznik = 0;
			Klient _klasaGlowna;

			public Licznik(Klient klasaGlowna)
			{
				_klasaGlowna = klasaGlowna;
				System.Console.WriteLine("dasdasd");
			}
			public void Start()
			{
				while (stan)
				{
					_licznik += 1;
					Console.WriteLine("Licznik: " + _licznik);
					Thread.Sleep(1000);
					if (_licznik > _DANY_CZAS)
					{
						wyslijTest();
						_licznik = 0;
					}
				}
			}

			private void wyslijTest()
			{
				_klasaGlowna._tcpAdapter.WyslijKomende(WspolnyInterfejs.Komendy.UTRZYMAJ);
			}

			public void Wylacz()
			{
				stan = false;
			}

			public void Reset()
			{
				_licznik = 0;
			}
		}

		public bool Rejestracja(string login, string haslo)
		{
			_tcpAdapter.WyslijKomende(Komendy.REJESTRACJA);

			_tcpAdapter.WyslijDane(Encoding.UTF8.GetBytes(login));

			_tcpAdapter.WyslijDane(Encoding.UTF8.GetBytes(haslo));

			return (_tcpAdapter.OdbierzKomende() == Komendy.POTWIERDZENIE) ? true : false;

		}


	}
}
