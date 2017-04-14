using Android.Content;
using Android.Gms.Maps.Model;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CaptureTheCampus
{
    public class Maths
    {
        public struct Triangle
        {
            public LatLng[] vertices;
        }

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
            double total_angle = GetAngle(gameActivity.playArea.vertices.Last.Value.Latitude, gameActivity.playArea.vertices.Last.Value.Longitude, gameActivity.player[gameActivity.playerPosition].currentPosition.Latitude, gameActivity.player[gameActivity.playerPosition].currentPosition.Longitude, gameActivity.playArea.vertices.First.Value.Latitude, gameActivity.playArea.vertices.First.Value.Longitude);

            // Add the angles from the point
            // to each other pair of vertices.
            gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First.Next;

            while (true)
            {
                total_angle += GetAngle(gameActivity.playArea.verticesNode.Previous.Value.Latitude, gameActivity.playArea.verticesNode.Previous.Value.Longitude, gameActivity.player[gameActivity.playerPosition].currentPosition.Latitude, gameActivity.player[gameActivity.playerPosition].currentPosition.Longitude, gameActivity.playArea.verticesNode.Value.Latitude, gameActivity.playArea.verticesNode.Value.Longitude);

                if (gameActivity.playArea.verticesNode.Next != null)
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

        private double GetAngle(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            // Get the dot product.
            double dot_product = DotProductThree(aX, aY, bX, bY, cX, cY);

            // Get the cross product.
            double cross_product = CrossProductLength(aX, aY, bX, bY, cX, cY);

            // Calculate the angle.
            return Math.Atan2(cross_product, dot_product);
        }

        private double DotProductThree(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
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
        public bool DoIntersect(LatLng p1, LatLng q1, LatLng p2, LatLng q2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
            {
                return true;
            }

            // Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if (o1 == 0 && OnSegment(p1, p2, q1))
            {
                return true;
            }

            // p1, q1 and p2 are colinear and q2 lies on segment p1q1
            if (o2 == 0 && OnSegment(p1, q2, q1))
            {
                return true;
            }

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if (o3 == 0 && OnSegment(p2, p1, q2))
            {
                return true;
            }

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if (o4 == 0 && OnSegment(p2, q1, q2))
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
        private int Orientation(LatLng p, LatLng q, LatLng r)
        {
            double val = (q.Longitude - p.Longitude) * (r.Latitude - q.Latitude) - (q.Latitude - p.Latitude) * (r.Longitude - q.Longitude);

            if (val == 0)
            {
                return 0;  // colinear
            }

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // Given three colinear points p, q, r, the function checks if
        // point q lies on line segment 'pr'
        private bool OnSegment(LatLng p, LatLng q, LatLng r)
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
            double a1 = pe1.Longitude - ps1.Longitude;
            double b1 = ps1.Latitude - pe1.Latitude;
            double c1 = a1 * ps1.Latitude + b1 * ps1.Longitude;

            // Get A,B,C of second line - points : ps2 to pe2
            double a2 = pe2.Longitude - ps2.Longitude;
            double b2 = ps2.Latitude - pe2.Latitude;
            double c2 = a2 * ps2.Latitude + b2 * ps2.Longitude;

            // Get delta and check if the lines are parallel
            double delta = a1 * b2 - a2 * b1;

            if (delta == 0)
            {
                throw new Exception("Lines are parallel");
            }

            // now return the Vector2 intersection point
            return new LatLng((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
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

                if (verticesNode.Next != null)
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
            double x = 0;
            double y = 0;
            double second_factor;

            for (int i = 0; i < num_points; i++)
            {
                second_factor = pts[i].Latitude * pts[i + 1].Longitude - pts[i + 1].Latitude * pts[i].Longitude;
                x += (pts[i].Latitude + pts[i + 1].Latitude) * second_factor;
                y += (pts[i].Longitude + pts[i + 1].Longitude) * second_factor;
            }

            // Divide by 6 times the polygon's area.
            double polygon_area = PolygonArea(gameActivity.playArea.vertices);
            x /= (6 * polygon_area);
            y /= (6 * polygon_area);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (x < 0)
            {
                x = -x;
                y = -y;
            }

            return new LatLng(x, y);
        }

        public LatLng ExtendLineSegment(LatLng firstPoint, LatLng secondPoint)
        {
            double lenAB = Math.Sqrt(Math.Pow(secondPoint.Latitude - firstPoint.Latitude, 2.0) + Math.Pow(secondPoint.Longitude - firstPoint.Longitude, 2.0));

            return new LatLng(firstPoint.Latitude + (firstPoint.Latitude - secondPoint.Latitude) / lenAB * (ShortestLineSegment(gameActivity.playArea.vertices) / 1000), firstPoint.Longitude + (firstPoint.Longitude - secondPoint.Longitude) / lenAB * (ShortestLineSegment(gameActivity.playArea.vertices) / 1000));
        }

        public LatLng FindRandomPoint()
        {
            Random random = new Random();

            List<Triangle> triangles = Triangulate(gameActivity.playArea.vertices);
            Triangle triangle = triangles[random.Next(0, triangles.Count)];

            LatLng v1 = new LatLng(triangle.vertices[1].Latitude - triangle.vertices[0].Latitude, triangle.vertices[1].Longitude - triangle.vertices[0].Longitude);
            LatLng v2 = new LatLng(triangle.vertices[2].Latitude - triangle.vertices[0].Latitude, triangle.vertices[2].Longitude - triangle.vertices[0].Longitude);
            double a1 = random.NextDouble();
            double a2 = random.NextDouble();

            v1.Latitude = v1.Latitude * a1;
            v1.Longitude = v1.Longitude * a1;
            v2.Latitude = v2.Latitude * a2;
            v2.Longitude = v2.Longitude * a2;

            LatLng x = new LatLng(v1.Latitude + v2.Latitude, v1.Longitude + v2.Longitude);
            x.Latitude += triangle.vertices[0].Latitude;
            x.Longitude += triangle.vertices[0].Longitude;

            return x;
        }

        // Triangulate the polygon.
        private List<Triangle> Triangulate(LinkedList<LatLng> vertices)
        {
            // Copy the points into a scratch array.
            int num_points = vertices.Count;
            LatLng[] pts = new LatLng[num_points];
            vertices.CopyTo(pts, 0);

            // Make room for the triangles.
            List<Triangle> triangles = new List<Triangle>();

            // While the copy of the polygon has more than
            // three points, remove an ear.
            while (pts.Length > 3)
            {
                // Remove an ear from the polygon.
                RemoveEar(ref pts, triangles);
            }

            // Copy the last three points into their own triangle.
            triangles.Add(AddTriangle(pts[0], pts[1], pts[2]));

            return triangles;
        }

        private Triangle AddTriangle(LatLng a, LatLng b, LatLng c)
        {
            Triangle triangle = new Triangle();

            triangle.vertices = new LatLng[3];
            triangle.vertices[0] = a;
            triangle.vertices[1] = b;
            triangle.vertices[2] = c;

            return triangle;
        }

        // Remove an ear from the polygon and
        // add it to the triangles array.
        private void RemoveEar(ref LatLng[] pts, List<Triangle> triangles)
        {
            // Find an ear.
            int a = 0;
            int b = 0;
            int c = 0;

            FindEar(pts, ref a, ref b, ref c);

            // Create a new triangle for the ear.
            triangles.Add(AddTriangle(pts[a], pts[b], pts[c]));

            // Remove the ear from the polygon.
            RemovePointFromArray(ref pts, b);
        }

        // Find the indexes of three points that form an "ear."
        private void FindEar(LatLng[] pts, ref int a, ref int b, ref int c)
        {
            int num_points = pts.Length;

            for (a = 0; a < num_points; a++)
            {
                b = (a + 1) % num_points;
                c = (b + 1) % num_points;

                if (FormsEar(pts, a, b, c))
                {
                    return;
                }
            }

            a--;

            // We should never get here because there should
            // always be at least two ears.
            Debug.Assert(false);
        }

        // Return true if the three points form an ear.
        private bool FormsEar(LatLng[] pts, int a, int b, int c)
        {
            // See if the angle ABC is concave.
            if (GetAngle(pts[a].Latitude, pts[a].Longitude, pts[b].Latitude, pts[b].Longitude, pts[c].Latitude, pts[c].Longitude) > 0)
            {
                // This is a concave corner so the triangle
                // cannot be an ear.
                return false;
            }

            // Make the triangle A, B, C.
            Triangle triangle = AddTriangle(pts[a], pts[b], pts[c]);

            // Check the other points to see 
            // if they lie in triangle A, B, C.
            for (int i = 0; i < pts.Length; i++)
            {
                if ((i != a) && (i != b) && (i != c))
                {
                    if (PointInTriangle(triangle, pts[i]))
                    {
                        // This point is in the triangle 
                        // do this is not an ear.
                        return false;
                    }
                }
            }

            // This is an ear.
            return true;
        }

        // Return true if the point is in the polygon.
        private bool PointInTriangle(Triangle triangle, LatLng point)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = triangle.vertices.Length - 1;
            double total_angle = GetAngle(triangle.vertices[max_point].Latitude, triangle.vertices[max_point].Longitude, point.Latitude, point.Longitude, triangle.vertices[0].Latitude, triangle.vertices[0].Longitude);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(triangle.vertices[i].Latitude, triangle.vertices[i].Longitude, point.Latitude, point.Longitude, triangle.vertices[i + 1].Latitude, triangle.vertices[i + 1].Longitude);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }

        // Remove point target from the array.
        private void RemovePointFromArray(ref LatLng[] pts, int target)
        {
            LatLng[] newPTS = new LatLng[pts.Length - 1];

            Array.Copy(pts, 0, newPTS, 0, target);
            Array.Copy(pts, target + 1, newPTS, target, pts.Length - target - 1);

            pts = newPTS;
        }

        // Return true if the point is in the polygon.
        public bool PointInPolygon(LinkedList<LatLng> vertices, LatLng point)
        {
            // Get the angle between the point and the
            // first and last vertices.
            double total_angle = GetAngle(vertices.Last.Value.Latitude, vertices.Last.Value.Longitude, point.Latitude, point.Longitude, vertices.First.Value.Latitude, vertices.First.Value.Longitude);

            // Add the angles from the point
            // to each other pair of vertices.
            LinkedListNode<LatLng> verticesNode = vertices.First.Next;

            while (true)
            {
                total_angle += GetAngle(verticesNode.Previous.Value.Latitude, verticesNode.Previous.Value.Longitude, point.Latitude, point.Longitude, verticesNode.Value.Latitude, verticesNode.Value.Longitude);

                if (verticesNode.Next != null)
                {
                    verticesNode = verticesNode.Next;
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

        public bool CircleLineIntersect(LatLng a, LatLng b, LatLng c, double radius)
        {
            double bc = LineSegmentLength(b, c);
            double ab = LineSegmentLength(a, b);

            if(ab > bc)
            {
                return false;
            }

            double ca = LineSegmentLength(c, a);

            if(ca > bc)
            {
                return false;
            }

            double d = Math.Abs(Math.Sin(GetAngle(a.Latitude, a.Longitude, b.Latitude, b.Longitude, c.Latitude, c.Longitude)) * ab);

            if(d > radius)
            {
                return false;
            }

            return true;
        }

        public LatLng DegreesToUnitVector(int degrees)
        {
            double radians = DegreesToRadians(degrees);

            LatLng degreesUnitVector = new LatLng(Math.Cos(radians), Math.Sin(radians));

            double degreesUnitVectorMagnitude = Math.Sqrt(Math.Pow(degreesUnitVector.Latitude, 2) + Math.Pow(degreesUnitVector.Longitude, 2));
            degreesUnitVector = new LatLng(degreesUnitVector.Latitude / degreesUnitVectorMagnitude, degreesUnitVector.Longitude / degreesUnitVectorMagnitude);

            return degreesUnitVector;
        }

        private double DegreesToRadians(int degrees)
        {
            return (degrees - 180) * (Math.PI / 180);
        }

        public int UnitVectorToDegrees(LatLng degreesUnitVector)
        {
            double radians = Math.Atan2(degreesUnitVector.Longitude, degreesUnitVector.Latitude);

            return RadiansToDegrees(radians);
        }

        private int RadiansToDegrees(double radians)
        {
            return (int)(radians * (180 / Math.PI)) + 180;
        }
    }
}