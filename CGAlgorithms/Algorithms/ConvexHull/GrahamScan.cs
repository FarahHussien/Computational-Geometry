using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public override void Run(List<Point> inputPoints, List<Line> inputLines, List<Polygon> inputPolygons, ref List<Point> outputPoints, ref List<Line> outputLines, ref List<Polygon> outputPolygons)
        {
            // Remove duplicate 
            inputPoints = inputPoints.Distinct().ToList();

            if (inputPoints.Count < 3)
            {
                outputPoints = inputPoints;
                return;
            }
            // get first extreme point
            int pivotPointIndex = 0;
            for (int idx = 1; idx < inputPoints.Count; idx++)
                if (inputPoints[idx].Y < inputPoints[pivotPointIndex].Y ||
                    (inputPoints[idx].Y == inputPoints[pivotPointIndex].Y && inputPoints[idx].X < inputPoints[pivotPointIndex].X))
                    pivotPointIndex = idx;

            // Swap 
            // easier reference during the sorting phase.
            Point pivotReference = inputPoints[pivotPointIndex];
            inputPoints[pivotPointIndex] = inputPoints[0];
            inputPoints[0] = pivotReference;

            // Sort points                   // with angle
            inputPoints = inputPoints.OrderBy(p => Math.Atan2(p.Y - pivotReference.Y, p.X - pivotReference.X))
                                     //if have same angle , points sorted by their distance from the pivot point.
                                     .ThenBy(p => Math.Sqrt(Math.Pow(p.X - pivotReference.X, 2) + Math.Pow(p.Y - pivotReference.Y, 2)))
                                     .ToList();

            List<Point> hullStack = new List<Point> { inputPoints[0], inputPoints[1] };

            for (int idx = 2; idx < inputPoints.Count; idx++)
            {
                hullStack.Add(inputPoints[idx]);

                // Remove points do not make a left turn
                while (hullStack.Count > 2)
                {
                    Point topMost = hullStack[hullStack.Count - 3];
                    Point secondTopMost = hullStack[hullStack.Count - 2];
                    Point current = hullStack[hullStack.Count - 1];

                    Line hullLine = new Line(topMost, secondTopMost);
                    if (HelperMethods.CheckTurn(hullLine, current) != Enums.TurnType.Left)
                        hullStack.RemoveAt(hullStack.Count - 2); 
                    else
                        break;
                }
            }

            // Remove collinear points
            // A(0,0), B(1,1) , C (2,2)
            List<Point> refinedHull = new List<Point>();
            for (int idx = 0; idx < hullStack.Count; idx++)
            {
                Point previousPoint = hullStack[(idx - 1 + hullStack.Count) % hullStack.Count];
                Point currentPoint = hullStack[idx];
                Point nextPoint = hullStack[(idx + 1) % hullStack.Count];

                Line boundaryLine = new Line(previousPoint, nextPoint);
                var turnDirection = HelperMethods.CheckTurn(boundaryLine, currentPoint);
                if (turnDirection == Enums.TurnType.Left || turnDirection == Enums.TurnType.Right)
                    refinedHull.Add(currentPoint); //  add points contributing to the convex hull
            }

            outputPoints = refinedHull;
        }

        public override string ToString()
        {
            return "Convex Hull - Advanced Graham Scan (Obfuscated)";
        }
    }
}