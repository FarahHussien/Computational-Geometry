using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }

            Point maxY = points[0] , minY = points[0], minX = points[0], maxX = points[0];
            foreach (Point p in points)
            {
                if (p.X < minX.X)
                    minX = p;
                if (p.X > maxX.X)
                    maxX = p;
                if (p.Y > maxY.Y)
                    maxY = p;
                if (p.Y < minY.Y)
                    minY = p;

            }
            outPoints.Add(minX);
            outPoints.Add(maxX);
            outPoints.Add(maxY);                     
            Line lNE = new Line(maxX, maxY);           
            Line l = new Line(minX, maxX);       
            Line lNW = new Line(maxY, minX);

            List<Line> Lpoly = new List<Line>();
            Lpoly.Add(lNE);
            Lpoly.Add(l);
            Lpoly.Add(lNW);

            List<Point> pointRegon = new List<Point>();
            while (Lpoly.Count > 0)
            {                              
                Line currentL = Lpoly[Lpoly.Count - 1];
                Lpoly.RemoveAt(Lpoly.Count - 1);
                foreach (Point p in points)
                {
                    Enums.TurnType turn = HelperMethods.CheckTurn(currentL, p);
                    if (turn == Enums.TurnType.Right)
                        pointRegon.Add(p);
                }
                if (pointRegon.Count > 0)
                {
                    int maxDist = pointRegon
                    .Select((p, i) => new { Point = p, Index = i, Distance = FindDist(currentL, p) })
                    .Aggregate((max, current) => current.Distance > max.Distance ? current : max)
                    .Index;

                    Point farthestPoint = pointRegon[maxDist];
                    outPoints.Add(farthestPoint);
                    Lpoly.Add(new Line(currentL.Start, farthestPoint));
                    Lpoly.Add(new Line(farthestPoint, currentL.End));

                    pointRegon.Clear();
                }
                

            }

            for (int i = 0; i < outPoints.Count - 1; i++)
            {
                if (outPoints[i] == outPoints[i + 1])
                    outPoints.Remove(outPoints[i]);
            }


        }
        public static double FindDist(Line L, Point P)
        {
            Point pl1 = L.Start;
            Point pl2 = L.End;
            double a = Math.Abs((pl2.X - pl1.X) * (pl1.Y - P.Y) - (pl1.X - P.X) * (pl2.Y - pl1.Y));
            double b = Math.Sqrt(Math.Pow(pl2.X - pl1.X, 2) + Math.Pow(pl2.Y - pl1.Y, 2));
            double dis = a / b;
            return dis;

        }
        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
     
    }
}
