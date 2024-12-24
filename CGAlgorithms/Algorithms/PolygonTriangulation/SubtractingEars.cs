using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class SubtractingEars : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons,
                                 ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Extract unique vertices and ensure correct ordering
            HashSet<Point> uniqueVertices = new HashSet<Point>(lines.SelectMany(l => new[] { l.Start, l.End }));
            List<Point> remainingVertices = uniqueVertices.ToList();

            // Ensure vertices are ordered counterclockwise
            remainingVertices = EnsureCounterClockwise(remainingVertices);

            // List to store diagonals
            List<Line> diagonals = new List<Line>();

            while (remainingVertices.Count > 3)
            {
                bool earFound = false;

                for (int i = 0; i < remainingVertices.Count; i++)
                {
                    int prev = (i - 1 + remainingVertices.Count) % remainingVertices.Count;
                    int next = (i + 1) % remainingVertices.Count;

                    Point a = remainingVertices[prev];
                    Point b = remainingVertices[i];
                    Point c = remainingVertices[next];

                    // Check if the current triangle is convex
                    if (IsConvex(a, b, c))
                    {
                        // Check if any point lies inside the triangle
                        bool isEar = true;
                        for (int j = 0; j < remainingVertices.Count; j++)
                        {
                            if (j != prev && j != i && j != next &&
                                HelperMethods.PointInTriangle(remainingVertices[j], a, b, c) == Enums.PointInPolygon.Inside)
                            {
                                isEar = false;
                                break;
                            }
                        }

                        if (isEar)
                        {
                            // Add diagonal and remove the ear
                            diagonals.Add(new Line(a, c));
                            remainingVertices.RemoveAt(i);
                            earFound = true;
                            break;
                        }
                    }
                }

                if (!earFound)
                {
                    throw new InvalidOperationException("Polygon is not simple or cannot be triangulated.");
                }
            }

            // Add the final triangle edges
            for (int i = 0; i < remainingVertices.Count; i++)
            {
                int next = (i + 1) % remainingVertices.Count;
                outLines.Add(new Line(remainingVertices[i], remainingVertices[next]));
            }

            // Add diagonals to output
            outLines.AddRange(diagonals);
        }

        /// <summary>
        /// Checks if the angle formed by points a, b, and c is convex.
        /// </summary>
        private bool IsConvex(Point a, Point b, Point c)
        {
            return HelperMethods.CheckTurn(new Line(a, b), c) == Enums.TurnType.Left;
        }

        /// <summary>
        /// Ensures the vertices are ordered in counterclockwise direction.
        /// </summary>
        private List<Point> EnsureCounterClockwise(List<Point> vertices)
        {
            // Compute the polygon area to determine orientation
            double area = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                int next = (i + 1) % vertices.Count;
                area += (vertices[i].X * vertices[next].Y) - (vertices[next].X * vertices[i].Y);
            }

            // If the area is negative, the polygon is clockwise and needs reversing
            if (area < 0)
            {
                vertices.Reverse();
            }

            return vertices;
        }

        public override string ToString()
        {
            return "Subtracting Ears";
        }
    }
}
