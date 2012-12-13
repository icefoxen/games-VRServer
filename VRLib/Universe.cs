using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using System.Drawing;

namespace VRLib
{

	// A location in which players move around.
	// Also, for our purposes, the basic unit of our instances.
	public interface IPlace
	{
		int Id { get; }
		Vector2d Coords { get; set; }
		string Name { get; set; }
		ICollection<IGameObj> Objs { get; set; }
		ICollection<IGameObj> DeadObjs { get; set; }
		ICollection<IGameObj> GetObjsWithin(Vector3d place, double distance);

		void AddGameObj(IGameObj g);
		void RemoveGameObj(IGameObj g);
		void KillGameObj(IGameObj g);
		void Update(double dt);
		void FreezeCallback(double freezeTime);
		void UnfreezeCallback(double unfreezeTime);
	}

	/// <summary>
	/// // A location in the universe that you can go to
	/// </summary>
	public class Place : IPlace
	{
		public int Id { get; set; }
		// This is the location of the place in the larger universe.
		public Vector2d Coords { get; set; }
		public string Name { get; set; }

		public ICollection<IGameObj> Objs { get; set; }
		public ICollection<IGameObj> DeadObjs { get; set; }

		public Place(int id, Vector2d c)
		{
			Id = id;
			Name = "";
			Coords = c;
			Objs = new HashSet<IGameObj>();
			DeadObjs = new HashSet<IGameObj>();
		}

		public ICollection<IGameObj> GetObjsWithin(Vector3d place, double distance)
		{
			var accm = new List<IGameObj>();
			foreach (var o in Objs) {
				var dist2 = Vector3d.Subtract(place, o.Loc).LengthSquared;
				if (dist2 < (distance * distance)) {
					accm.Add(o);
				}
			}
			return accm;
		}

		public void AddGameObj(IGameObj g)
		{
			Objs.Add(g);
		}
		public void RemoveGameObj(IGameObj g)
		{
			Objs.Remove(g);
		}
		public void KillGameObj(IGameObj g)
		{
			Objs.Remove(g);
			DeadObjs.Add(g);
		}

		public void Update(double dt)
		{
			foreach (var g in Objs) {
				g.Update(this, dt);
			}
		}
		public void FreezeCallback(double freezeTime)
		{

		}
		public void UnfreezeCallback(double unfreezeTime)
		{

		}
	}


	public enum StarType
	{
		BrownDwarf,
		RedDwarf,
		OrangeDwarf,
		YellowDwarf,
		GreenDwarf,
		BlueGiant,
		WhiteGiant,
		WhiteSupergiant,

		RedGiant,
		BlackHole,
		NeutronStar,
		WhiteDwarf,
		Nebula,
	}

	public enum PlanetType
	{
		Terran,
		Ice,
		Desert,
		Dead,
		Inferno,
		GasGiant,
		Ocean
	}

	/// <summary>
	/// Our fundamental physics approximation.
	/// Q: Why not do a 2-body solution?
	/// A: Because if you have any situations where a 2-body solution is 
	/// significantly better than this, you rapidly become aware that it is
	/// actually not a 2-body problem but an n-body problem.
	/// ie, two stars of equal size orbiting a common center of mass...  
	/// now imagine it with planets trying to orbit each star... ugh.
	/// Q: Why not do an n-body solution?
	/// A: I want to keep the solution stable over arbitrary time periods, and
	/// be able to trivially solve it at any arbitrary point of time.
	/// Some research suggests that Sufficiently Fast And Stable solutions exist
	/// for sufficiently small n though (dozens or 100's)...
	/// </summary>
	public class KeplerOrbit
	{
		double eccentricity;
		double semiLatus;

		public KeplerOrbit(double e, double p)
		{
			eccentricity = e;
			semiLatus = p;
		}

		public double radius(double theta)
		{
			return semiLatus / (1 + eccentricity * Math.Cos(theta));
		}

		public double rMin()
		{
			return semiLatus / (1 + eccentricity);
		}

		public double rMax()
		{
			return semiLatus / (1 - eccentricity);
		}

		public double semiMajor()
		{
			return semiLatus / (1 - (eccentricity * eccentricity));
		}

		public double semiMinor()
		{
			return semiLatus / Math.Sqrt(1 - (eccentricity * eccentricity));
		}

		/// <summary>
		/// Kepler's third law
		/// </summary>
		public double period()
		{
			var area = Math.PI * semiMajor() * semiMinor();
			// gravityConstant actually = 4*pi^2 / (G(M1 + M2))
			// But we have no mass for these objects yet, so.
			var gravityConstant = 1;
			return Math.Sqrt(gravityConstant * (area * area * area));
		}

		public double angularVelocity(double r)
		{
			// r = radius
			var a = semiMajor();
			var b = semiMinor();
			var n = 2 * Math.PI / period();
			var dtheta = n * a * b / (r * r);
			return dtheta;
		}

		
		/// <summary>
		/// Apparently actually solving for this parameter is best done by "iterative numerical algorithms",
		/// according to wikipedia, so that's what this is.
		/// We need to find E from the equation
		/// M = E - e * sin(E)
		/// where e is the eccentricity and M is the mean anomaly, found above.
		/// </summary>
		/// <returns>
		/// Eccentric anomaly
		/// </returns>
		/// <param name='M'>
		/// Mean anomaly
		/// </param>
		public double holyShitSolveEccentricAnomaly(double M)
		{
			// Emperically, M is a pretty good place to start guessing.
			// A little less emperically, e*sin(E) is always less than 1
			// (for closed orbits), so if
			// M = E - e*sin(e)
			// then E is always within 1 of M.
			double guess = M;
			double precision = 0.00001;
			double result = 0;
			int step = 1;
			double offset = 0.5;
			double offsetChange = 0.5;
			do {
				result = guess - (eccentricity * Math.Sin(guess));
				//Console.WriteLine("Want {0}, guessed E={1}, got {2}, difference {3}",
				//                  M, guess, result, result - guess);

				if (result < M) {
					guess += offset;
				} else {
					guess -= offset;
				}
				offset *= offsetChange;
				step += 1;
			} while (Math.Abs(M - result) > precision);
			/*
			Console.WriteLine("Want {0}, guessed E={1}, got {2}, difference {3}",
			                  M, guess, result, result - guess);

			Console.WriteLine("Solved in {0} steps", step);
			*/
			return guess;
		}

		/// <summary>
		/// Returns the angular position of the body as a function of time, with perihelion being 0
		/// </summary>
		/// <returns>
		/// The angular position.
		/// </returns>
		/// <param name='t'>
		/// Time, assuming that t=0 results in an angular position of 0 (perihelion)
		/// </param>
		public double angularPosition(double t)
		{
			var M = (2 * Math.PI * t) / period();
			var E = holyShitSolveEccentricAnomaly(M);
			//var almostTrueAnomaly = Math.Sqrt((1+eccentricity) / (1-eccentricity)) * Math.Tan(E/2);
			var trueAnomaly = Math.Atan2(Math.Sqrt(1 + eccentricity) * Math.Sin(E / 2), 
			                             Math.Sqrt(1 - eccentricity) * Math.Cos(E / 2)) * 2;
			return trueAnomaly;
		}

		// XXX: orbit inclination?
		public Vector3d offsetVector(double t)
		{
			var theta = angularPosition(t);
			var r = radius(theta);
			//Console.WriteLine ("R = {0}, theta = {1}", r, theta);
			var x = r * Math.Cos(theta);
			var y = r * Math.Sin(theta);
			return new Vector3d(x, y, 0);
		}
	}

}

