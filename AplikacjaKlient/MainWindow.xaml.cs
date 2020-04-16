using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public enum WIDOK { START, LOGOWANIE, ZALOGOWANY, REJESTRACJA };
		private UserControl _aktualnyWidok;

		public MainWindow()
		{
			InitializeComponent();
			przelaczWidok(WIDOK.START);

		}

		void DataWindow_Closing(object sender, CancelEventArgs e)
		{
			Klient.instancja().Stop();
		}

		public void przelaczWidok(WIDOK widok)
		{
			switch (widok)
			{
				case WIDOK.START:
					_aktualnyWidok = new Start(this);
					break;
				case WIDOK.LOGOWANIE:
					//_aktualnyWidok = new Logowanie(this);
					//break;
				case WIDOK.ZALOGOWANY:
					_aktualnyWidok = new Zalogowany();
					break;
				case WIDOK.REJESTRACJA:
					_aktualnyWidok = new Rejestracja(this);
					break;
				default:
					_aktualnyWidok = new Start(this);
					break;
			}
			this.DataContext = _aktualnyWidok;
		}
	}
}
