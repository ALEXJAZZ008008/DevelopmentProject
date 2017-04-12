using Android.Content;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using System.Collections.Generic;

namespace CaptureTheCampus
{
    public class Area
    {
        private GameActivity gameActivity;
        private Utilities utilities;
        private Maths maths;

        public Area(Context context1, Context context2)
        {
            Log.Info("Area", "Area built");

            gameActivity = (GameActivity)context1;
            utilities = (Utilities)context2;
            maths = new Maths(gameActivity);
        }

        public void UpdatePaths()
        {
            bool positionBool = maths.CheckPosition();

            if (positionBool && gameActivity.path.vertices.Count >= 1)
            {
                UpdatePath();
            }
            else
            {
                ResetPath(positionBool);
            }
        }

        private void UpdatePath()
        {
            gameActivity.path.verticesNode = gameActivity.path.vertices.AddAfter(gameActivity.path.verticesNode, gameActivity.path.currentPosition);

            if (gameActivity.path.drawingBool == false)
            {
                FindInitialPathIntersection();

                utilities.SetPolyline(gameActivity.path.vertices);

                gameActivity.path.drawingBool = true;
            }
            else
            {
                utilities.SetPolyline(gameActivity.path.vertices);
            }

        }

        private void FindInitialPathIntersection()
        {
            gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First.Next;

            while (true)
            {
                if (maths.DoIntersect(gameActivity.path.vertices.First.Value, gameActivity.path.vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value))
                {
                    gameActivity.path.vertices.First.Value = maths.LineIntersectionPoint(gameActivity.path.vertices.First.Value, gameActivity.path.vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value);

                    break;
                }

                if (gameActivity.playArea.verticesNode.Next != null)
                {
                    gameActivity.playArea.verticesNode = gameActivity.playArea.verticesNode.Next;
                }
                else
                {
                    if (CheckInitialPathIntersectionCirularly())
                    {
                        break;
                    }
                    else
                    {
                        gameActivity.finishBool = true;
                    }
                }
            }
        }

        private bool CheckInitialPathIntersectionCirularly()
        {
            if (maths.DoIntersect(gameActivity.path.vertices.First.Value, gameActivity.path.vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value))
            {
                gameActivity.path.vertices.First.Value = maths.LineIntersectionPoint(gameActivity.path.vertices.First.Value, gameActivity.path.vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value);

                return true;
            }
            else
            {
                Log.Error("CheckInitialPathIntersectionCirularly", "Error in path");
                Toast.MakeText(gameActivity, "Error in path", ToastLength.Long).Show();

                return false;
            }
        }

        private void ResetPath(bool positionBool)
        {
            if (gameActivity.path.drawingBool != false)
            {
                gameActivity.path.verticesNode = gameActivity.path.vertices.AddLast(gameActivity.path.currentPosition).Previous;

                FindFinalPathIntersection();

                utilities.SetPolyline(gameActivity.path.vertices);

                BuildArea();

                gameActivity.path.drawingBool = false;
            }

            gameActivity.path.vertices.Clear();

            if (!positionBool)
            {
                gameActivity.path.verticesNode = gameActivity.path.vertices.AddFirst(gameActivity.path.currentPosition);
            }
        }

        private void FindFinalPathIntersection()
        {
            gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First.Next;

            while (true)
            {
                if (maths.DoIntersect(gameActivity.path.verticesNode.Value, gameActivity.path.vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value))
                {
                    gameActivity.path.vertices.Last.Value = maths.LineIntersectionPoint(gameActivity.path.verticesNode.Value, gameActivity.path.vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value);

                    break;
                }

                if (gameActivity.playArea.verticesNode.Next != null)
                {
                    gameActivity.playArea.verticesNode = gameActivity.playArea.verticesNode.Next;
                }
                else
                {
                    if (CheckFinalPathIntersectionCircularly())
                    {
                        break;
                    }
                    else
                    {
                        gameActivity.finishBool = true;
                    }
                }
            }
        }

        private bool CheckFinalPathIntersectionCircularly()
        {
            if (maths.DoIntersect(gameActivity.path.verticesNode.Value, gameActivity.path.vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value))
            {
                gameActivity.path.vertices.Last.Value = maths.LineIntersectionPoint(gameActivity.path.verticesNode.Value, gameActivity.path.vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value);

                return true;
            }
            else
            {
                Log.Error("CheckFinalPathIntersectionCircularly", "Error in path");
                Toast.MakeText(gameActivity, "Error in path", ToastLength.Long).Show();

                return false;
            }
        }

        private void BuildArea()
        {
            CheckFirstPlayAreaIntersection();
        }

        private void CheckFirstPlayAreaIntersection()
        {
            if(CheckPlayAreaIntersection(gameActivity.path.vertices.First.Value))
            {
                CheckSecondPlayAreaIntersection();
            }
            else
            {
                gameActivity.finishBool = true;
            } 
        }

        private void CheckSecondPlayAreaIntersection()
        {
            if (CheckPlayAreaIntersection(gameActivity.path.vertices.Last.Value))
            {
                BuildAreas();
            }
            else
            {
                gameActivity.finishBool = true;
            }
        }

