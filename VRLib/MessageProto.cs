using System;
using System.Collections.Generic;
using MongoDB.Bson;
using OpenTK;


namespace VRLib
{


	/// <summary>
	/// This encomasses the protocol for sending and receiving messages.
	/// </summary>
	public class EncodeMessage
	{
		const int ProtocolVersion = 1;

		public static BsonDocument CreateMessage(MessageKind kind, BsonDocument msg)
		{
			msg ["msgkind"] = kind;
			// XXX: Should each message include a checksum as well?  CRC32 perhaps?
			return msg;
		}

		// These are messages that the server must be able to communicate 
		// to the client...


		public static BsonDocument ServerHandshake(string servername)
		{
			var m = CreateMessage(MessageKind.ServerHandshake, 
			                       new BsonDocument {
				{"serverName", servername},
				{"protocol", ProtocolVersion},
				{"welcomeMessage", "Welcome to " + servername},
			}
			);
			return m;
		}

		public static BsonDocument ServerHandshakeAccepted()
		{
			var m = CreateMessage(MessageKind.ServerHandshakeAccepted, 
			                       new BsonDocument {
			}
			);
			return m;
		}

		public static BsonDocument ServerHandshakeRejected(string reason)
		{
			var m = CreateMessage(MessageKind.ServerHandshakeRejected, 
			                       new BsonDocument {
				{"reason", reason}
			}
			);
			return m;
		}

		public static BsonDocument ClientHandshake(string username, string pass)
		{
			var m = CreateMessage(MessageKind.ClientHandshake, 
			                       new BsonDocument {
				{"username", username},
				{"password", pass},
			}
			);
			return m;
		}

		public static BsonDocument Ping(double t)
		{
			var m = CreateMessage(MessageKind.Ping,
			                      new BsonDocument {
				{"time", t}
			}
			);
			return m;
		}

		public static BsonDocument Pong(double t)
		{
			var m = CreateMessage(MessageKind.Pong,
			                      new BsonDocument {
				{"time", t}
			}
			);
			return m;
		}

		// A Place, with all its contained bodies (but incomplete info on them)
		// A bunch of places, without any info on their contents (starmap)
		// Another player within sensor range, containing location and heading
		public static BsonDocument Place(Place p)
		{
			var accm = new BsonArray();
			foreach (Body b in p.Objs) {
				//var k = b.kind;
				//BsonDocument dc;
				/*
				switch (k) {
					case BodyKind.Planet:
						dc = Serialize.Planet((Planet)b);
						accm.Add(dc);
						break;
					case BodyKind.Star:
						dc = Serialize.Star((Star)b);
						accm.Add(dc);
						break;
					default:
						break;
				}
				*/
			}

			var d = CreateMessage(MessageKind.Place, new BsonDocument{
				{"name", p.Name},
				{"loc", Serialize.Vector2(p.Coords)},
				{"bodies", accm}
			}
			);
			return d;
		}

		public static BsonDocument Starmap(ICollection<Place> p)
		{
			var d = CreateMessage(MessageKind.Starmap, new BsonDocument{
			}
			);
			return d;
		}

		public static BsonDocument PhysicsUpdate()
		{
			var d = CreateMessage(MessageKind.PhysicsUpdate, new BsonDocument{
			}
			);
			return d;
		}

		// The messages a client must be able to communicate to the server:
		// That they have changed their movement
		// That they have left the current system
		// That they are entering a target system

		public static BsonDocument PlayerMovement()
		{
			var d = CreateMessage(MessageKind.PlayerMovement, new BsonDocument{
			}
			);
			return d;
		}

		public static BsonDocument PlayerLeftSystem()
		{
			var d = CreateMessage(MessageKind.PlayerLeftSystem, new BsonDocument{
			}
			);
			return d;
		}

		public static BsonDocument PlayerEnteredSystem()
		{
			var d = CreateMessage(MessageKind.PlayerEnteredSystem, new BsonDocument{
			}
			);
			return d;
		}
	}

	public class DecodeMessage
	{
		public static bool CheckMessageKind(MessageKind kind, BsonDocument msg)
		{
			// XXX: Should handle errors properly
			//var knd = msg.TryGetElement("msgkind", ...);
			return msg ["msgkind"] == kind;
		}

		//public static 

		public static Place Place(BsonDocument d)
		{
			//var k = (MessageKind) d["msgkind"].AsInt32;
			//var name = d ["name"].AsString;
			var loc = Deserialize.Vector2(d ["loc"].AsBsonArray);
			//var bodies = d ["bodies"].AsBsonArray;
			//var accm = new HashSet<Body>();
			/*
			foreach (var i in bodies) {
				var itm = i.AsBsonDocument;
				var kind = (BodyKind)itm ["bodykind"].AsInt32;
				Body b;
				switch (kind) {
					case BodyKind.Planet:
						b = Deserialize.Planet(itm);
						accm.Add(b);
						break;
					case BodyKind.Star:
						b = Deserialize.Star(itm);
						accm.Add(b);
						break;
					default:
						Console.WriteLine("AAAAAAAAAAAAAAH");
						b = new Star(Vector3d.Zero, StarType.BlackHole);
						break;
				}
				accm.Add(b);
			}
			*/

			var p = new Place(0, loc);
			/*
			p.name = name;
			p.loc = loc;
			p.bodies = accm;
*/
			return p;
		}
	}
}

