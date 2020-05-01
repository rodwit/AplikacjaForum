using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Xml.Linq;
using System.Threading;
using WspolnyInterfejs;
using System.Diagnostics;

namespace AplikacjaKlient
{
	public sealed class Klient
	{
		private static Klient _instancja = null;
		private string _login = null;
		TcpAdapter _tcpAdapter = null;

		Thread _licznikThread = null;
		Licznik _licznik;

		public static Klient Instancja()
		{
			if (_instancja == null)
				_instancja = new Klient();
			return _instancja;
		}

		public string Login { get => _login; }


		private Klient()
		{
			XDocument konfiguracja = XDocument.Load("./konfiguracja.xml");
			string adres = konfiguracja.Root.Element("Adres").Value;
			int port = Int32.Parse(konfiguracja.Root.Element("Port").Value);

			
			try
			{
				_tcpAdapter = new TcpAdapter(adres, port);
			}
			catch(Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
				Environment.Exit(-100);
			}

			_licznik = new Licznik(this);

			_licznikThread = new Thread(_licznik.Start);
			_licznikThread.Start();
		}


		public void Stop()
		{
			_licznik.Wylacz();
			_tcpAdapter.Close();
			_login = null;
		}

		~Klient()
		{
			_tcpAdapter.Close();
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

		public bool Logowanie(string login, string haslo)
		{
			_tcpAdapter.WyslijKomende(Komendy.LOGOWANIE);
			_tcpAdapter.WyslijDane(Encoding.UTF8.GetBytes(login));
			_tcpAdapter.WyslijDane(Encoding.UTF8.GetBytes(haslo));

			if(_tcpAdapter.OdbierzKomende() == Komendy.POTWIERDZENIE)
			{
				_login = login;
				return true;
			}
			return false;
		}

		public Watek[] ListaWatkow()
		{
			_tcpAdapter.WyslijKomende(Komendy.LISTA);

			if(_tcpAdapter.OdbierzKomende() == Komendy.NIE_POTWIERDZENIE)
			{
				return null;
			}

			int ilosc = BitConverter.ToInt32(_tcpAdapter.OdbierzDane());
			Watek[] tablicaWatkow = new Watek[ilosc];

			for(int i=0; i < ilosc; ++i)
			{
				Watek temp = _tcpAdapter.ToWatek(_tcpAdapter.OdbierzDane());
				tablicaWatkow[i] = temp;
			}

			return tablicaWatkow;
		}

		public int NowyWatek(string temat)
		{
			_tcpAdapter.WyslijKomende(Komendy.NOWY_WATEK);
			_tcpAdapter.WyslijDane(UTF8Encoding.UTF8.GetBytes(temat));
			_tcpAdapter.OdbierzKomende();

			return 1;
			
		}

		public Watek PobierzWatek(int index)
		{
			_tcpAdapter.WyslijKomende(Komendy.WYSLIJ_WATEK);

			_tcpAdapter.WyslijDane(BitConverter.GetBytes(index));

			byte[] watedByte = _tcpAdapter.OdbierzDane();

			Watek watek = _tcpAdapter.ToWatek(watedByte);

			return watek;
		}

		public int WyslijPost(string tresc)
		{
			_tcpAdapter.WyslijKomende(Komendy.ZAPISZ_POST);

			PostText post = new PostText(tresc,_login,DateTime.Now);

			_tcpAdapter.WyslijDane(post);

			_tcpAdapter.OdbierzKomende();

			return 1;
		}

		/// <summary>
		/// Nasłuchuje na pojawienie się nowego postu.
		/// </summary>
		/// <returns></returns>
		public bool CzyPost()
		{
			if (_tcpAdapter.DaneDostepne())
			{
				if (_tcpAdapter.OdbierzKomende() == Komendy.WATEK_ZMIENIONY)
					return true;
			}
			else
				return false;

			return false;
		}

		public void ZglosObserwacjeWatku(bool zglos)
		{
			_tcpAdapter.WyslijKomende(Komendy.OBSERWUJ_WATEK);
			if (zglos)
				_tcpAdapter.WyslijKomende(Komendy.POTWIERDZENIE);
			else
				_tcpAdapter.WyslijKomende(Komendy.NIE_POTWIERDZENIE);
		}

	}
}
