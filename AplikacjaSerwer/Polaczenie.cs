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
		TcpClient _klient = null;
		TcpAdapter _tcpAdapter = null;

		private bool aktywny = true;

		Thread _licznikThread = null;
		Licznik _licznik = null;

		public Polaczenie(TcpClient klient)
		{
			_klient = klient;
			_tcpAdapter = new TcpAdapter(_klient);
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
						Console.WriteLine("Logowanie");
						break;
					case WspolnyInterfejs.Komendy.REJESTRACJA:
						rejestracja();
						break;
					case WspolnyInterfejs.Komendy.LISTA:
						Console.WriteLine("Lista");
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
	}
}
