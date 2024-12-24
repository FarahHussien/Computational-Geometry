using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    // JarvisMarch: compute convex hull of a set of points in 2D space
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            outPoints = new List<Point> ();
            Point origPoint = new Point(0, 0); // cache original point for special case triangle
            
            // awl 2 cases
            if (points.Count < 3) // single point or line "2 points"
            {
                outPoints = points;
            }
            else
            {
                int down = 0; // index of the bottommost "y-axis"
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].Y < points[down].Y || (points[i].Y == points[down].Y && points[i].X < points[down].X))
                    {
                        down = i; // smallest y 
                        //break;
                    }
                        
                }

                int next;
                int currPointIndx = down; // index of smallest y to start with

                for (; currPointIndx != down || outPoints.Count == 0;)
                {
                    outPoints.Add(points[currPointIndx]); // add it to l
                    next = GetNextPointIndex(currPointIndx, points.Count); // get index of next point to check if it makes with cur one extreme segment

                    // search most ccw point
                    for (int i = 0; i < points.Count; i++)
                    {
                        int ret = OrientationTest(points[currPointIndx], points[i], points[next]);
                        if (ret == 1) // ccw
                        {
                            next = i;
                        }
                    }

                    currPointIndx = next; // to start with and get from the next one the make with extreme segment
                }

                List<Point> collinearPoints = new List<Point>();
                for (int i = 0; i < outPoints.Count - 2; i++)
                {
                    int ret = OrientationTest(outPoints[i], outPoints[i + 1], outPoints[i + 2]);
                    if (ret == 0)
                    {
                        collinearPoints.Add(outPoints[i + 1]);
                    }
                }

                RemoveCollinearFromOutPoints(ref outPoints, collinearPoints);

                // jarvis special case triangle
                if (outPoints.Contains(origPoint))
                {
                    outPoints.Remove(origPoint);
                }
            }
        }

        // some helper functions

        // to get index of next point to check if it makes with cur extreme segment
        public static int GetNextPointIndex(int currentPoint, int totalPoints)
        {
            return (currentPoint + 1) % totalPoints;  // ensrue circular indexing
        }
        public static int OrientationTest(Point p, Point q, Point r)
        {
            int signed_area = (int)((q.X - p.X) * (r.Y - q.Y) - (q.Y - p.Y) * (r.X - q.X)); // signed area of ^pqr^
            if (signed_area == 0) return 0;      // points are colinear => on same line
            return (signed_area > 0) ? 1 : 2;   // either ccw or cw => 1 means ccw
        }

        public void RemoveCollinearFromOutPoints(ref List<Point> outPoints, List<Point> collinearPoints)
        {
            foreach (var point in collinearPoints)
            {
                outPoints.Remove(point);
            }
        }
        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
