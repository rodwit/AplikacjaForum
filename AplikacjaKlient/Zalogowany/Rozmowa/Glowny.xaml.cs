﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WspolnyInterfejs;

namespace AplikacjaKlient.Zalogowany.Rozmowa
{
	/// <summary>
	/// Interaction logic for Glowny.xaml
	/// </summary>
	public partial class Glowny : UserControl
	{
		private Zalogowany.Glowny _rodzic;
		private int _index;
		
		private Thread _nasluchPost = null;
		private bool _nasluchuj = true;
		private bool _nasluchujPauza = false;

		public Glowny(Zalogowany.Glowny rodzic, int index)
		{
			InitializeComponent();
			_rodzic = rodzic;
			_index = index;
			aktualizuj();
			_nasluchPost = new Thread(nasluchujPost);
			_nasluchPost.Start();
		}

		private void aktualizuj()
		{
			Watek watek = Klient.Instancja().PobierzWatek(_index);
			LabelTemat.Content = watek.ZwrocNazwe;
			listaRozmowa.ItemsSource = watek.ZwrocRozmowe();
		}

		private void nasluchujPost()
		{
			Klient.Instancja().ZglosObserwacjeWatku(true);

			while (_nasluchuj)
			{
				if (_nasluchujPauza)
					continue;
				if (Klient.Instancja().CzyPost())
					aktualizuj();
			}
		}

		private void ButtonWyslij_Click(object sender, RoutedEventArgs e)
		{
			_nasluchujPauza = true;
			Klient.Instancja().WyslijPost(TextBoxPost.Text);
			TextBoxPost.Text = "";
			aktualizuj();
			_nasluchujPauza = false;
		}

		private void ButtonWstecz_Click(object sender, RoutedEventArgs e)
		{
			_nasluchuj = false;
			Klient.Instancja().ZglosObserwacjeWatku(false);
			_rodzic.PrzelaczWidok(Zalogowany.Glowny.Widok.LISTA);
		}
	}
}
