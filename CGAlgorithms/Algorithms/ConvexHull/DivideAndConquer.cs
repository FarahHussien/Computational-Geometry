using CGAlgorithms;
using CGUtilities;
using System.Collections.Generic;
using System;
using System.Linq;

public class DivideAndConquer : Algorithm
{
    public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
    {
        // Step 1: Sort points by X and Y coordinates (using the pointXY method)
        pointXY(ref points);

        // Step 2: Divide points into two halves and recursively calculate the convex hull
        outPoints = divide(points);

        
    }

    private void pointXY(ref List<Point> points)
    {
        points = points.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();
    }

    public static int LargX(List<Point> points)
    {
        int x = 0;
        int i = 0;
        foreach (Point currentPoint in points)
        {
            if (currentPoint.X > points[x].X || (currentPoint.X == points[x].X && currentPoint.Y > points[x].Y))
            {
                x = i;
            }
            i++;
        }

        return x;
    }

    public static int SmlX(List<Point> points)
    {
        int i = 0;
        int x = 0;
        foreach (Point currentPoint in points)
        {
            if (currentPoint.X < points[x].X || (currentPoint.X == points[x].X && currentPoint.Y < points[x].Y))
            {
                x = i;
            }
            i++;
        }
        return x;
    }

    public List<Point> margesort(List<Point> Lift, List<Point> Right)
    {
        int MLP = LargX(Lift);
        int MRP = SmlX(Right);
        int Nleft = Lift.Count;
        int Nright = Right.Count;
        bool flg;
        int URP = MRP;
        int ULP = MLP;
        int oldright = (Nright + URP - 1) % Nright;
        int nextToleft = (ULP + 1) % Nleft;

        do
        {
            do
            {
                if (HelperMethods.CheckTurn(new Line(Right[URP], Lift[ULP]), Lift[nextToleft]) == Enums.TurnType.Right)
                {
                    ULP = nextToleft; nextToleft = (ULP + 1) % Nleft;
                    flg = false;
                }
                else
                {
                    flg = true;
                }

            } while (!flg);

            if (flg == true && (HelperMethods.CheckTurn(new Line(Right[URP], Lift[ULP]), Lift[nextToleft]) == Enums.TurnType.Colinear))
                ULP = nextToleft; 
            nextToleft = (ULP + 1) % Nleft;

            for (; HelperMethods.CheckTurn(new Line(Lift[ULP], Right[URP]), Right[oldright]) == Enums.TurnType.Left; URP = oldright, oldright = (Nright + URP - 1) % Nright)
            {
                flg = false;
            }

            if (flg == true && (HelperMethods.CheckTurn(new Line(Lift[ULP], Right[URP]), Right[oldright]) == Enums.TurnType.Colinear))
                URP = oldright; oldright = (Nright + URP - 1) % Nright;
        } while (flg == false);

        int DLP = MLP;
        int DRP = MRP;
        int oldleft = (Nleft + DLP - 1) % Nleft;
        int nextright = (DRP + 1) % Nright;

        do
        {
            flg = true;
            for (; HelperMethods.CheckTurn(new Line(Right[DRP], Lift[DLP]), Lift[oldleft]) == Enums.TurnType.Left; DLP = oldleft, oldleft = (Nleft + DLP - 1) % Nleft)
            {
                flg = false;
            }

            if (flg == true && (HelperMethods.CheckTurn(new Line(Right[DRP], Lift[DLP]), Lift[oldleft]) == Enums.TurnType.Colinear))
                DLP = oldleft; oldleft = (Nleft + DLP - 1) % Nleft;

            for (; HelperMethods.CheckTurn(new Line(Lift[DLP], Right[DRP]), Right[nextright]) == Enums.TurnType.Right; DRP = nextright, nextright = (DRP + 1) % Nright)
            {
                flg = false;
            }

            if (flg == true && (HelperMethods.CheckTurn(new Line(Lift[DLP], Right[DRP]), Right[nextright]) == Enums.TurnType.Colinear))
                DRP = nextright; nextright = (DRP + 1) % Nright;
        } while (flg == false);

        List<Point> new_points = new List<Point>();
        int ind = ULP;
        if (!new_points.Contains(Lift[ULP]))
            new_points.Add(Lift[ULP]);

        while (ind != DLP)
        {
            ind = (ind + 1) % Nleft;
            if (!new_points.Contains(Lift[ind]))
                new_points.Add(Lift[ind]);
        }

        ind = DRP;
        if (!new_points.Contains(Right[DRP]))
            new_points.Add(Right[DRP]);

        while (ind != URP)
        {
            ind = (ind + 1) % Nright;
            if (!new_points.Contains(Right[ind]))
                new_points.Add(Right[ind]);
        }

        return new_points;
    }

    public List<Point> divide(List<Point> Points)
    {
        if (Points.Count == 1)
        {
            return Points;
        }

        List<Point> righ = new List<Point>();
        List<Point> lif = new List<Point>();

        int i = 0;
        do
        {
            lif.Add(Points[i]);
            i++;
        } while (i < Points.Count / 2);

        i = Points.Count / 2;
        do
        {
            righ.Add(Points[i]);
            i++;
        } while (i < Points.Count);

        List<Point> Left = divide(lif);
        List<Point> Right = divide(righ);

        return margesort(Left, Right);
    }



    public override string ToString()
    {
        return "Convex Hull - Divide & Conquer";
    }

}
