using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace WspolnyInterfejs
{
	public class TcpAdapter
	{
		TcpClient _tcpClient;
		NetworkStream _networkStream;

		public TcpAdapter(string adres, int port)
		{

			_tcpClient = new TcpClient(adres, port);
			_networkStream = _tcpClient.GetStream();
		}

		public TcpAdapter(TcpClient tcpClient)
		{
			_tcpClient = tcpClient;
			_networkStream = _tcpClient.GetStream();
		}

		~TcpAdapter()
		{
			_networkStream.Close();
			_tcpClient.Close();
		}

		public string Adres
		{
			get
			{
				return ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address.ToString();
			}
		}

		public int Port
		{
			get
			{
				return ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Port;
			}
		}

		public void Close()
		{
			_networkStream.Close();
			_tcpClient.Close();
		}

		public byte[] OdbierzDane()
		{
			byte[] dane = new byte[sizeof(int)];
			_networkStream.Read(dane, 0, sizeof(int));
			int iloscDanych = BitConverter.ToInt32(dane);
			dane = new byte[iloscDanych];
			_networkStream.Read(dane, 0, iloscDanych);
			Console.WriteLine("Get data");
			return dane;
		}

		public Watek ToWatek(byte[] dane)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(dane);
			Watek wynik = (Watek)binaryFormatter.Deserialize(memoryStream);

			return wynik;
		}

		public Post ToPost(byte[] dane)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(dane);
			Post wynik = (Post)binaryFormatter.Deserialize(memoryStream);

			return wynik;
		}

		public void WyslijDane(byte[] dane)
		{
			Console.WriteLine("Send data");
			_networkStream.Write(BitConverter.GetBytes(dane.Length), 0, sizeof(int)); //wyślij rozmiar tablicy byte
			_networkStream.Write(dane, 0, dane.Length);
		}

		public void WyslijDane(object dane)
		{
			if (dane == null)
				throw new Exception("WyslijDane puste dane");
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
			binaryFormatter.Serialize(memoryStream, dane);

			WyslijDane(memoryStream.ToArray());

		}

		public void WyslijKomende(WspolnyInterfejs.Komendy komenda)
		{
			Console.WriteLine("Send command: " + komenda);
			byte[] komendaByte = BitConverter.GetBytes((int)komenda);
			_networkStream.Write(komendaByte, 0, komendaByte.Length);
		}

		public WspolnyInterfejs.Komendy OdbierzKomende()
		{
			byte[] polecenie = new byte[sizeof(int)];
			_networkStream.Read(polecenie, 0, sizeof(int));
			Komendy komenda = (Komendy)BitConverter.ToInt32(polecenie);
			Console.WriteLine("Get command: " + komenda);
			return komenda;
		}

		public bool DaneDostepne()
		{
			return _networkStream.DataAvailable;
		}

	}
}
