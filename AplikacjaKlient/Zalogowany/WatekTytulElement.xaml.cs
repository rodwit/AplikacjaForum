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
		private int _id;

		string _tytulProperty;
		string _autorProperty;




		public WatekTytulElement()
		{
			InitializeComponent();

		}

		public WatekTytulElement(Watek watek) : this()
		{
			_tytulProperty = "Tytuł :"+watek.ZwrocNazwe;
			_autorProperty = "Autor: "+watek.ZwrocAutora;
			_id = watek.ZwrocID;
		}

		public string TytulProperty { get => _tytulProperty; set => _tytulProperty = value; }
		public string AutorProperty { get => _autorProperty; set => _autorProperty = value; }
		public int Id { get => _id; }
	}
}
