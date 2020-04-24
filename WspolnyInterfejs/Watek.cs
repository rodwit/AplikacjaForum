using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace WspolnyInterfejs
{
	[Serializable]
	public class Watek
	{
		uint _id = 0;
		string _nazwa;
		string _autor;
		List<Post> _rozmowa = new List<Post>();

		public Watek(uint id, string nazwa, string autor)
		{
			_id = id;
			if (_id == 0)
				throw new Exception("_id Watek rowne 0");
			_nazwa = nazwa;
			_autor = autor;
		}

		public void DodajPost(Post post)
		{
			_rozmowa.Add(post);
		}

		public uint ZwrocID { get =>_id; }

		/// <summary>
		/// Zwraca Watek bez zapisu rozmowy
		/// </summary>
		/// <returns></returns>
		public Watek ZwrocMniejszy()
		{
			Watek temp = new Watek(_id,_nazwa,_autor);
			return temp;
		}

		public string ZwrocNazwe { get => _nazwa; }

		public string ZwrocAutora { get => _autor; }
	}

	[Serializable]
	public class Post
	{
		private object _dane;

		
		public Post(object dane)
		{
			_dane = dane;
		}

		public object WezDane() { return _dane; }

	}

	[Serializable]
	public class PostText : Post
	{
		private string _daneString;
		public PostText(string dane) : base(null)
		{
			_daneString = dane;
		}
	}

	[Serializable]
	public class PostEmotka : Post
	{
		public PostEmotka(int dane) : base(dane)
		{
		}
	}

	[Serializable]
	public class PostObraz : Post
	{
		public PostObraz(Graphics dane) : base(dane)
		{
		}

		public PostObraz(Bitmap dane) : base(Graphics.FromImage(dane))
		{
			
		}
	}
}