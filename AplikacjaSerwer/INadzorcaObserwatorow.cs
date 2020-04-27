using System;
using System.Collections.Generic;
using System.Text;

namespace WspolnyInterfejs
{
	interface INadzorcaObserwatorow
	{
		public void DodajObserwatora(IObserwator obserwator, int index);
		public void UsunObserwatora(IObserwator obserwator);
		public void Powiadom();
	}
}
