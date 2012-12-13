using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Text;

using MongoDB.Bson;
using VRLib;

namespace VRTestClient
{
	public class VRTestClient
	{
	
		const string serverName = "localhost";
		const int serverPort = 4444;
		// Use this for initialization
		public static void Main(string[] args)
		{
			Console.WriteLine("Starting...");
			var client = new TcpClient(serverName, serverPort);
			Console.WriteLine(string.Format("Client connected: {0}", client.Connected));
			var stream = client.GetStream();

			while (true) {
				Console.WriteLine("Writing to stream...");
				var msg = EncodeMessage.PlayerEnteredSystem();
				msg.WriteTo(stream);
				stream.Flush();
				var response = BsonDocument.ReadFrom(stream);
				Console.WriteLine("Received {0}", response);
				Thread.Sleep(100);


			}
		}
	
	}
}
