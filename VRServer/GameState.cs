using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using MongoDB.Bson;
using MongoDB.Bson.IO;
using VRLib;

namespace VRServer
{


	class GameState
	{
		// Network stuff
		const string ServerName = "Test server";
		
		// We'll just keep all times in milliseconds, I guess.
		// Length of a physics frame, in milliseconds.
		// We run all physics at a fixed framerate.
		// 1000 milliseconds / 10 fps = 100 ms per frame.
		// We can get millisecond precision for a double for time durations of 100's of years.
		public const double physicsFrameTime = 1.0 / 10.0;
		double LastFrameTime;
		public Stopwatch Timer;

		HashSet<Place> UniverseLive;
		HashSet<Place> UniverseFrozen;

		HashSet<Player> Players;

		public GameState()
		{
			Timer = new Stopwatch();
			var ug = new UniverseGen();

			UniverseLive = new HashSet<Place>(ug.MakeUniverse());
			UniverseFrozen = new HashSet<Place>();
			Players = new HashSet<Player>();
			LastFrameTime = 0;
		}

		public void NextPhysicsFrame()
		{
			foreach (var p in UniverseLive) {
				p.Update((double)physicsFrameTime);
			}
		}

		public void FreezePlace(Place p)
		{
			p.FreezeCallback(LastFrameTime);
			UniverseLive.Remove(p);
			UniverseFrozen.Add(p);
		}

		public void UnfreezePlace(Place p)
		{
			p.UnfreezeCallback(LastFrameTime);
			UniverseFrozen.Remove(p);
			UniverseLive.Add(p);
		}

		public void MainLoop()
		{
			Util.Log("Server starting...");

			var net = new NetworkHandler();
			net.StartListening();
			var cont = true;
			net.CheckForNewConnections();
			Util.Log("Server started.");

			// Timing data
			Timer.Start();
			var framenum = 0;

			while (cont) {
				// Get input from network.
				// XXX: This polls constantly, which is somewhat awkward...
				// A better way might be to have a ProcessNewInput() function that
				// gets called from the ReceiveCallback.
				// Or something.
				foreach (var newp in net.GetNewPlayers()) {
					Players.Add(newp);

				}
				foreach (var p in Players) {
					p.Update(net, 1);
				}


				// Calc physics
				double now = ((double)Timer.ElapsedMilliseconds) / 1000.0;
				if ((LastFrameTime + physicsFrameTime) < now) {
					LastFrameTime = now;
					NextPhysicsFrame();
					framenum += 1;

					// Print stats
					if ((framenum % 10) == 0) {
						//var physicsFPS = now / framenum;
						var nownow = ((double)Timer.ElapsedMilliseconds) / 1000.0;
						Console.WriteLine("Now: {0}, LastFrameTime: {1}, physics frame took {2}", now, LastFrameTime, nownow - now);
					}
				}// else {
				// Thread.Sleep(0) yields back to the scheduler.
				//Thread.Sleep(0);
				//}


				// Send data to clients

			}
		}
	}
}
