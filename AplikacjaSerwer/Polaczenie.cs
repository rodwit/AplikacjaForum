using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using WspolnyInterfejs;
using System.Threading;
using System.Xml.Linq;
using System.Net;

namespace AplikacjaSerwer
{
	class Polaczenie : IObserwatorRozmowy
	{
		bool _zalogowany = false;
		string _login = null;
		int _aktywnyWatek = -1;
		TcpAdapter _tcpAdapter = null;
		TcpAdapter _tcpAdapterTlo = null;

		bool aktywny = true;

		Thread _licznikThread = null;
		Licznik _licznik = null;

		public Polaczenie(TcpClient klient)
		{
			_tcpAdapter = new TcpAdapter(klient);

			//TcpClient tlo = new TcpClient(_tcpAdapter.Adres,_tcpAdapter.Port);
			//_tcpAdapterTlo = new TcpAdapter(tlo);

			Console.WriteLine("Podlaczono:");
			Console.WriteLine("Adress: ", _tcpAdapter.Adres);
			Console.WriteLine("Port: ", _tcpAdapter.Port);
		}

		public void AktualizujRozmowe()
		{
			_tcpAdapter.WyslijKomende(Komendy.WATEK_ZMIENIONY);
			//_tcpAdapter.WyslijDane(ZarzadcaWatkami.Instancja().ZwrocWatek(_aktywnyWatek));
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
					case Komendy.NOWY_WATEK:
						nowyWatek();
						break;
					case WspolnyInterfejs.Komendy.WYSLIJ_WATEK:
						wyslijWatek();
						break;
					case Komendy.ZAPISZ_POST:
						zapiszPost();
						break;
					case Komendy.OBSERWUJ_WATEK:
						obserwujWatek();
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

			_tcpAdapter.Close();
			ZarzadcaWatkami.Instancja().UsunObserwatora(this);
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

		void logowanie()
		{
			string login = Encoding.UTF8.GetString(_tcpAdapter.OdbierzDane());
			string haslo = Encoding.UTF8.GetString(_tcpAdapter.OdbierzDane());

			XDocument uzytkownicy = XDocument.Load("./uzytkownicy.xml");
			var temp = uzytkownicy.Root.Element(login);

			if (temp != null)
				if (temp.Value == haslo)
				{ _zalogowany = true; _login = login; }

			if (_zalogowany)
				_tcpAdapter.WyslijKomende(Komendy.POTWIERDZENIE);
			else
				_tcpAdapter.WyslijKomende(Komendy.NIE_POTWIERDZENIE);
		}

		void lista()
		{
			if (_zalogowany == false) //wartownik
				return;


			if (ZarzadcaWatkami.Instancja().Count == 0)
			{
				_tcpAdapter.WyslijKomende(Komendy.NIE_POTWIERDZENIE); // jak nie ma wątków
				return;
			}
			_tcpAdapter.WyslijKomende(Komendy.POTWIERDZENIE);
			_tcpAdapter.WyslijDane(BitConverter.GetBytes(ZarzadcaWatkami.Instancja().Count)); // wyślij liczbę wątków
			foreach (var ele in ZarzadcaWatkami.Instancja().ToArray())
				_tcpAdapter.WyslijDane(ele.ZwrocMniejszy()); // wyślij tematy

		}

		void nowyWatek()
		{
			if (_zalogowany == false) //wartownik
				return;

			string temat = Encoding.UTF8.GetString(_tcpAdapter.OdbierzDane());
			ZarzadcaWatkami.Instancja().DodajWatek(temat, _login);
			_tcpAdapter.WyslijKomende(Komendy.POTWIERDZENIE);
		}

		void wyslijWatek()
		{
			if (_zalogowany == false) //wartownik
				return;


			int index = BitConverter.ToInt32(_tcpAdapter.OdbierzDane());

			_aktywnyWatek = index;

			Watek watek = ZarzadcaWatkami.Instancja().ZwrocWatek(index);

			_tcpAdapter.WyslijDane(watek);

		}

		void zapiszPost()
		{
			if (_zalogowany == false) //wartownik
				return;

			Post post = _tcpAdapter.ToPost(_tcpAdapter.OdbierzDane());
			ZarzadcaWatkami.Instancja().DodajPost(_aktywnyWatek, post);
			_tcpAdapter.WyslijKomende(Komendy.POTWIERDZENIE);
		}

		void obserwujWatek()
		{
			if (_tcpAdapter.OdbierzKomende() == Komendy.POTWIERDZENIE)
				ZarzadcaWatkami.Instancja().DodajObserwatora(this, _aktywnyWatek);
			else
				ZarzadcaWatkami.Instancja().UsunObserwatora(this);
		}
	}
}
