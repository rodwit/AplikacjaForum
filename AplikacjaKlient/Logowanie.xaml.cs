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

namespace AplikacjaKlient
{
	/// <summary>
	/// Interaction logic for Logowanie.xaml
	/// </summary>
	public partial class Logowanie : UserControl
	{
		private MainWindow _rodzic;

		public Logowanie(MainWindow rodzic)
		{
			InitializeComponent();
			_rodzic = rodzic;
		}

		private void buttonAnuluj_Click(object sender, RoutedEventArgs e)
		{
			_rodzic.przelaczWidok(MainWindow.WIDOK.START);
		}

		private void buttonOk_Click(object sender, RoutedEventArgs e)
		{
			if (!Klient.Instancja().Logowanie(textboxLogin.Text, textboxHaslo.Password))
				MessageBox.Show("Błędne hasło lub login");
			else
				_rodzic.przelaczWidok(MainWindow.WIDOK.ZALOGOWANY);
		}
	}
}
