// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common
{
	public class Point
	{
		public float X { get; set; }
		public float Y { get; set; }
		
		public Point(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}

		public Point()
		{
			this.X = 0;
			this.Y = 0;
		}
	}

	// Adapted from: http://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
	public class Geometry
	{

		// Given three colinear points p, q, r, the function checks if
		// point q lies on line segment 'pr'
		public static bool OnSegment(Point p, Point q, Point r)
		{
			if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
				 q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
				return true;

			return false;
		}

		// To find Orientation of ordered triplet (p, q, r).
		// The function returns following values
		// 0 --> p, q and r are colinear
		// 1 --> Clockwise
		// 2 --> Counterclockwise
		public static int Orientation(Point p, Point q, Point r)
		{
			// See 10th slides from following link for derivation of the formula
			// http://www.dcs.gla.ac.uk/~pat/52233/slides/Geometry1x1.pdf
			float val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

			if (val == 0) return 0;  // colinear

			return (val > 0) ? 1 : 2; // clock or counterclock wise
		}

		// The main function that returns true if line segment 'p1q1'
		// and 'p2q2' intersect.
		public static bool DoIntersect(Point p1, Point q1, Point p2, Point q2)
		{
			// Find the four Orientations needed for general and
			// special cases
			int o1 = Orientation(p1, q1, p2);
			int o2 = Orientation(p1, q1, q2);
			int o3 = Orientation(p2, q2, p1);
			int o4 = Orientation(p2, q2, q1);

			// General case
			if (o1 != o2 && o3 != o4)
				return true;

			// Special Cases
			// p1, q1 and p2 are colinear and p2 lies on segment p1q1
			if (o1 == 0 && OnSegment(p1, p2, q1)) return true;

			// p1, q1 and p2 are colinear and q2 lies on segment p1q1
			if (o2 == 0 && OnSegment(p1, q2, q1)) return true;

			// p2, q2 and p1 are colinear and p1 lies on segment p2q2
			if (o3 == 0 && OnSegment(p2, p1, q2)) return true;

			// p2, q2 and q1 are colinear and q1 lies on segment p2q2
			if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

			return false; // Doesn't fall in any of the above cases
		}
	}
}
