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
	public class Point
	{
		public float X { get; set; }
		public float Y { get; set; }
		public Point()
		{
			this.X = 0;
			this.Y = 0;
		}
	}

	public class Region
	{
		
	}

	public static class RegionMngr
	{
		public const int RegionSize = 180;	// Start at m000_000
		public const int MapSize = 16128;	// The size of one map in one dimension

		private static Region[][] Regions;

		public static void Start()
		{
			Regions = new Region[Settings.MapLengthX * (MapSize / RegionSize)][];
			/*for (int i = 0; i < Regions.Length; i++)
			{
				Regions[i] = new Region[Settings.MapLengthY * (MapSize / RegionSize)];
				for (int j = 0; j < Regions[i].Length; j++)
					Regions[i][j] = new Region();
			}*/
		}

		internal static void PcWalkCheck(Player player, float startX, float startY, Point[] movePos)
		{
			ClientPacketHandler.send_PCMoveTo(player, movePos);
		}

		internal static void UpdatePCPos(Player player, float curX, float curY, bool isLast)
		{
			player.Chara.Position.X = curX;
			player.Chara.Position.Y = curY;

			// TODO : Character should not be save when it stops walking
			//		  but in certain time intervals, this is a workaround
			if (isLast)
				player.Chara.Save();
		}
	}
}
