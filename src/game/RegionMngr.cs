// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;

namespace game
{
	public class Polygon
	{
		public int pointCount { get; set; }
		public Point[] points { get; set; }

		public Polygon()
		{
			pointCount = 0;
			points = new Point[0];
		}
	}

	public class CollisionLine
	{
		public Point P1 { get; set; }
		public Point P2 { get; set; }
	}

	public class Region
	{
		public List<uint> Npcs { get; set; }
		public List<uint> Monsters { get; set; }

		public Region()
		{
			this.Npcs = new List<uint>();
			this.Monsters = new List<uint>();
		}
	}

	public class Map
	{
		public CollisionLine[] Collisions;
	}

	public static class RegionMngr
	{
		public const int RegionSize = 180;	// Start at m000_000
		public const int MapSize = 16128;	// The size of one map in one dimension

		private static Region[][] Regions;
		private static Map[][] Maps;

		public static void Start()
		{
			Regions = new Region[Settings.MapLengthX * (MapSize / RegionSize)][];
			for (int i = 0; i < Regions.Length; i++)
			{
				Regions[i] = new Region[Settings.MapLengthY * (MapSize / RegionSize)];
				for (int j = 0; j < Regions[i].Length; j++)
					Regions[i][j] = new Region();
			}

			if (Settings.CheckCollision)
			{
				Maps = new Map[Settings.MapLengthX][];
				for (int i = 0; i < Maps.Length; i++)
				{
					Maps[i] = new Map[Settings.MapLengthY];
				}

				LoadMap("db/maps/m010_003.nfa");
			}
		}

		public static void AddNpcToRegion(Npc n1)
		{
			GetRegion(n1.Position.X, n1.Position.Y).Npcs.Add(n1.Handle);
		}

		private static void LoadMap(string dir)
		{
			ConsoleUtils.Write(ConsoleMsgType.Debug, "Loading Map...\n");

			int polyCount = 0;
			Polygon[] polygons;
			using (BinaryReader br = new BinaryReader(File.OpenRead(dir)))
			{
				polyCount = br.ReadInt32();
				polygons = new Polygon[polyCount];

				for (int i = 0; i < polyCount; i++)
				{
					polygons[i] = new Polygon();

					polygons[i].pointCount = br.ReadInt32();
					polygons[i].points = new Point[polygons[i].pointCount];

					for (int j = 0; j < polygons[i].pointCount; j++)
					{
						polygons[i].points[j] = new Point();
						polygons[i].points[j].X = (int)(br.ReadInt32());
						polygons[i].points[j].Y = (int)(br.ReadInt32());
					}
				}

				br.Close();
			}

			int mapX = 10; int mapY = 3;

			List<CollisionLine> cls = new List<CollisionLine>();
			for (int i = 0; i < polygons.Length; i++)
			{
				for (int j = 1; j < polygons[i].points.Length; j++)
				{
					CollisionLine cl = new CollisionLine();
					cl.P1 = polygons[i].points[j - 1];
					cl.P2 = polygons[i].points[j];
					cls.Add(cl);
				}
			}

			Maps[mapX][mapY] = new Map();
			Maps[mapX][mapY].Collisions = cls.ToArray();

			ConsoleUtils.Write(ConsoleMsgType.Debug, "Loaded Map...{0}\n", cls.Count);
		}

		internal static void PcWalkCheck(Player player, float startX, float startY, Point[] movePos)
		{
			if (!Settings.CheckCollision)
			{
				ClientPacketHandler.send_PCMoveTo(player, movePos.ToArray());
				return;
			}

			//ConsoleUtils.Write(ConsoleMsgType.Debug, "[Debug Move] startX: {0} ; startY: {1} ; Points: {2}\n", startX, startY, movePos.Length);
			//for (int i = 0; i < movePos.Length; i++)
			//	ConsoleUtils.Write(ConsoleMsgType.Debug, "--> MovePos[{0}]: ( {1}, {2} )\n", i, (int)movePos[i].X, (int)movePos[i].Y);

			List<Point> moves = new List<Point>();
			int mapX = 10; int mapY = 3;
			bool endOfTest = false;

			for (int i = 0; i < movePos.Length; i++)
			{
				int fromX, fromY;
				if (i == 0)
				{
					fromX = (int)startX; fromY = (int)startY;
				}
				else
				{
					fromX = (int)movePos[i - 1].X; fromY = (int)movePos[i - 1].Y;
				}

				Map m = Maps[mapX][mapY];

				for (int j = 0; j < m.Collisions.Length; j++)
				{
					if (Geometry.DoIntersect(new Point(fromX, fromY), movePos[i], m.Collisions[j].P1, m.Collisions[j].P2))
					{
						//ConsoleUtils.Write(ConsoleMsgType.Debug, "Intersects with ({0}, {1}) ({2}, {3})!\n", (int)m.Collisions[j].P1.X, (int)m.Collisions[j].P1.Y, (int)m.Collisions[j].P2.X, (int)m.Collisions[j].P2.Y);
						endOfTest = true;
						break;
					}
				}

				if (endOfTest)
				{
					break;
				}
				else
				{
					//ConsoleUtils.Write(ConsoleMsgType.Debug, "OK! ( {0}, {1} )\n", movePos[i].X, movePos[i].Y);
					moves.Add(movePos[i]);
				}
			}

			//Console.Write("End of Move Process\n");
			
			ClientPacketHandler.send_PCMoveTo(player, moves.ToArray());
		}

		/// <summary>
		/// Returns the X region id of a position
		/// </summary>
		/// <param name="x">x position</param>
		/// <returns></returns>
		public static uint GetRegionX(float x)
		{
			return (uint)x / RegionSize;
		}

		/// <summary>
		/// Returns the Y region id of a position
		/// </summary>
		/// <param name="y">y position</param>
		/// <returns></returns>
		public static uint GetRegionY(float y)
		{
			return (uint)y / RegionSize;
		}

		private static Region GetRegion(float fromX, float fromY)
		{
			uint rx = GetRegionX(fromX);
			uint ry = GetRegionY(fromY);
			return Regions[rx][ry];
		}

		internal static void UpdatePCPos(Player player, float curX, float curY, bool isLast)
		{
			uint newRX = GetRegionX(curX);
			uint newRY = GetRegionX(curY);
			
			player.Position.X = curX;
			player.Position.Y = curY;

			if (player.RegionX != newRX || player.RegionY != newRY)
			{
				OnRegionChange(player, newRX, newRY);
			}

			// TODO : Character should not be save when it stops walking
			//		  but in certain time intervals, this is a workaround
			if (isLast)
				player.Save();
		}

		private static void OnRegionChange(Player player, uint newRX, uint newRY)
		{
			Region r = Regions[newRX + (player.RegionX - newRX) * 1][newRY + (player.RegionY - newRY) * 1];

			if (r != null)
			{
				for (int i = 0; i < r.Npcs.Count; i++)
				{
					ClientPacketHandler.send_EntityAck(GObjectManager.Npcs[r.Npcs[i]]);
					ClientPacketHandler.send_EntityState(player, GObjectManager.Npcs[r.Npcs[i]].Handle, 0x0);
					ClientPacketHandler.send_Packet516(player, GObjectManager.Npcs[r.Npcs[i]].Handle);
				}
			}

			player.RegionX = newRX;
			player.RegionY = newRY;

			ClientPacketHandler.send_RegionAck(player, newRX, newRY);
		}

		internal static void AddMobToRegion(Monster mob)
		{
			GetRegion(mob.Position.X, mob.Position.Y).Monsters.Add(mob.Handle);

			ClientPacketHandler.send_EntityAck(mob);
		}
	}
}
