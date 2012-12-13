#define VERBOSELOG
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using OpenTK;
using MongoDB.Bson;
using VRLib;

namespace VRServer
{
	/// <summary>
	/// Player state.
	/// Describes what the player is currently up to, which defines what messages are valid and how
	/// it responds to them.
	/// LoggingIn1: Client has connected but has not given handshake
	/// LoggingIn2: Client has connected and has given client handshake
	/// InGame: Client is in a Place in the game
	/// Moving: Client is moving between Places
	/// Disconnecting: The client has sent a message that it is disconnected
	/// Reconnecting: The client has disconnected uncleanly, so it should be expected it will try to reconnected
	/// Pingout: The client has not disconnected but its pings are absent or very slow
	/// </summary>
	enum PlayerState
	{
		LoggingIn1,
		LoggingIn2,
		InGame,
		Moving,
		Disconnecting,
		Reconnecting,
		Pingout
	}

	/// <summary>
	/// Server connection.
	/// A connection from a server to a single client.
	/// Should asynchronously manage state with the client,
	/// tell it what it needs to know, and be able to provide
	/// status information.
	/// </summary>
	public class Player
	{
		// Network properties
		public bool Connected { get; set; }
		public double LastPing { get; set; }
		public NetworkState NetState { get; set; }

		// Game properties
		public string Name { get; set; }
		public Ship Ship { get; set; }
		public Place Place { get; set; }

		public Player(NetworkState net)
		{
			NetState = net;
			Name = "";
			Connected = false;
		}

		public void Update(NetworkHandler net, double dt)
		{
			Console.WriteLine("Player updated...");
			HandleMessages();
		}

		public void HandleMessages()
		{
			var msgs = NetState.GetNewMessages();
			foreach (var m in msgs) {
				Console.WriteLine("Player received message {0}", m);
			}
		}

		/*
		public void DoHandshake()
		{
#if VERBOSELOG
			Util.Log("Doing handshake");
#endif
			var a = EncodeMessage.ServerHandshake("test servername");
			// XXX: ...this is NOT asynchronous!  Um.  Threads, I guess?
			a.WriteTo(stream);
			var response = BsonDocument.ReadFrom(stream);
			if (!DecodeMessage.CheckMessageKind(MessageKind.ClientHandshake, response)) {
				Util.Log("Handshake failed: client did not send right response");
			} else {
#if VERBOSELOG
				Util.Log(String.Format("Got valid handshake response, username {0}, passwd {1}", response ["username"], response ["password"]));
#endif
				var b = EncodeMessage.ServerHandshakeAccepted();
				b.WriteTo(stream);
				Connected = true;
			}
		}

		// Okay... so we are going to have to wait for a bsondocument, read it,
		// then figure out what the hell it is and do SOMETHING to the client state
		// based on what it is.
		// Oof.
		public void ListenForData()
		{
			//var msg = BsonDocument.ReadFrom(stream);

		}

		public void DoPing()
		{
			var t = 0;
			Util.Log("Doing ping at time {0}");
			var ping = EncodeMessage.Ping(t);
			ping.WriteTo(stream);
			//var pong = BsonDocument.ReadFrom(stream);

		}
		*/
	}
}

