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

        public void UpdatePaths(int playerPosition)
        {
            if (gameActivity.player[playerPosition].positionBool)
            {
                gameActivity.player[playerPosition].positionBool = maths.CheckPosition(playerPosition);
            }

            if (gameActivity.player[playerPosition].positionBool && gameActivity.player[playerPosition].vertices.Count >= 1)
            {
                UpdatePath(playerPosition);
            }
            else
            {
                if (gameActivity.player[playerPosition].deathBool)
                {
                    gameActivity.player[playerPosition].positionBool = true;
                }

                ResetPath(playerPosition, gameActivity.player[playerPosition].positionBool);
            }

            gameActivity.player[playerPosition].positionBool = true;
            gameActivity.player[playerPosition].deathBool = false;
        }

        private void UpdatePath(int playerPosition)
        {
            gameActivity.player[playerPosition].verticesNode = gameActivity.player[playerPosition].vertices.AddLast(gameActivity.player[playerPosition].currentPosition);

            if (gameActivity.player[playerPosition].drawingBool == false)
            {
                FindInitialPathIntersection(playerPosition);

                utilities.SetPolyline(playerPosition, gameActivity.player[playerPosition].vertices);

                gameActivity.player[playerPosition].drawingBool = true;
            }
            else
            {
                CheckPathIntercetion(playerPosition);

                utilities.SetPolyline(playerPosition, gameActivity.player[gameActivity.playerPosition].vertices);
            }

        }

        private void FindInitialPathIntersection(int playerPosition)
        {
            gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First.Next;

            while (true)
            {
                if (maths.DoIntersect(gameActivity.player[playerPosition].vertices.First.Value, gameActivity.player[playerPosition].vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value))
                {
                    gameActivity.player[playerPosition].vertices.First.Value = maths.LineIntersectionPoint(gameActivity.player[playerPosition].vertices.First.Value, gameActivity.player[playerPosition].vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value);

                    break;
                }

                if (gameActivity.playArea.verticesNode.Next != null)
                {
                    gameActivity.playArea.verticesNode = gameActivity.playArea.verticesNode.Next;
                }
                else
                {
                    if (CheckInitialPathIntersectionCirularly(playerPosition))
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

        private bool CheckInitialPathIntersectionCirularly(int playerPosition)
        {
            if (maths.DoIntersect(gameActivity.player[playerPosition].vertices.First.Value, gameActivity.player[playerPosition].vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value))
            {
                gameActivity.player[playerPosition].vertices.First.Value = maths.LineIntersectionPoint(gameActivity.player[playerPosition].vertices.First.Value, gameActivity.player[playerPosition].vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value);

                return true;
            }
            else
            {
                Log.Error("CheckInitialPathIntersectionCirularly", "Error in path");
                Toast.MakeText(gameActivity, "Error in path", ToastLength.Long).Show();

                return false;
            }
        }

        private void CheckPathIntercetion(int playerPosition)
        {
            gameActivity.player[playerPosition].verticesNode = gameActivity.player[playerPosition].vertices.First.Next;

            while (true)
            {
                if (maths.DoIntersect(gameActivity.player[playerPosition].vertices.Last.Previous.Value, gameActivity.player[playerPosition].vertices.Last.Value, gameActivity.player[playerPosition].verticesNode.Previous.Value, gameActivity.player[playerPosition].verticesNode.Value))
                {
                    AmendPath(playerPosition, gameActivity.player[playerPosition].vertices.Last.Previous.Value, gameActivity.player[playerPosition].vertices.Last.Value, gameActivity.player[playerPosition].verticesNode.Previous.Value, gameActivity.player[playerPosition].verticesNode.Value);

                    break;
                }

                if (gameActivity.player[playerPosition].verticesNode.Next != null)
                {
                    gameActivity.player[playerPosition].verticesNode = gameActivity.player[playerPosition].verticesNode.Next;
                }
                else
                {
                    break;
                }
            }
        }

        private void AmendPath(int playerPosition, LatLng p1, LatLng p2, LatLng q1, LatLng q2)
        {
            LinkedList<LatLng> temporaryVertices = new LinkedList<LatLng>();

            gameActivity.player[playerPosition].verticesNode = gameActivity.player[playerPosition].vertices.First;

            while (true)
            {
                if (gameActivity.player[playerPosition].verticesNode.Value != q2)
                {
                    temporaryVertices.AddLast(gameActivity.player[playerPosition].verticesNode.Value);
                }
                else
                {
                    break;
                }

                if (gameActivity.player[playerPosition].verticesNode.Next != null)
                {
                    gameActivity.player[playerPosition].verticesNode = gameActivity.player[playerPosition].verticesNode.Next;
                }
                else
                {
                    break;
                }
            }

            temporaryVertices.AddLast(maths.LineIntersectionPoint(p1, p2, q1, q2));

            temporaryVertices.AddLast(p2);

            gameActivity.player[playerPosition].vertices = temporaryVertices;
        }

        private void ResetPath(int playerPosition, bool positionBool)
        {
            if (gameActivity.player[playerPosition].drawingBool != false)
            {
                gameActivity.player[playerPosition].verticesNode = gameActivity.player[playerPosition].vertices.AddLast(gameActivity.player[playerPosition].currentPosition).Previous;

                FindFinalPathIntersection(playerPosition);

                utilities.SetPolyline(playerPosition, gameActivity.player[playerPosition].vertices);

                BuildArea(playerPosition);

                gameActivity.player[playerPosition].polyline = null;

                gameActivity.player[playerPosition].drawingBool = false;
            }

            gameActivity.player[playerPosition].vertices.Clear();

            if (!positionBool)
            {
                gameActivity.player[playerPosition].verticesNode = gameActivity.player[playerPosition].vertices.AddFirst(gameActivity.player[playerPosition].currentPosition);
            }
        }

        private void FindFinalPathIntersection(int playerPosition)
        {
            gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First.Next;

            while (true)
            {
                if (maths.DoIntersect(gameActivity.player[playerPosition].vertices.Last.Previous.Value, gameActivity.player[playerPosition].vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value))
                {
                    gameActivity.player[playerPosition].vertices.Last.Value = maths.LineIntersectionPoint(gameActivity.player[playerPosition].vertices.Last.Previous.Value, gameActivity.player[playerPosition].vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value);

                    break;
                }

                if (gameActivity.playArea.verticesNode.Next != null)
                {
                    gameActivity.playArea.verticesNode = gameActivity.playArea.verticesNode.Next;
                }
                else
                {
                    if (CheckFinalPathIntersectionCircularly(playerPosition))
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

        private bool CheckFinalPathIntersectionCircularly(int playerPosition)
        {
            if (maths.DoIntersect(gameActivity.player[playerPosition].vertices.Last.Previous.Value, gameActivity.player[playerPosition].vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value))
            {
                gameActivity.player[playerPosition].vertices.Last.Value = maths.LineIntersectionPoint(gameActivity.player[playerPosition].vertices.Last.Previous.Value, gameActivity.player[playerPosition].vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value);

                return true;
            }
            else
            {
                Log.Error("CheckFinalPathIntersectionCircularly", "Error in path");
                Toast.MakeText(gameActivity, "Error in path", ToastLength.Long).Show();

                return false;
            }
        }

        private void BuildArea(int playerPosition)
        {
            CheckFirstPlayAreaIntersection(playerPosition);
        }

        private void CheckFirstPlayAreaIntersection(int playerPosition)
        {
            if (CheckPlayAreaIntersection(gameActivity.player[playerPosition].vertices.First.Value))
            {
                CheckSecondPlayAreaIntersection(playerPosition);
            }
            else
            {
                gameActivity.finishBool = true;
            }
        }

        private void CheckSecondPlayAreaIntersection(int playerPosition)
        {
            if (CheckPlayAreaIntersection(gameActivity.player[playerPosition].vertices.Last.Value))
            {
                BuildAreas(playerPosition);
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

        private void BuildAreas(int playerPosition)
        {
            LinkedList<LatLng> firstPolygonVertices = new LinkedList<LatLng>();
            LinkedList<LatLng> secondPolygonVertices = new LinkedList<LatLng>();

            AddPath(playerPosition, firstPolygonVertices, secondPolygonVertices);

            AddFirstPolygon(playerPosition, firstPolygonVertices);

            utilities.SetPolygon(firstPolygonVertices);

            AddSecondPolygon(playerPosition, secondPolygonVertices);

            utilities.SetPolygon(secondPolygonVertices);

            TestAreas(playerPosition);
        }

        private void AddPath(int playerPosition, LinkedList<LatLng> firstPolygonVertices, LinkedList<LatLng> secondPolygonVertices)
        {
            gameActivity.player[playerPosition].verticesNode = gameActivity.player[playerPosition].vertices.First;

            while (true)
            {
                firstPolygonVertices.AddLast(gameActivity.player[playerPosition].verticesNode.Value);
                secondPolygonVertices.AddLast(gameActivity.player[playerPosition].verticesNode.Value);

                if (gameActivity.player[playerPosition].verticesNode.Next != null)
                {
                    gameActivity.player[playerPosition].verticesNode = gameActivity.player[playerPosition].verticesNode.Next;
                }
                else
                {
                    break;
                }
            }
        }

        private void AddFirstPolygon(int playerPosition, LinkedList<LatLng> firstPolygonVertices)
        {
            if (gameActivity.playArea.vertices.Find(gameActivity.player[playerPosition].vertices.Last.Value).Next != null)
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.Find(gameActivity.player[playerPosition].vertices.Last.Value).Next;
            }
            else
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First;
            }

            while (true)
            {
                if (gameActivity.playArea.verticesNode.Value != gameActivity.player[playerPosition].vertices.First.Value)
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

        private void AddSecondPolygon(int playerPosition, LinkedList<LatLng> secondPolygonVertices)
        {
            if (gameActivity.playArea.vertices.Find(gameActivity.player[playerPosition].vertices.Last.Value).Previous != null)
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.Find(gameActivity.player[playerPosition].vertices.Last.Value).Previous;
            }
            else
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.Last;
            }

            while (true)
            {
                if (gameActivity.playArea.verticesNode.Value != gameActivity.player[playerPosition].vertices.First.Value)
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

        private void TestAreas(int playerPosition)
        {
            double firstArea = maths.PolygonArea(new LinkedList<LatLng>(gameActivity.playArea.polygons.Last.Previous.Value.Points));
            double secondArea = maths.PolygonArea(new LinkedList<LatLng>(gameActivity.playArea.polygons.Last.Value.Points));

            if (firstArea <= secondArea)
            {
                AddFirstArea(playerPosition, firstArea, secondArea);
            }
            else
            {
                AddSecondArea(playerPosition, secondArea, firstArea);
            }

            UpdatePolygons(playerPosition);
        }

        private void AddFirstArea(int playerPosition, double takenArea, double leftArea)
        {
            gameActivity.playArea.polygons.First.Value.Points = gameActivity.playArea.polygons.Last.Value.Points;

            gameActivity.playArea.polygons.Remove(gameActivity.playArea.polygons.Last.Value);

            UpdateScore(playerPosition, takenArea, leftArea);
        }

        private void AddSecondArea(int playerPosition, double takenArea, double leftArea)
        {
            gameActivity.playArea.polygons.First.Value.Points = gameActivity.playArea.polygons.Last.Previous.Value.Points;

            gameActivity.playArea.polygons.Remove(gameActivity.playArea.polygons.Last.Previous.Value);

            UpdateScore(playerPosition, takenArea, leftArea);
        }

        private void UpdateScore(int playerPosition, double takenArea, double leftArea)
        {
            gameActivity.area = (int)((leftArea / gameActivity.initialArea) * 100);
            gameActivity.areaTextView.Text = "Area: " + gameActivity.area.ToString();

            gameActivity.player[playerPosition].score += (int)((takenArea / gameActivity.initialArea) * 100);
            gameActivity.scoreTextView.Text = "Score: " + gameActivity.player[playerPosition].score.ToString();
        }

        private void UpdatePolygons(int playerPosition)
        {
            gameActivity.playArea.vertices = new LinkedList<LatLng>(gameActivity.playArea.polygons.First.Value.Points);
            gameActivity.playArea.vertices.RemoveLast();

            gameActivity.playArea.polygons.Last.Value.FillColor = Color.HSVToColor(new float[] { utilities.Colour(playerPosition), 1.0f, 1.0f });
        }
    }
}