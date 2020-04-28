using System;
using System.Collections.Generic;
using System.Text;

namespace WspolnyInterfejs
{
	interface INadzorcaObserwatorowRozmow
	{
		public void DodajObserwatora(IObserwatorRozmowy obserwator, int index);
		public void UsunObserwatora(IObserwatorRozmowy obserwator);
		public void Powiadom();
	}
}
