using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using WspolnyInterfejs;
using System.Threading;
using System.Xml.Linq;


namespace AplikacjaSerwer
{
	class Polaczenie
	{
		bool _zalogowany = false;
		TcpClient _klient = null;
		TcpAdapter _tcpAdapter = null;

		bool aktywny = true;

		private static List<Watek> _watki; // lista tematów rozmów

		Thread _licznikThread = null;
		Licznik _licznik = null;

		public Polaczenie(TcpClient klient)
		{
			_klient = klient;
			_tcpAdapter = new TcpAdapter(_klient);

			if (_watki == null)
			{
				_watki = new List<Watek>();

				//testowy 
				Watek testowy = new Watek(1, "Testowy", "Test");
				_watki.Add(testowy);
			}

		}



		public void Start()
		{
			Console.WriteLine("Nowe połączenie");

			_licznik = new Licznik(this);
			_licznikThread = new Thread(_licznik.Start);
			_licznikThread.Start();

			while (aktywny)
			{
				if (!_tcpAdapter.DaneDostepne())
				{
					continue;
				}
				_licznik.Reset();
				_licznik.Wstrzymaj();


				switch (_tcpAdapter.OdbierzKomende())
				{
					case WspolnyInterfejs.Komendy.LOGOWANIE:
						logowanie();
						break;
					case WspolnyInterfejs.Komendy.REJESTRACJA:
						rejestracja();
						break;
					case WspolnyInterfejs.Komendy.LISTA:
						lista();
						break;
					case WspolnyInterfejs.Komendy.TEMAT:
						Console.WriteLine("Temat");
						break;
					case Komendy.UTRZYMAJ:
						Console.WriteLine("Utrzymaj");
						break;
					default:
						Console.WriteLine("Default");
						break;
				}

				_licznik.Wznow();
			}
			Console.WriteLine("Zakończono połączenie");

			_klient.Close();
		}

		private sealed class Licznik
		{
			private Polaczenie _klasaGlowna = null;
			private bool stan = true;
			private bool wstrzymaj = false;
			private const int _DANY_CZAS = 10;  // 60 s
			private int _licznik = 0;

			public Licznik(Polaczenie klasaGlowna)
			{
				_klasaGlowna = klasaGlowna;
			}
			public void Start()
			{
				while (stan)
				{
					if (wstrzymaj)
						continue;
					_licznik += 1;
					Console.WriteLine("Licznik: " + _licznik);
					Thread.Sleep(1000);
					if (_licznik > _DANY_CZAS)
					{
						wylacz();
						_licznik = 0;
					}
				}
			}

			private void wylacz()
			{
				stan = false;
				_klasaGlowna.aktywny = false;
			}

			public void Reset()
			{
				_licznik = 0;
			}

			public void Wstrzymaj()
			{
				wstrzymaj = true;
			}

			public void Wznow()
			{
				wstrzymaj = false;
			}
		}

		/// <summary>
		/// Zwraca wątek po id. Jak nie to null
		/// </summary>
		private Watek zwrocWatek(uint id) 
		{
			foreach (var ele in _watki)
				if (ele.ZwrocID == id)
					return ele;
			return null;
		}

		private void rejestracja()
		{
			string login = Encoding.UTF8.GetString(_tcpAdapter.OdbierzDane());
			string haslo = Encoding.UTF8.GetString(_tcpAdapter.OdbierzDane());

			bool flagaStworz = false;

			XDocument uzytkownicy = XDocument.Load("./uzytkownicy.xml");
			var temp = uzytkownicy.Root.Element(login);
			if (temp == null)
				flagaStworz = true;


			if (flagaStworz)
			{
				uzytkownicy.Root.Add(new XElement(login, haslo));
				uzytkownicy.Save("./uzytkownicy.xml");
				_tcpAdapter.WyslijKomende(Komendy.POTWIERDZENIE);
				return;
			}
			else
			{
				_tcpAdapter.WyslijKomende(Komendy.NIE_POTWIERDZENIE);
				return;
			}

		}

		void logowanie()
		{
			string login = Encoding.UTF8.GetString(_tcpAdapter.OdbierzDane());
			string haslo = Encoding.UTF8.GetString(_tcpAdapter.OdbierzDane());

			XDocument uzytkownicy = XDocument.Load("./uzytkownicy.xml");
			var temp = uzytkownicy.Root.Element(login);

			if (temp != null)
				if (temp.Value == haslo)
				{ _zalogowany = true; }

			if (_zalogowany)
				_tcpAdapter.WyslijKomende(Komendy.POTWIERDZENIE);
			else
				_tcpAdapter.WyslijKomende(Komendy.NIE_POTWIERDZENIE);
		}

		void lista()
		{
			if (_zalogowany == false) //wartownik
				return;


			if (_watki.Count == 0)
			{
				_tcpAdapter.WyslijKomende(Komendy.NIE_POTWIERDZENIE); // jak nie ma wątków
				return;
			}
			_tcpAdapter.WyslijKomende(Komendy.POTWIERDZENIE);
			_tcpAdapter.WyslijDane(BitConverter.GetBytes(_watki.Count)); // wyślij liczbę wątków
			foreach (var ele in _watki)
				_tcpAdapter.WyslijDane(ele.ZwrocMniejszy()); // wyślij tematy

		}
	}
}
