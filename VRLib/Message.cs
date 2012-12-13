using System;
using System.Collections.Generic;

using MongoDB.Bson;


namespace VRLib
{
	public enum MessageKind
	{
		InvalidMessage = 0,
		ServerHandshake,
		ClientHandshake,
		ServerHandshakeAccepted,
		ServerHandshakeRejected,
		Ping,
		Pong,

		Place,
		Starmap,
		PhysicsUpdate,
		PlayerMovement,
		PlayerLeftSystem,
		PlayerEnteredSystem,
	}

	/// <summary>
	/// A class containing a communication from the client to the server or vice versa.
	/// Contains methods to serialize or deserialize it.
	/// </summary>
	public class Message
	{
		const int ProtocolVersion = 1;

		public MessageKind Kind { get; set; }
		public virtual BsonDocument Serialize()
		{
			return null;
		}

		public static Message Deserialize(BsonDocument doc)
		{
			var kind = (MessageKind)(doc ["k"].AsInt32);
			switch (kind) {
				case MessageKind.ServerHandshake:
					break;
				default:
					break;
			}
			return new Message();
		}
	}

	public class ServerHandshakeMsg : Message
	{
		public ServerHandshakeMsg()
		{
			Kind = MessageKind.ServerHandshake;
		}
	}
	public class ClientHandshakeMsg : Message
	{

	}
	public class ServerHandshakeAcceptedMsg : Message
	{

	}
	public class ServerHandshakeRejectedMsg : Message
	{

	}
	public class PingMsg : Message
	{

	}
	public class PongMsg : Message
	{

	}


}

