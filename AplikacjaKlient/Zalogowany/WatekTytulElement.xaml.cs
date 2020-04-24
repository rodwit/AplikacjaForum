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
using WspolnyInterfejs;

namespace AplikacjaKlient.Zalogowany
{
	/// <summary>
	/// Interaction logic for Tytul.xaml
	/// </summary>
	public partial class WatekTytulElement : UserControl
	{
		private uint _id;
		public WatekTytulElement()
		{
			InitializeComponent();

		}

		public WatekTytulElement(Watek watek) : this()
		{
			_id = watek.ZwrocID;
			LabelTytul.Content = watek.ZwrocNazwe;
			LabelAutor.Content = watek.ZwrocAutora;
		}
	}
}
