using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }
            points.Sort(comp());
            int i = 0;
            while (i < points.Count - 1)
            {
                if (points[i].Equals(points[i + 1]))
                    points.RemoveAt(i);

                else
                    i++;
                
            }

            nlgnImp(points, outPoints);

        }


        private void nlgnImp(List<Point> points, List<Point> outPoints)
        {
            int[] n0xt = new int[points.Count];
            int[] pr0v = new int[points.Count];
            n0xt[0] = 1;
            n0xt[1] = 0;
            pr0v[0] = 1;
            pr0v[1] = 0;

            int i = 2;
            while (i < points.Count)
            {
                if (points[i].Y > points[i - 1].Y)
                {
                    n0xt[i] = i - 1;
                    pr0v[i] = pr0v[i - 1];
                }
                else
                {
                    n0xt[i] = n0xt[i - 1];
                    pr0v[i] = i - 1;
                }
                n0xt[pr0v[i]] = i;
                pr0v[n0xt[i]] = i;

                while (CheckCW(i, pr0v[i], pr0v[pr0v[i]], points))
                {
                    n0xt[pr0v[pr0v[i]]] = i;
                    pr0v[i] = pr0v[pr0v[i]];
                }

                while (CheckCW(n0xt[n0xt[i]], n0xt[i], i, points))
                {
                    pr0v[n0xt[n0xt[i]]] = i;
                    n0xt[i] = n0xt[n0xt[i]];
                }

                i++;
            }

            outPoints.Add(points[0]);


            for (int k = n0xt[0]; k != 0 && outPoints.Count <= points.Count + 1; k = n0xt[k])
            {
                outPoints.Add(points[k]);
            }
        }


        private bool CheckCW(int i, int v1, int v2, List<Point> points)
        {
            int j = i;
            while (j < v1)
            {
                if (!IsLeftTurn(points[i], points[j], points[v2]))
                {
                    return false;
                }
                j++;
            }

            j = v1;
            while (j < v2)
            {
                if (!IsLeftTurn(points[i], points[j], points[v2]))
                {
                    return false;
                }
                j++;
            }

            j = v2;
            while (j < i)
            {
                if (!IsLeftTurn(points[i], points[j], points[v2]))
                {
                    return false;
                }
                j++;
            }

            return true;
        }
        
        private int Orientation(Point a, Point b, Point c)
        {
            var vab = HelperMethods.GetVector(new Line(a, b));
            var vbc = HelperMethods.GetVector(new Line(b, c));
            var res = HelperMethods.CrossProduct(vab, vbc);

            return Math.Sign(res);
        }


        private bool IsTurn(Point a, Point b, Point c, int direction)
        {
            return Orientation(a, b, c) * direction <= 0;
        }

        private bool IsLeftTurn(Point a, Point b, Point c)
        {
            return IsTurn(a, b, c, 1);
        }

        private bool IsRightTurn(Point a, Point b, Point c)
        {
            return IsTurn(a, b, c, -1);
        }

        private static Comparison<Point> comp()
        {
            return (p0int1, p0int2) =>
            {
                int xComparison = p0int1.X.CompareTo(p0int2.X);
                return xComparison != 0 ? xComparison : p0int1.Y.CompareTo(p0int2.Y);
            };
        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }

    }
}