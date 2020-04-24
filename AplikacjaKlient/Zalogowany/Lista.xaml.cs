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
	/// Interaction logic for Lista.xaml
	/// </summary>
	public partial class Lista : UserControl
	{
		public Lista()
		{
			InitializeComponent();
			Watek[] listaWatkow =  Klient.Instancja().Tematy();
			List<WatekTytulElement> watekTytuls = new List<WatekTytulElement>();

			foreach (var ele in listaWatkow)
			{
				WatekTytulElement watekTytulElement = new WatekTytulElement(ele);
				watekTytuls.Add(watekTytulElement);
			}
			
			lista.ItemsSource = watekTytuls;
		}
	}
}
