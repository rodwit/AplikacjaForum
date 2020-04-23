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
		private Lista _lista = new Lista();

		public Glowny()
		{
			InitializeComponent();
			contentControl.Content = _lista;
		}
	}
}
