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
	/// Interaction logic for Start.xaml
	/// </summary>
	public partial class Start : UserControl
	{
		private MainWindow _rodzic;
		public Start(MainWindow rodzic)
		{
			InitializeComponent();
			_rodzic = rodzic;
		}

		private void buttonLogowanie_Click(object sender, RoutedEventArgs e)
		{
			_rodzic.przelaczWidok(MainWindow.WIDOK.LOGOWANIE);
		}

		private void buttonRejestracja_Click(object sender, RoutedEventArgs e)
		{
			_rodzic.przelaczWidok(MainWindow.WIDOK.REJESTRACJA);
		}
	}
}
