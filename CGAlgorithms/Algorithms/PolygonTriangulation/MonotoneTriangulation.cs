using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation : Algorithm
    {
        public override void Run(System.Collections.Generic.List<CGUtilities.Point> points, System.Collections.Generic.List<CGUtilities.Line> lines, System.Collections.Generic.List<CGUtilities.Polygon> polygons, ref System.Collections.Generic.List<CGUtilities.Point> outPoints, ref System.Collections.Generic.List<CGUtilities.Line> outLines, ref System.Collections.Generic.List<CGUtilities.Polygon> outPolygons)
        {
            Polygon polygon = new Polygon(lines);
            List<Point> vertices = new List<Point>();
            Stack<Point> stack = new Stack<Point>();

            polygon = CheckCCW(polygon);
            bool isMonotonePolygon = CheckMonotone(polygon);

            if (!isMonotonePolygon) return;

            for (int i = 0; i < polygon.lines.Count; i++)
                points.Add(polygon.lines[i].Start);

            for (int i = 0; i < polygon.lines.Count; i++)
                vertices.Add(polygon.lines[i].Start);

            vertices.Sort(point_sort); // sort points on max Y and max X on tie

            if(vertices.Count < 3)
            {
                outPoints.Clear();
                Console.ReadKey();  
                return;
            }

            Point first_point = vertices[0];
            Point second_point = vertices[1];
            stack.Push(first_point);
            stack.Push(second_point);

            int start_indx = 2;

            while (start_indx != polygon.lines.Count)
            {
                Point current_point = vertices[start_indx];
                Point top = stack.Peek(); // get top of  stack without removing it
                                          
                bool same_side = false;
                if (current_point.X < vertices[0].X && top.X < vertices[0].X)
                    same_side = true;
                else if (current_point.X > vertices[0].X && top.X > vertices[0].X)
                    same_side = true;

                // P and Top on the same side 
                if (same_side == true)
                {
                    stack.Pop();
                    Point top2 = stack.Peek();

                    //Check the top point is convex or not
                    int index = points.IndexOf(top);
                    if (IsConvex(polygon, index) == true)
                    {
                        outLines.Add(new Line(current_point, top2));
                        if (stack.Count == 1)
                        {
                            stack.Push(current_point);
                            start_indx++;
                        }
                    }
                    else
                    {
                        stack.Push(top);
                        stack.Push(current_point);
                        start_indx++;
                    }
                }
                //P and Top on different side 
                else
                {
                    while (stack.Count != 1)
                    {
                        Point top2 = stack.Pop();
                        outLines.Add(new Line(current_point, top2));
                    }
                    stack.Pop();
                    stack.Push(top);
                    stack.Push(current_point);
                }
            }
        }

        public Polygon CheckCCW(Polygon polygon)
        {
            double signed_area = 0;
            foreach (var line in polygon.lines)
            {
                signed_area += (line.End.X - line.Start.X) * (line.End.Y + line.Start.Y);
            }
            signed_area /= 2;

            // If the polygon is clockwise, reverse it to make it counterclockwise
            if (signed_area > 0)
            {
                polygon.lines.Reverse();
                foreach (var line in polygon.lines)
                {
                    // Swap the start and end points of each line
                    (line.Start, line.End) = (line.End, line.Start);
                }
            }

            return polygon;
        }

        public bool CheckMonotone(Polygon polygon)
        {
            int nonMonotoneCount = 0;

            for (int currentIndex = 0; currentIndex < polygon.lines.Count; currentIndex++)
            {
                int previousIndex = ((currentIndex - 1) + polygon.lines.Count) % polygon.lines.Count;
                int nextIndex = (currentIndex + 1) % polygon.lines.Count;

                Point currentPoint = polygon.lines[currentIndex].Start;
                Point previousPoint = polygon.lines[previousIndex].Start;
                Point nextPoint = polygon.lines[nextIndex].Start;

                // Check if the point is a cusp and whether the polygon violates monotonicity
                bool isCuspPoint = !IsConvex(polygon, currentIndex);
                bool isMonotoneIncreasing = nextPoint.Y > currentPoint.Y && previousPoint.Y > currentPoint.Y;
                bool isMonotoneDecreasing = nextPoint.Y < currentPoint.Y && previousPoint.Y < currentPoint.Y;

                // Count points that are cusps and violate the monotonicity condition
                if ((isMonotoneIncreasing || isMonotoneDecreasing) && isCuspPoint)
                {
                    nonMonotoneCount++;
                }
            }

            // Return true if no cusp points violate monotonicity, otherwise return false
            return nonMonotoneCount == 0;
        }

        public bool IsConvex(Polygon p, int Current)
        {
            int previous = ((Current - 1) + p.lines.Count) % p.lines.Count;
            int next = (Current + 1) % p.lines.Count;

            Point p1 = p.lines[previous].Start;
            Point p2 = p.lines[Current].Start;
            Point p3 = p.lines[next].Start;
            Line l = new Line(p1, p2);
            if (HelperMethods.CheckTurn(l, p3) == Enums.TurnType.Left)
                return true;
            return false;
        }

        public static int point_sort(Point a, Point b)
        {
            if (a.Y == b.Y)
                return -a.X.CompareTo(b.X);
            else
                return -a.Y.CompareTo(b.Y);
        }

        public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}