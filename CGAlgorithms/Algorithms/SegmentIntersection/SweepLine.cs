using System;
using CGUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms;
using System.Runtime.InteropServices;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    class EventPoint
    {
        public Point P;
        public string EventType;
        public int index;
    }

    class SweepLine : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons,
                           ref List<Point> outPoints, ref List<Line> outLines,
                           ref List<Polygon> outPolygons)
        {
            // Ensure all line segments are correctly oriented.
            foreach (Line line in lines)
            {
                if (line.End.X < line.Start.X)
                {
                    Point temp = line.Start;
                    line.Start = line.End;
                    line.End = temp;
                }
            }

            // Check every pair of lines for intersections
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    Point intersection;
                    bool hasIntersection = FindIntersection(lines[i], lines[j], out intersection);

                    // If an intersection is found, add it to the output list
                    if (hasIntersection)
                    {
                        // Check if the point is already in outPoints to avoid duplicates
                        if (!outPoints.Any(p => p.X == intersection.X && p.Y == intersection.Y))
                        {
                            outPoints.Add(intersection);
                        }
                    }
                }
            }
        }

        // Find intersection point of two line segments
        private bool FindIntersection(Line line1, Line line2, out Point intersection)
        {
            double x1 = line1.Start.X, y1 = line1.Start.Y;
            double x2 = line1.End.X, y2 = line1.End.Y;

            double x3 = line2.Start.X, y3 = line2.Start.Y;
            double x4 = line2.End.X, y4 = line2.End.Y;

            double denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            // Lines are parallel or coincident
            if (denominator == 0)
            {
                intersection = new Point(0, 0);
                return false;
            }

            double px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denominator;
            double py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denominator;

            intersection = new Point(px, py);

            // Check if the intersection point is on both line segments
            return IsPointOnLineSegment(intersection, line1) && IsPointOnLineSegment(intersection, line2);
        }

        // Check if a point lies on a line segment
        private bool IsPointOnLineSegment(Point point, Line line)
        {
            double minX = Math.Min(line.Start.X, line.End.X), maxX = Math.Max(line.Start.X, line.End.X);
            double minY = Math.Min(line.Start.Y, line.End.Y), maxY = Math.Max(line.Start.Y, line.End.Y);

            return point.X >= minX && point.X <= maxX &&
                   point.Y >= minY && point.Y <= maxY;
        }

        
        public static bool IsPointOnLineSegment(Line line, Point point)
        {
            return Math.Min(line.Start.X, line.End.X) <= point.X && point.X <= Math.Max(line.Start.X, line.End.X)
                && Math.Min(line.Start.Y, line.End.Y) <= point.Y && point.Y <= Math.Max(line.Start.Y, line.End.Y);
        }

        public override string ToString()
        {
            return "Sweep Line Intersection Points";
        }
    }
}
