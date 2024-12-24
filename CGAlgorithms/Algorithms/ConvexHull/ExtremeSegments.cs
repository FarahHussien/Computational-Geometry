using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        private class AngularDistanceComparer
        {
            public Point OriginPoint { get; private set; }

            public AngularDistanceComparer(Point origin)
            {
                OriginPoint = origin;
            }

            public int ComparePolarAngles(Point p1, Point p2)
            {
                double angle1 = CalculateAngularDifference(OriginPoint, p1);
                double angle2 = CalculateAngularDifference(OriginPoint, p2);

                if (Math.Abs(angle1 - angle2) > 1e-9)
                    return angle1.CompareTo(angle2);

                return CalculateSpatialDistance(OriginPoint, p1).CompareTo(CalculateSpatialDistance(OriginPoint, p2));
            }

            private double CalculateAngularDifference(Point start, Point end)
            {
                return Math.Atan2(end.Y - start.Y, end.X - start.X);
            }

            private double CalculateSpatialDistance(Point p1, Point p2)
            {
                return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            }
        }

        private enum RotationDirection
        {
            Clockwise = -1,
            CounterClockwise = 1,
            Collinear = 0
        }

        private RotationDirection DetermineTurnDirection(Point p1, Point p2, Point p3)
        {
            long crossProd = CalculateCrossProduct(p1, p2, p3);

            if (Math.Abs(crossProd) < 1e-9)
                return RotationDirection.Collinear;

            return crossProd > 0 ? RotationDirection.CounterClockwise : RotationDirection.Clockwise;
        }

        private long CalculateCrossProduct(Point p1, Point p2, Point p3)
        {
            return (long)((p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X));
        }

        private List<Point> EliminateRedundantPoints(List<Point> inputSet)
        {
            var visitedPoints = new HashSet<(double, double)>();
            var distinctPoints = new List<Point>();

            foreach (var point in inputSet)
            {
                var pointKey = (point.X, point.Y);
                if (visitedPoints.Add(pointKey))
                {
                    distinctPoints.Add(point);
                }
            }

            return distinctPoints;
        }

        public override void Run(List<Point> inputPoints, List<Line> inputLines, List<Polygon> inputPolygons, ref List<Point> outputPoints, ref List<Line> outputLines, ref List<Polygon> outputPolygons)
        {
            // Complex redundancy elimination step
            inputPoints = EliminateRedundantPoints(inputPoints);

            if (inputPoints.Count < 3)
            {
                outputPoints = inputPoints;
                return;
            }

            // Selecting the lowest and leftmost point as pivot
            Point basePivot = inputPoints
                .OrderBy(p => p.Y)
                .ThenBy(p => p.X)
                .First();

            // Creating the comparer for angular sorting
            var polarComparer = new AngularDistanceComparer(basePivot);
            var sortedPoints = inputPoints
                .Where(p => p != basePivot)
                .OrderBy(p => p, Comparer<Point>.Create((a, b) => polarComparer.ComparePolarAngles(a, b)))
                .ToList();

            // Convex hull construction with a more complex logic
            var hullPoints = new List<Point> { basePivot };

            foreach (var currentPoint in sortedPoints)
            {
                while (hullPoints.Count >= 2)
                {
                    Point topPoint = hullPoints[hullPoints.Count - 1];
                    Point secondTopPoint = hullPoints[hullPoints.Count - 2];

                    if (DetermineTurnDirection(secondTopPoint, topPoint, currentPoint) == RotationDirection.CounterClockwise)
                        break;

                    hullPoints.RemoveAt(hullPoints.Count - 1);
                }

                hullPoints.Add(currentPoint);
            }

            outputPoints = hullPoints;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments (Advanced and Complex)";
        }
    }
}