        private bool CheckPlayAreaIntersection(LatLng value)
        {
            LatLng firstExtendedPosition = maths.ExtendLineSegment(value, maths.FindCentroid());
            LatLng secondExtendedPosition = maths.ExtendLineSegment(value, firstExtendedPosition);

            gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First.Next;

            while (true)
            {
                if (maths.DoIntersect(firstExtendedPosition, secondExtendedPosition, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value))
                {
                    gameActivity.playArea.vertices.AddBefore(gameActivity.playArea.verticesNode, value);

                    return true;
                }

                if (gameActivity.playArea.verticesNode.Next != null)
                {
                    gameActivity.playArea.verticesNode = gameActivity.playArea.verticesNode.Next;
                }
                else
                {
                    if (CheckPlayAreaIntersectionCircularly(firstExtendedPosition, secondExtendedPosition, value))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private bool CheckPlayAreaIntersectionCircularly(LatLng firstExtendedPosition, LatLng secondExtendedPosition, LatLng value)
        {
            if (maths.DoIntersect(firstExtendedPosition, secondExtendedPosition, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value))
            {
                gameActivity.playArea.vertices.AddLast(value);

                return true;
            }
            else
            {
                Log.Error("CheckPlayAreaIntersectionCircularly", "Error in play area");
                Toast.MakeText(gameActivity, "Error in play area", ToastLength.Long).Show();

                return false;
            }
        }

        private void BuildAreas()
        {
            LinkedList<LatLng> firstPolygonVertices = new LinkedList<LatLng>();
            LinkedList<LatLng> secondPolygonVertices = new LinkedList<LatLng>();

            AddPath(firstPolygonVertices, secondPolygonVertices);

            AddFirstPolygon(firstPolygonVertices);

            utilities.SetPolygon(firstPolygonVertices);

            AddSecondPolygon(secondPolygonVertices);

            utilities.SetPolygon(secondPolygonVertices);

            TestAreas();
        }

        private void AddPath(LinkedList<LatLng> firstPolygonVertices, LinkedList<LatLng> secondPolygonVertices)
        {
            gameActivity.path.verticesNode = gameActivity.path.vertices.First;

            while (true)
            {
                firstPolygonVertices.AddLast(gameActivity.path.verticesNode.Value);
                secondPolygonVertices.AddLast(gameActivity.path.verticesNode.Value);

                if (gameActivity.path.verticesNode.Next != null)
                {
                    gameActivity.path.verticesNode = gameActivity.path.verticesNode.Next;
                }
                else
                {
                    break;
                }
            }
        }

        private void AddFirstPolygon(LinkedList<LatLng> firstPolygonVertices)
        {
            if (gameActivity.playArea.vertices.Find(gameActivity.path.vertices.Last.Value).Next != null)
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.Find(gameActivity.path.vertices.Last.Value).Next;
            }
            else
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First;
            }

            while (true)
            {
                if (gameActivity.playArea.verticesNode.Value != gameActivity.path.vertices.First.Value)
                {
                    firstPolygonVertices.AddLast(gameActivity.playArea.verticesNode.Value);

                    if (gameActivity.playArea.verticesNode.Next != null)
                    {
                        gameActivity.playArea.verticesNode = gameActivity.playArea.verticesNode.Next;
                    }
                    else
                    {
                        gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void AddSecondPolygon(LinkedList<LatLng> secondPolygonVertices)
        {
            if (gameActivity.playArea.vertices.Find(gameActivity.path.vertices.Last.Value).Previous != null)
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.Find(gameActivity.path.vertices.Last.Value).Previous;
            }
            else
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.Last;
            }

            while (true)
            {
                if (gameActivity.playArea.verticesNode.Value != gameActivity.path.vertices.First.Value)
                {
                    secondPolygonVertices.AddLast(gameActivity.playArea.verticesNode.Value);

                    if (gameActivity.playArea.verticesNode.Previous != null)
                    {
                        gameActivity.playArea.verticesNode = gameActivity.playArea.verticesNode.Previous;
                    }
                    else
                    {
                        gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.Last;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void TestAreas()
        {
            double firstArea = maths.PolygonArea(new LinkedList<LatLng>(gameActivity.playArea.polygons.Last.Previous.Value.Points));
            double secondArea = maths.PolygonArea(new LinkedList<LatLng>(gameActivity.playArea.polygons.Last.Value.Points));

            if (firstArea <= secondArea)
            {
                AddFirstArea(firstArea);
            }
            else
            {
                AddSecondArea(secondArea);
            }

            UpdatePolygons();
        }

        private void AddFirstArea(double firstArea)
        {
            gameActivity.playArea.polygons.First.Value.Points = gameActivity.playArea.polygons.Last.Value.Points;            

            gameActivity.playArea.polygons.Remove(gameActivity.playArea.polygons.Last.Value);

            UpdateScore(firstArea);
        }

        private void AddSecondArea(double secondArea)
        {
            gameActivity.playArea.polygons.First.Value.Points = gameActivity.playArea.polygons.Last.Previous.Value.Points;

            gameActivity.playArea.polygons.Remove(gameActivity.playArea.polygons.Last.Previous.Value);

            UpdateScore(secondArea);
        }

        private void UpdateScore(double area)
        {
            gameActivity.area = (int)((maths.PolygonArea(gameActivity.playArea.vertices) / gameActivity.initialArea) * 100);
            gameActivity.areaTextView.Text = "Area: " + gameActivity.area.ToString();

            gameActivity.score += (int)((area / gameActivity.initialArea) * 100);
            gameActivity.scoreTextView.Text = "Score: " + gameActivity.score.ToString();
        }

        private void UpdatePolygons()
        {
            gameActivity.playArea.vertices = new LinkedList<LatLng>(gameActivity.playArea.polygons.First.Value.Points);
            gameActivity.playArea.vertices.RemoveLast();

            gameActivity.playArea.polygons.Last.Value.FillColor = Color.HSVToColor(new float[] { utilities.Colour(gameActivity.markerNumber), 1.0f, 1.0f });
        }
    }
}