using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace VRLib
{
	public class UniverseGen
	{
		public const int PlaceCount = 1000;
		public const double MaxSize = 1000;
		Random rand;
		public UniverseGen()
		{
			rand = new Random();
		}

		public IEnumerable<Place> MakeUniverse()
		{
			var lst = new List<Place>();
			for (int i = 0; i < PlaceCount; i++) {
				//var p = MakeRandomPlace(i);
				var p = MakeRandomStarSystem(i);
				lst.Add(p);
			}
			return lst;
		}


		public Place MakeRandomPlace(int id)
		{
			var x = rand.NextDouble() * MaxSize;
			var y = rand.NextDouble() * MaxSize;

			Place p = new Place(0, Vector2d.Zero);
			/*
			p.idnum = id;
			p.loc = new Vector2d(x, y);
			p.bodies = new HashSet<Body>();
			p.name = "";
			*/
			return p;
		}
		
		public Place MakeRandomStarSystem(int id)
		{
			var x = rand.NextDouble() * MaxSize;
			var y = rand.NextDouble() * MaxSize;
			
			Place p = new Place(id, new Vector2d(x, y));
			var star = new Body(Vector3d.Zero, Quaterniond.Identity, 1.0);
			p.AddGameObj(star);
			var numBodies = 5;
			for (int i = 0; i < numBodies; i++) {
				
				var eccentricity = rand.NextDouble() * 0.5;
				var slr = rand.NextDouble() * 100 + 10;
				var o = new KeplerOrbit(eccentricity, slr);
				var planet = new Body(new Vector3d(i * 100, 0, 0), Quaterniond.Identity, 1.0, star, o);
				p.AddGameObj(planet);
			}
			/*
			p.loc = new Vector2d(x, y);
			p.idnum = id;
			p.bodies = new HashSet<Body>();
			p.name = "";
			
			var s = new Star(new Vector3d(400, 400, 0), 
			                 Util.RandomEnum<StarType>(rand));
			p.bodies.Add(s);

			//var numPlanets = rand.Next() % 15;
			var numPlanets = 5;
			for (int i = 0; i < numPlanets; i++) {

			}
			*/
			return p;
		}
	}
}

