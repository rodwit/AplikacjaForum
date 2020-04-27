using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AplikacjaKlient.Zalogowany
{
	/// <summary>
	/// Interaction logic for Zalogowany.xaml
	/// </summary>
	public partial class Glowny : UserControl
	{
		private MainWindow _rodzic;
		private Lista _lista;
		public enum Widok { LISTA, ROZMOWA};

		public Glowny(MainWindow rodzic)
		{
			InitializeComponent();
			_rodzic = rodzic;
			_lista = new Lista(this);
			contentControl.Content = _lista;
			LabelUzytkownik.Content = Klient.Instancja().Login;
		}

		public void PrzelaczWidok(Widok widok, int index = -1)
		{
			if (widok == Widok.LISTA)
				contentControl.Content = _lista;
			else
				contentControl.Content = new Rozmowa.Glowny(this, index);
		}

		private void ButtonNowyWatek_Click(object sender, RoutedEventArgs e)
		{
			DodajWatek dodajWatek = new DodajWatek();
			dodajWatek.Owner = _rodzic;
			bool? wynik = dodajWatek.ShowDialog();

			if (wynik != true)
				return;

			Klient.Instancja().NowyWatek(dodajWatek.ZwrocTemat());
			_lista.AktualizujListe();

		}

		private void ButtonOdswiez_Click(object sender, RoutedEventArgs e)
		{
			_lista.AktualizujListe();
		}
	}
}
