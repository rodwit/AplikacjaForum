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
	/// Interaction logic for Lista.xaml
	/// </summary>
	public partial class Lista : UserControl
	{
		public Lista()
		{
			InitializeComponent();
			List<WatekTytulElement> watekTytuls = new List<WatekTytulElement>();
			watekTytuls.Add(new WatekTytulElement());
			watekTytuls.Add(new WatekTytulElement());
			watekTytuls.Add(new WatekTytulElement());
			lista.ItemsSource = watekTytuls;
		}
	}
}
