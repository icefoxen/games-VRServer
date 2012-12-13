using System;
using System.Collections.Generic;

using OpenTK;

namespace VRLib
{
	public interface IGameObj
	{
		// Attributes
		long Id { get; } // ID is guaranteed to be unique and not change over the object's lifetime.

		Vector3d OldLoc { get; set; }
		Vector3d Loc { get; set; }
		Quaterniond OldFacing { get; set; }
		Quaterniond Facing { get; set; }

		Vector3d Vel { get; set; }

		double RVel { get; set; }
		double Mass { get; set; }
		double Moment { get; set; }
		Collider Collider { get; set; }

		// True if colliding with it does something
		bool Collidable { get; set; }

		bool Alive { get; set; }
                
		// Operations
		void Push(Vector3d force);
		void Rotate(Vector3d axis, double torque);
		bool Colliding(IGameObj o);
		void OnCollide(IGameObj o, IPlace g);
		void Update(IPlace g, double dt);
		void Die(IPlace g);
		void FreezeCallback(double freezeTime);
		void UnfreezeCallback(double unfreezeTime);

	}

	/*
	public interface IControllable : IGameObj
	{
		double MaxVel { get; set; }
		IGameObj Target { get; set; }
		void Thrust();
		void TurnLeft();
		void TurnRight();
		void Brake();
		//void Fire(GameState g);
		//void Secondary(GameState g);
		//void Special(GameState g);
		//void SwitchMain();
		//void SwitchSecondary();
		//void SwitchSpecial();
	}*/

	/// <summary>
	/// Any kind of object in space that is affected by conventional physics and 
	/// </summary>
	public class SpaceObj : IGameObj
	{
		// Attributes
		public long Id { get; set; } 
		public Vector3d OldLoc { get; set; }
		public Vector3d Loc { get; set; }
		public Quaterniond OldFacing { get; set; }
		public Quaterniond Facing { get; set; }

		public Vector3d Vel { get; set; }

		public double RVel { get; set; }
		public double Mass { get; set; }
		public double Moment { get; set; }
		public Collider Collider { get; set; }

		// True if colliding with it does something
		public bool Collidable { get; set; }

		public bool Alive { get; set; }

		
		public SpaceObj(Vector3d loc)
		{
			Loc = loc;
			OldLoc = loc;
			Facing = Quaterniond.Identity;
			OldFacing = Quaterniond.Identity;
			Mass = 1.0;
			Moment = 1.0;
			Collider = null;
			Vel = Vector3d.Zero;
			RVel = 0;
			Collidable = false;
		}

		// Operations
		public virtual void Push(Vector3d force)
		{
			// Planets do not get pushed.
		}
		public virtual void Rotate(Vector3d axis, double torque)
		{
			// Planets do not have their rotation changed, either.
		}
		public bool Colliding(IGameObj o)
		{
			if (Collidable && o.Collidable) {
				return Collider.Colliding(o.Collider);
			} else {
				return false;
			}
		}
		public virtual void OnCollide(IGameObj o, IPlace g)
		{
		}

		public virtual void Die(IPlace g)
		{
		}


		public virtual void Update(IPlace g, double dt)
		{
			OldLoc = Loc;
		}

		
		public virtual void FreezeCallback(double freezeTime)
		{
		}
		public virtual void UnfreezeCallback(double unfreezeTime)
		{
		}
	}

	public class Ship : SpaceObj
	{
		public Ship(Vector3d loc) : base(loc)
		{

		}
	}

	/// <summary>
	/// An astronomical body in a star system... a star, planet, moon, whatever.
	/// As such, it moves on a fixed orbit and cannot be physically affected by player interactions.
	/// </summary>
	public class Body : IGameObj
	{
		public string Name { get; set; }
		// The parent and orbit can be null.
		public Body Parent { get; set; }
		public KeplerOrbit Orbit { get; set; }
		public double Radius { get; set; }
		double orbitTime = 0.0;

		
		// Attributes
		public long Id { get; set; } 
		public Vector3d OldLoc { get; set; }
		public Vector3d Loc { get; set; }
		public Quaterniond OldFacing { get; set; }
		public Quaterniond Facing { get; set; }

		public Vector3d Vel { get; set; }

		public double RVel { get; set; }
		public double Mass { get; set; }
		public double Moment { get; set; }
		public Collider Collider { get; set; }

		// True if colliding with it does something
		public bool Collidable { get; set; }

		public bool Alive { get; set; }

		
		public Body(Vector3d loc, Quaterniond facing, double mass)
		{
			Loc = loc;
			OldLoc = loc;
			Facing = facing;
			OldFacing = facing;
			Mass = mass;
			Moment = 1.0;
			Collider = new SphereCollider(loc, 1.0);
			Vel = Vector3d.Zero;
			RVel = 0;
			Collidable = true;
		}

		public Body(Vector3d loc, Quaterniond facing, double mass, Body parent, KeplerOrbit orbit) : this(loc, facing, mass)
		{
			Parent = parent;
			Orbit = orbit;
		}

		public Body() : this(Vector3d.Zero, Quaterniond.Identity, 1.0)
		{
		}
                
		// Operations
		public virtual void Push(Vector3d force)
		{
			// Planets do not get pushed.
		}
		public virtual void Rotate(Vector3d axis, double torque)
		{
			// Planets do not have their rotation changed, either.
		}
		public bool Colliding(IGameObj o)
		{
			if (Collidable && o.Collidable) {
				return Collider.Colliding(o.Collider);
			} else {
				return false;
			}
		}
		public virtual void OnCollide(IGameObj o, IPlace g)
		{
		}

		public virtual void Die(IPlace g)
		{
		}

		Vector3d getLocation(double t)
		{
			if (Parent == null) {
				return Loc;
			} else {
				var ploc = Parent.getLocation(t);
				var offset = Orbit.offsetVector(t);
				return ploc + new Vector3d(offset);
			}
		}

		public virtual void Update(IPlace g, double dt)
		{
			OldLoc = Loc;
			//Console.WriteLine("Updated body, dt {0}", dt);
			orbitTime += dt;
			Loc = getLocation(orbitTime);
		}

		
		public virtual void FreezeCallback(double freezeTime)
		{
			orbitTime = freezeTime;
		}
		public virtual void UnfreezeCallback(double unfreezeTime)
		{
			orbitTime = unfreezeTime;
		}
	}
}

