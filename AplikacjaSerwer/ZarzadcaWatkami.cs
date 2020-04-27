using System;
using System.Collections.Generic;
using System.Text;
using WspolnyInterfejs;

namespace AplikacjaSerwer
{
	class ZarzadcaWatkami : INadzorcaObserwatorow
	{
		private static ZarzadcaWatkami _instancja;
		private List<Watek> _lista;
		private Stack<int> _wolne;
		private Dictionary<IObserwator, int> _listaObserwatorowRozmow;


		public static ZarzadcaWatkami Instancja()
		{
			if (_instancja == null)
				_instancja = new ZarzadcaWatkami();
			return _instancja;
		}


		private ZarzadcaWatkami()
		{
			_listaObserwatorowRozmow = new Dictionary<IObserwator, int>();
			_lista = new List<Watek>();
			_wolne = new Stack<int>();
		}

		public void DodajWatek(string temat, string autor)
		{
			int index = -1;
			if(_wolne.Count > 0)
			{
				index = _wolne.Pop();
				Watek watek = new Watek(index, temat, autor);
				_lista[index] = watek;
				return;
			}
			else
			{
				index = _lista.Count;
				Watek watek = new Watek(index, temat, autor);
				_lista.Add(watek);
				return;
			}
		}

		public Watek ZwrocWatek(int index) { return _lista[index]; }

		public int Count { get => _lista.Count; }


		public Watek[] ToArray() => _lista.ToArray();

		public void DodajObserwatora(IObserwator obserwator, int index)
		{
			_listaObserwatorowRozmow.Add(obserwator, index);
		}

		public void UsunObserwatora(IObserwator obserwator)
		{
			_listaObserwatorowRozmow.Remove(obserwator);
		}

		public void Powiadom()
		{
			foreach (var ele in _listaObserwatorowRozmow)
				ele.Key.Aktualizuj();
		}
	}
}
