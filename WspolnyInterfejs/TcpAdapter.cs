using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace WspolnyInterfejs
{
	public class TcpAdapter
	{
		NetworkStream _networkStream;

		public TcpAdapter(TcpClient tcpClient)
		{
			_networkStream = tcpClient.GetStream();
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

		public void WyslijDane(byte[] dane)
		{
			Console.WriteLine("Send data");
			_networkStream.Write(BitConverter.GetBytes(dane.Length), 0, sizeof(int)); //wyślij rozmiar tablicy byte
			_networkStream.Write(dane, 0, dane.Length);
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
