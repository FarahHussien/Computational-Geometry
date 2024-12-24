using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outpoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int i = 0;
            while (i < points.Count)
            {
                bool flg = false;

                foreach (Point j in points)
                {
                    foreach (Point k in points)
                    {

                        foreach (Point l in points)
                        {
                            if (j != points[i] && k != points[i] && l != points[i])
                            {
                                Enums.PointInPolygon Inside = HelperMethods.PointInTriangle(points[i], j, k, l);

                                if (Inside.Equals(Enums.PointInPolygon.Inside) || Inside.Equals(Enums.PointInPolygon.OnEdge))
                                {
                                    points.RemoveAt(i);
                                    flg = true;
                                    break;
                                }
                            }
                        }
                        if (flg)
                            break;
                    }
                    if (flg)
                        break;
                }

                if (flg)
                    i--;
                
                i++;
            }
            outpoints = points;
        }


        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}