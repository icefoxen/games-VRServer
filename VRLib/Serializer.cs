using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using OpenTK;

namespace VRLib
{
	public class Serialize
	{
		public static BsonArray Vector2(Vector2d v)
		{
			var d = new BsonArray {
				v.X, v.Y
			};
			return d;
		}
		public static BsonArray Vector3(Vector3d v)
		{
			var d = new BsonArray {
				v.X, v.Y, v.Z
			};
			return d;
		}

		/*
		public static BsonDocument Star (Star s) {
			var d = new BsonDocument{
				{"bodykind", s.kind},
				{"name", s.name},
				{"loc", Vector3(s.loc)},
				{"startype", s.type}
			};
			return d;
		}

		public static BsonDocument Planet (Planet s) {
			var d = new BsonDocument{
				{"bodykind", s.kind},
				{"name", s.name},
				{"loc", Vector3(s.loc)},
				{"planettype", s.type}
			};
			return d;
		}
		*/
	}

	public class Deserialize
	{
		public static Vector2d Vector2(BsonArray b)
		{
			var x = b [0].AsDouble;
			var y = b [1].AsDouble;
			var v = new Vector2d(x, y);
			return v;
		}
		public static Vector3d Vector3(BsonArray b)
		{
			var x = b [0].AsDouble;
			var y = b [1].AsDouble;
			var z = b [2].AsDouble;
			var v = new Vector3d(x, y, z);
			return v;
		}
		/*
		public static Star Star(BsonDocument d)
		{
			//var k = (BodyKind) d["bodykind"].AsInt32;
			var n = d ["name"].AsString;
			var l = Vector3(d ["loc"].AsBsonArray);
			var t = (StarType)d ["startype"].AsInt32;

			var s = new Star(l, t);
			s.name = n;
			return s;
		}

		public static Planet Planet(BsonDocument d)
		{
			//var k = (BodyKind) d["bodykind"].AsInt32;
			var n = d ["name"].AsString;
			var l = Vector3(d ["loc"].AsBsonArray);
			var t = (PlanetType)d ["planettype"].AsInt32;

			var p = new Planet(l, t);
			p.name = n;
			return p;
		}
		*/
	}
}

