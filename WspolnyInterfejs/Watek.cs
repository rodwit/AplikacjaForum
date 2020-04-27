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
		int _id = -1;
		string _nazwa;
		string _autor;

		List<Post> _rozmowa = new List<Post>();

		public Watek(int id, string nazwa, string autor)
		{

			_id = id;
			if (_id == -1)
				throw new Exception("_id Watek rowne 0");
			_nazwa = nazwa;
			_autor = autor;
		}

		public void DodajPost(Post post)
		{
			_rozmowa.Add(post);
		}

		public int ZwrocID { get =>_id; }

		/// <summary>
		/// Zwraca Watek bez zapisu rozmowy
		/// </summary>
		/// <returns></returns>
		public Watek ZwrocMniejszy()
		{
			Watek temp = new Watek(_id,_nazwa,_autor);
			return temp;
		}

		public List<Post> ZwrocRozmowe()
		{
			return _rozmowa;
		}

		public string ZwrocNazwe { get => _nazwa; }

		public string ZwrocAutora { get => _autor; }
	}

	[Serializable]
	public class Post
	{
		private object _dane;
		private string _autor;
		private string _data;
		private string _czas;

		
		public Post(object dane, string autor, DateTime czas)
		{
			_dane = dane;
			_autor = autor;
			_data = czas.Date.ToLongDateString();
			_czas = czas.Date.ToShortTimeString();
		}

		public string Autor { get => _autor; set => _autor = value; }
		

		public object Dane => _dane;

		public string Data { get => _data; set => _data = value; }
		public string Czas { get => _czas; set => _czas = value; }
	}

	[Serializable]
	public class PostText : Post
	{
		private string _daneString;
		public PostText(string dane, string autor, DateTime czas) : base(null,autor, czas)
		{
			_daneString = dane;
		}
		public new string Dane => _daneString;
	}

	[Serializable]
	public class PostEmotka : Post
	{
		public PostEmotka(int dane, string autor, DateTime czas) : base(dane, autor, czas)
		{
		}
	}

	[Serializable]
	public class PostObraz : Post
	{
		public PostObraz(Graphics dane, string autor, DateTime czas) : base(dane, autor, czas)
		{
		}

		public PostObraz(Bitmap dane, string autor, DateTime czas) : base(Graphics.FromImage(dane), autor, czas)
		{
			
		}
	}
}