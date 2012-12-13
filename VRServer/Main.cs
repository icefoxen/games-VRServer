using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using MongoDB.Bson;
using VRLib;

namespace VRServer
{
	class VRServer
	{

		public static void Main(string[] args)
		{	
			var g = new GameState();
			g.MainLoop();
		}
	}
}
