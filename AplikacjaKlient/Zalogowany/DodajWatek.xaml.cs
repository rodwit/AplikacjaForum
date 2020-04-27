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
using System.Windows.Shapes;

namespace AplikacjaKlient.Zalogowany
{
	/// <summary>
	/// Interaction logic for DodajWatek.xaml
	/// </summary>
	public partial class DodajWatek : Window
	{
		public DodajWatek()
		{
			InitializeComponent();
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

		}

		private void ButtonAnuluj_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		public string ZwrocTemat()
		{
			return TextBoxTemat.Text;
		}
	}
}
