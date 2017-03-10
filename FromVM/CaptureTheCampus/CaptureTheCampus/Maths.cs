using Android.Content;
using Android.Gms.Maps.Model;
using Android.Util;
using System;
using System.Collections.Generic;

namespace CaptureTheCampus
{
    public class Maths
    {
        private GameActivity gameActivity;

        public Maths(Context context)
        {
            Log.Info("Maths", "Maths built");

            gameActivity = (GameActivity)context;
        }

        public bool CheckPosition()
        {
            // Get the angle between the point and the
            // first and last vertices.
            double total_angle = GetAngle(gameActivity.playArea.vertices.Last.Value.Latitude, gameActivity.playArea.vertices.Last.Value.Longitude, gameActivity.path.currentPosition.Latitude, gameActivity.path.currentPosition.Longitude, gameActivity.playArea.vertices.First.Value.Latitude, gameActivity.playArea.vertices.First.Value.Longitude);

            // Add the angles from the point
            // to each other pair of vertices.
            gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First.Next;

            while (true)
            {
                total_angle += GetAngle(gameActivity.playArea.verticesNode.Previous.Value.Latitude, gameActivity.playArea.verticesNode.Previous.Value.Longitude, gameActivity.path.currentPosition.Latitude, gameActivity.path.currentPosition.Longitude, gameActivity.playArea.verticesNode.Value.Latitude, gameActivity.playArea.verticesNode.Value.Longitude);

                if(gameActivity.playArea.verticesNode.Next != null)
                {
                    gameActivity.playArea.verticesNode = gameActivity.playArea.verticesNode.Next;
                }
                else
                {
                    break;
                }
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }

        private double GetAngle(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the dot product.
            double dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            double cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return Math.Atan2(cross_product, dot_product);
        }

        private double DotProduct(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }

        private double CrossProductLength(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        // The main function that returns true if line segment 'p1q1'
        // and 'p2q2' intersect.
        public bool doIntersect(LatLng p1, LatLng q1, LatLng p2, LatLng q2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
            {
                return true;
            }

            // Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1))
            {
                return true;
            }

            // p1, q1 and p2 are colinear and q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1))
            {
                return true;
            }

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2))
            {
                return true;
            }

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2))
            {
                return true;
            }

