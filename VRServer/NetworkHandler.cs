using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using MongoDB.Bson;

namespace VRServer
{
	// State object for reading client data asynchronously
	public class NetworkState
	{
		public Socket workSocket = null;
		// Size of receive buffer.
		public const int BufferSize = 4 * 1024;
		// Receive buffer.
		public byte[] buffer = new byte[BufferSize];

		// Buffer in which to accumulate a BSON document's bytes
		public MemoryStream accumBuffer = new MemoryStream(BufferSize);

		// Buffer in which to store received messages
		public Queue<BsonDocument> messageBuffer = new Queue<BsonDocument>();

		public IEnumerable<BsonDocument> GetNewMessages()
		{
			BsonDocument[] arr;
			lock (messageBuffer) {
				arr = messageBuffer.ToArray();
				messageBuffer.Clear();
			}
			return arr;
		}
	}

	public class NetworkHandler
	{
		const int ServerPort = 4444;
		// Client  socket.

		TcpListener listener;
		List<Player> NewPlayers = new List<Player>();

		public void StartListening()
		{
			try {
				Util.Log(String.Format("Starting to listen on port {0}", ServerPort));
				// XXX: This does not necessarily work right, since listening on an IPv6 socket
				// does not necessarily imply listening on an IPv4 socket.
				listener = new TcpListener(IPAddress.IPv6Any, ServerPort);
				listener.Start();
			} catch (Exception e) {
				Util.Log("Could not create TcpListener");
				throw e;
			}
		}

		

		public IEnumerable<Player> GetNewPlayers()
		{
			Player[] arr;
			lock (NewPlayers) {
				arr = NewPlayers.ToArray();
				NewPlayers.Clear();
			}
			return arr;
		}

		public void CheckForNewConnections()
		{
			listener.BeginAcceptSocket(new AsyncCallback(AcceptCallback), listener);
		}

		void AcceptCallback(IAsyncResult ar)
		{
			// Get the socket that handles the client request.
			var l = ar.AsyncState as TcpListener;
			if (l == null) {
				throw new Exception("No TCPListerner; this should never happen!");
			}
			var sock = l.EndAcceptSocket(ar);
			//Console.WriteLine("Connection accepted");

			var state = new NetworkState();
			var p = new Player(state);
			lock (NewPlayers) {
				NewPlayers.Add(p);
			}
			state.workSocket = sock;
			sock.BeginReceive(state.buffer, 0, NetworkState.BufferSize, 0,
			                     new AsyncCallback(ReceiveCallback), state);

			// This call forms a sort of asynchronous loop, starting up a new 'process'
			// such as it is which makes sure the callback gets called again 
			CheckForNewConnections();
		}

		void ReceiveCallback(IAsyncResult ar)
		{
			// Retrieve the state object and the handler socket
			// from the asynchronous state object.
			var state = ar.AsyncState as NetworkState;
			if (state == null) {
				throw new Exception("No NetworkState; this should never happen!");
			}
			var sock = state.workSocket;

			// Read data from the client socket. 
			try {
				// We get the data into an array
				int bytesRead = sock.EndReceive(ar);
				//Console.WriteLine("Received {0} bytes", bytesRead);

				// 0 bytes read == connection closed.
				if (bytesRead == 0) {
					return;
				}

				// Copy that into our stream
				var accm = state.accumBuffer;
				accm.Write(state.buffer, 0, bytesRead);
				accm.Seek(0, SeekOrigin.Begin);
				// Check if there are any full BSON docs in the stream
				if (VRLib.Util.BsonIsComplete(accm)) {
					// Grab docs off the stream and do stuff with them
					do {
						//Console.WriteLine("Reading from stream...");
						var bson = BsonDocument.ReadFrom(accm);
						Console.WriteLine(bson);
						// The async callbacks are actually in a different thread
						// behind the scenes, so we must synchronize data going out of them
						// to the game's main loop
						lock (state.messageBuffer) {
							state.messageBuffer.Enqueue(bson);
						}
					} while(VRLib.Util.BsonIsComplete(accm));

					// Then copy anything left to a new stream.
					// Waste of memory, but, screw it.
					// C# 3.5 does NOT Have MemoryStream.CopyTo()
					// KHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAN!
					var newstream = new MemoryStream();
					var fuckmewitharake = accm.ToArray();
					var harder = accm.Position;
					var faster = accm.Length;
					newstream.Write(fuckmewitharake, (int)harder, (int)(faster - harder));
					state.accumBuffer = newstream;
				}
				state.accumBuffer.Seek(0, SeekOrigin.End);
			} catch (SocketException e) {
				Util.Log("Error on finishing receiving:");
				Util.Log(e.ToString());
				sock.Close();
				return;
			}

			
			sock.BeginReceive(state.buffer, 0, NetworkState.BufferSize, 0,
			                  new AsyncCallback(ReceiveCallback), state);

		}

		public void SendTo(NetworkState ns, BsonDocument data)
		{
			var bytedata = VRLib.Util.BsonToBytes(data);
			Send(ns.workSocket, bytedata);
		}

		public void SendTo(NetworkState ns, byte[] data)
		{
			Send(ns.workSocket, data);
		}
    
		void Send(Socket sock, byte[] data)
		{
			// Convert the string data to byte data using ASCII encoding.
			//byte[] byteData = Encoding.ASCII.GetBytes(data);

			// Begin sending the data to the remote device.
			Console.WriteLine("Trying to send {0} bytes", data.Length);
			try {
				sock.BeginSend(data, 0, data.Length, 0,
			                  new AsyncCallback(SendCallback), sock);
			} catch (SocketException e) {
				Util.Log("Error on beginning sending:");
				Util.Log(e.ToString());
				sock.Close();
				return;
			}
		}

		void SendCallback(IAsyncResult ar)
		{
			Socket sock = ar.AsyncState as Socket;
			if (sock == null) {
				throw new Exception("No Socket; this should never happen!");
			}
			try {
				// Retrieve the socket from the state object.

				// Complete sending the data to the remote device.
				Console.WriteLine("Ending send...");
				int bytesSent = sock.EndSend(ar);
				Console.WriteLine("Sent {0} bytes to client.", bytesSent);
			} catch (SocketException e) {
				Util.Log("Error on ending sending:");
				Util.Log(e.ToString());
				sock.Close();
				return;
			}

			//handler.Shutdown(SocketShutdown.Both);
			//handler.Close();
		}
	}

}

