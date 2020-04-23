using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WspolnyInterfejs
{
	public struct Watek
	{
		string Nazwa;
		string Autor;
		List<Post> Rozmowa;
	}


	abstract class Post
	{
		private object _dane;

	}

/*
	public abstract class Post <T>
	{
		private T _dane;
		public Post(T dane)
		{
			_dane = dane;
		}
		public T WezDane() { return _dane; }
	}

	class PostText : Post<string>
	{
		public PostText(string dane) : base(dane)
		{
		}
	}

	class PostEmotka : Post<int>
	{
		public PostEmotka(int dane) : base(dane)
		{
		}
	}

	class PostObraz : Post<Graphics>
	{
		public PostObraz(Graphics dane) : base(dane)
		{
		}

		public PostObraz(Bitmap dane) : base(Graphics.FromImage(dane))
		{
			
		}
	}
	*/
}