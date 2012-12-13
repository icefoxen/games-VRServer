using System;
using System.IO;

using OpenTK;
using MongoDB.Bson;

namespace VRLib
{
	public class Util
	{
		public static Random Rand = new Random();
                
		public const double PIOVER2 = Math.PI / 2;
		public const double TWOPI = Math.PI * 2;
                
		public static T RandomEnum<T>(Random r)
		{
			T[] values = (T[])Enum.GetValues(typeof(T));
			return values [r.Next(0, values.Length)];
		}

		/// <summary>
		/// Reads the first 4 bytes of the given array, and determines whether it is a complete BSON document.
		/// </summary>
		/// <returns>
		/// <c>true</c> if is the BSON document in the array is complete, otherwise <c>false</c>.
		/// </returns>
		/// <param name='b'>
		/// A byte array representing a serialized BSON document.
		/// </param>
		public static bool BsonIsComplete(byte[] b, int offset)
		{
			// The 'length' field of a BSON doc is 4 bytes.
			if (b.Length < offset + 4) {
				//Console.WriteLine("Too short!");
				return false;
			}
			var len = BitConverter.ToInt32(b, offset);
			//Console.WriteLine("Wanted {0} bytes, got {1} bytes", len, b.Length - offset);
			return len <= (b.Length - offset);
		}

		public static bool BsonIsComplete(MemoryStream s)
		{
			var b = s.ToArray();
			return BsonIsComplete(b, (int)s.Position);
		}
		
		public static byte[] BsonToBytes(BsonDocument b)
		{
			var s = new MemoryStream();
			b.WriteTo(s);
			var buf = s.ToArray();
			return buf;
		}
		
		public static BsonDocument BytesToBson(byte[] b)
		{
			//var s = new MemoryStream(b);
			//var d = BsonDocument.ReadFrom(s);
			//Console.WriteLine("Len {0}, byte count {1}", b.Length, BitConverter.ToInt32(b, 0));
			var d = BsonDocument.ReadFrom(b);
			return d;
		}

		// From http://stackoverflow.com/questions/1073336/circle-line-collision-detection
		public static bool CollideRayWithCircle(Vector2d rayStart, Vector2d ray, Vector2d circle, double r)
		{
			Vector2d f = Vector2d.Subtract(rayStart, circle);
			double a = Vector2d.Dot(ray, ray);
			double b = 2 * Vector2d.Dot(f, ray);
			double c = Vector2d.Dot(f, f) - r * r;
                        
			double discriminant = b * b - 4 * a * c;
			if (discriminant < 0) {
				return false;
				// no intersection
			} else {
				// ray didn't totally miss sphere,
				// so there is a solution to
				// the equation.
                                
                                
				discriminant = Math.Sqrt(discriminant);
				double t1 = (-b + discriminant) / (2 * a);
				//double t2 = (-b - discriminant) / (2 * a);
                                
				if (t1 >= 0 && t1 <= 1) {
					return true;
					// solution on is ON THE RAY.
				} else {
					return false;
					// solution "out of range" of ray
				}
                                
				// use t2 for second point
			}

		}
	}
}