            return false; // Doesn't fall in any of the above cases
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are colinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        private int orientation(LatLng p, LatLng q, LatLng r)
        {
            // See http://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            double val = (q.Longitude - p.Longitude) * (r.Latitude - q.Latitude) - (q.Latitude - p.Latitude) * (r.Longitude - q.Longitude);

            if (val == 0)
            {
                return 0;  // colinear
            }

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // Given three colinear points p, q, r, the function checks if
        // point q lies on line segment 'pr'
        private bool onSegment(LatLng p, LatLng q, LatLng r)
        {
            if (q.Latitude <= Math.Max(p.Latitude, r.Latitude) && q.Latitude >= Math.Min(p.Latitude, r.Latitude) && q.Longitude <= Math.Max(p.Longitude, r.Longitude) && q.Longitude >= Math.Min(p.Longitude, r.Longitude))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public LatLng LineIntersectionPoint(LatLng ps1, LatLng pe1, LatLng ps2, LatLng pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            double A1 = pe1.Longitude - ps1.Longitude;
            double B1 = ps1.Latitude - pe1.Latitude;
            double C1 = A1 * ps1.Latitude + B1 * ps1.Longitude;

            // Get A,B,C of second line - points : ps2 to pe2
            double A2 = pe2.Longitude - ps2.Longitude;
            double B2 = ps2.Latitude - pe2.Latitude;
            double C2 = A2 * ps2.Latitude + B2 * ps2.Longitude;

            // Get delta and check if the lines are parallel
            double delta = A1 * B2 - A2 * B1;

            if (delta == 0)
            {
                throw new Exception("Lines are parallel");
            }

            // now return the Vector2 intersection point
            return new LatLng((B2 * C1 - B1 * C2) / delta, (A1 * C2 - A2 * C1) / delta);
        }

        public double PolygonArea(LinkedList<LatLng> vertices)
        {
            // Return the absolute value of the signed area.
            // The signed area is negative if the polyogn is
            // oriented clockwise.

            return Math.Abs(SignedPolygonArea(vertices));
        }

        private double SignedPolygonArea(LinkedList<LatLng> vertices)
        {
            // Add the first point to the end.
            int num_points = vertices.Count;
            LatLng[] pts = new LatLng[num_points + 1];
            vertices.CopyTo(pts, 0);
            pts[num_points] = vertices.First.Value;

            // Get the areas.
            double area = 0;

            for (int i = 0; i < num_points; i++)
            {
                area += (pts[i + 1].Latitude - pts[i].Latitude) * (pts[i + 1].Longitude + pts[i].Longitude) / 2;
            }

            // Return the result.
            return area;
        }

        public double ShortestLineSegment(LinkedList<LatLng> vertices)
        {
            double shortestLength, temporaryShortestLength;

            LinkedListNode<LatLng> verticesNode = vertices.First.Next;

            shortestLength = LineSegmentLength(verticesNode.Previous.Value, verticesNode.Value);

            verticesNode = verticesNode.Next;

            while (true)
            {
                temporaryShortestLength = LineSegmentLength(verticesNode.Previous.Value, verticesNode.Value);

                if (temporaryShortestLength < shortestLength)
                {
                    shortestLength = temporaryShortestLength;
                }

                if(verticesNode.Next != null)
                {
                    verticesNode = verticesNode.Next;
                }
                else
                {
                    temporaryShortestLength = LineSegmentLength(vertices.First.Value, vertices.Last.Value);

                    if (temporaryShortestLength < shortestLength)
                    {
                        shortestLength = temporaryShortestLength;
                    }
                    
                    break;
                }
            }

            return shortestLength;
        }

        private double LineSegmentLength(LatLng firstPoint, LatLng secondPoint)
        {
            return Math.Sqrt(SquareLineSegmentLength(firstPoint, secondPoint));
        }

        private double SquareLineSegmentLength(LatLng firstPoint, LatLng secondPoint)
        {
            return Math.Abs(SignedSquareLineSegmentLength(firstPoint, secondPoint));
        }

        private double SignedSquareLineSegmentLength(LatLng firstPoint, LatLng secondPoint)
        {
            return Math.Pow((firstPoint.Latitude - secondPoint.Latitude), 2) + Math.Pow((firstPoint.Longitude - secondPoint.Longitude), 2);
        }

        // Find the polygon's centroid.
        public LatLng FindCentroid()
        {
            // Add the first point to the end.
            int num_points = gameActivity.playArea.vertices.Count;
            LatLng[] pts = new LatLng[num_points + 1];
            gameActivity.playArea.vertices.CopyTo(pts, 0);
            pts[num_points] = gameActivity.playArea.vertices.First.Value;

            // Find the centroid.
            double X = 0;
            double Y = 0;
            double second_factor;

            for (int i = 0; i < num_points; i++)
            {
                second_factor = pts[i].Latitude * pts[i + 1].Longitude - pts[i + 1].Latitude * pts[i].Longitude;
                X += (pts[i].Latitude + pts[i + 1].Latitude) * second_factor;
                Y += (pts[i].Longitude + pts[i + 1].Longitude) * second_factor;
            }

            // Divide by 6 times the polygon's area.
            double polygon_area = PolygonArea(gameActivity.playArea.vertices);
            X /= (6 * polygon_area);
            Y /= (6 * polygon_area);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (X < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new LatLng(X, Y);
        }

        public LatLng ExtendLineSegment(LatLng firstPoint, LatLng secondPoint)
        {
            double lenAB = Math.Sqrt(Math.Pow(secondPoint.Latitude - firstPoint.Latitude, 2.0) + Math.Pow(secondPoint.Longitude - firstPoint.Longitude, 2.0));

            return new LatLng(firstPoint.Latitude + (firstPoint.Latitude - secondPoint.Latitude) / lenAB * (ShortestLineSegment(gameActivity.playArea.vertices) / 1000), firstPoint.Longitude + (firstPoint.Longitude - secondPoint.Longitude) / lenAB * (ShortestLineSegment(gameActivity.playArea.vertices) / 1000));
        }
    }
}