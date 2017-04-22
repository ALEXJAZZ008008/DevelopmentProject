using Android.Content;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using System.Collections.Generic;

namespace CaptureTheCampus.Game
{
    public class Area
    {
        private GameActivity gameActivity;
        private Utilities utilities;

        public Area(Context context, Utilities inUtilities)
        {
            Log.Info("Area", "Area built");

            gameActivity = (GameActivity)context;
            utilities = inUtilities;
        }

        public void UpdatePaths(int playerPosition)
        {
            if (gameActivity.playerArray[playerPosition].positionBool)
            {
                gameActivity.playerArray[playerPosition].positionBool = Static.Maths.CheckPosition(gameActivity, playerPosition);
            }

            if (gameActivity.playerArray[playerPosition].positionBool && gameActivity.playerArray[playerPosition].vertices.Count >= 1)
            {
                UpdatePath(playerPosition);
            }
            else
            {
                if (gameActivity.playerArray[playerPosition].deathBool)
                {
                    gameActivity.playerArray[playerPosition].positionBool = true;
                }

                ResetPath(playerPosition, gameActivity.playerArray[playerPosition].positionBool);
            }

            gameActivity.playerArray[playerPosition].positionBool = true;
            gameActivity.playerArray[playerPosition].deathBool = false;
        }

        private void UpdatePath(int playerPosition)
        {
            gameActivity.playerArray[playerPosition].verticesNode = gameActivity.playerArray[playerPosition].vertices.AddLast(gameActivity.playerArray[playerPosition].currentPosition);

            if (gameActivity.playerArray[playerPosition].drawingBool == false)
            {
                FindInitialPathIntersection(playerPosition);

                utilities.SetPolyline(playerPosition, gameActivity.playerArray[playerPosition].vertices);

                gameActivity.playerArray[playerPosition].drawingBool = true;
            }
            else
            {
                CheckPathIntercetion(playerPosition);

                utilities.SetPolyline(playerPosition, gameActivity.playerArray[gameActivity.playerPosition].vertices);
            }

        }

        private void FindInitialPathIntersection(int playerPosition)
        {
            gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.vertices.First.Next;

            while (true)
            {
                if (Static.Maths.DoIntersect(gameActivity.playerArray[playerPosition].vertices.First.Value, gameActivity.playerArray[playerPosition].vertices.Last.Value, gameActivity.gamePlayArea.verticesNode.Previous.Value, gameActivity.gamePlayArea.verticesNode.Value))
                {
                    gameActivity.playerArray[playerPosition].vertices.First.Value = Static.Maths.LineIntersectionPoint(gameActivity.playerArray[playerPosition].vertices.First.Value, gameActivity.playerArray[playerPosition].vertices.Last.Value, gameActivity.gamePlayArea.verticesNode.Previous.Value, gameActivity.gamePlayArea.verticesNode.Value);

                    break;
                }

                if (gameActivity.gamePlayArea.verticesNode.Next != null)
                {
                    gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.verticesNode.Next;
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

                        break;
                    }
                }
            }
        }

        private bool CheckInitialPathIntersectionCirularly(int playerPosition)
        {
            if (Static.Maths.DoIntersect(gameActivity.playerArray[playerPosition].vertices.First.Value, gameActivity.playerArray[playerPosition].vertices.Last.Value, gameActivity.gamePlayArea.vertices.First.Value, gameActivity.gamePlayArea.vertices.Last.Value))
            {
                gameActivity.playerArray[playerPosition].vertices.First.Value = Static.Maths.LineIntersectionPoint(gameActivity.playerArray[playerPosition].vertices.First.Value, gameActivity.playerArray[playerPosition].vertices.Last.Value, gameActivity.gamePlayArea.vertices.First.Value, gameActivity.gamePlayArea.vertices.Last.Value);

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
            for (int i = 0; i < gameActivity.numberOfPlayers; i++)
            {
                if (gameActivity.playerArray[i].vertices.First != null && gameActivity.playerArray[i].vertices.First.Next != null)
                {
                    gameActivity.playerArray[i].verticesNode = gameActivity.playerArray[i].vertices.First.Next;

                    while (true)
                    {
                        if ((gameActivity.playerArray[playerPosition].vertices.Last.Previous.Value != gameActivity.playerArray[i].verticesNode.Previous.Value && gameActivity.playerArray[playerPosition].vertices.Last.Previous.Value != gameActivity.playerArray[i].verticesNode.Value) && (gameActivity.playerArray[playerPosition].vertices.Last.Value != gameActivity.playerArray[i].verticesNode.Previous.Value && gameActivity.playerArray[playerPosition].vertices.Last.Value != gameActivity.playerArray[i].verticesNode.Value))
                        {
                            if (Static.Maths.DoIntersect(gameActivity.playerArray[playerPosition].vertices.Last.Previous.Value, gameActivity.playerArray[playerPosition].vertices.Last.Value, gameActivity.playerArray[i].verticesNode.Previous.Value, gameActivity.playerArray[i].verticesNode.Value))
                            {
                                if (i == playerPosition)
                                {
                                    AmendPath(playerPosition, gameActivity.playerArray[playerPosition].vertices.Last.Previous.Value, gameActivity.playerArray[playerPosition].vertices.Last.Value, gameActivity.playerArray[playerPosition].verticesNode.Previous.Value, gameActivity.playerArray[playerPosition].verticesNode.Value);
                                }
                                else
                                {
                                    gameActivity.KillPlayer(playerPosition);
                                }

                                break;
                            }
                        }

                        if (gameActivity.playerArray[i].verticesNode.Next != null)
                        {
                            gameActivity.playerArray[i].verticesNode = gameActivity.playerArray[i].verticesNode.Next;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void AmendPath(int playerPosition, LatLng p1, LatLng p2, LatLng q1, LatLng q2)
        {
            LinkedList<LatLng> temporaryVertices = new LinkedList<LatLng>();

            gameActivity.playerArray[playerPosition].verticesNode = gameActivity.playerArray[playerPosition].vertices.First;

            while (true)
            {
                if (gameActivity.playerArray[playerPosition].verticesNode.Value != q2)
                {
                    temporaryVertices.AddLast(gameActivity.playerArray[playerPosition].verticesNode.Value);
                }
                else
                {
                    break;
                }

                if (gameActivity.playerArray[playerPosition].verticesNode.Next != null)
                {
                    gameActivity.playerArray[playerPosition].verticesNode = gameActivity.playerArray[playerPosition].verticesNode.Next;
                }
                else
                {
                    break;
                }
            }

            temporaryVertices.AddLast(Static.Maths.LineIntersectionPoint(p1, p2, q1, q2));

            temporaryVertices.AddLast(p2);

            gameActivity.playerArray[playerPosition].vertices = temporaryVertices;
        }

        private void ResetPath(int playerPosition, bool positionBool)
        {
            if (gameActivity.playerArray[playerPosition].drawingBool != false)
            {
                gameActivity.playerArray[playerPosition].verticesNode = gameActivity.playerArray[playerPosition].vertices.AddLast(gameActivity.playerArray[playerPosition].currentPosition).Previous;

                FindFinalPathIntersection(playerPosition);

                utilities.SetPolyline(playerPosition, gameActivity.playerArray[playerPosition].vertices);

                BuildArea(playerPosition);

                gameActivity.playerArray[playerPosition].polyline = null;

                gameActivity.playerArray[playerPosition].drawingBool = false;
            }

            gameActivity.playerArray[playerPosition].vertices.Clear();

            if (!positionBool)
            {
                gameActivity.playerArray[playerPosition].verticesNode = gameActivity.playerArray[playerPosition].vertices.AddFirst(gameActivity.playerArray[playerPosition].currentPosition);
            }
        }

        private void FindFinalPathIntersection(int playerPosition)
        {
            gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.vertices.First.Next;

            while (true)
            {
                if (Static.Maths.DoIntersect(gameActivity.playerArray[playerPosition].vertices.Last.Previous.Value, gameActivity.playerArray[playerPosition].vertices.Last.Value, gameActivity.gamePlayArea.verticesNode.Previous.Value, gameActivity.gamePlayArea.verticesNode.Value))
                {
                    gameActivity.playerArray[playerPosition].vertices.Last.Value = Static.Maths.LineIntersectionPoint(gameActivity.playerArray[playerPosition].vertices.Last.Previous.Value, gameActivity.playerArray[playerPosition].vertices.Last.Value, gameActivity.gamePlayArea.verticesNode.Previous.Value, gameActivity.gamePlayArea.verticesNode.Value);

                    break;
                }

                if (gameActivity.gamePlayArea.verticesNode.Next != null)
                {
                    gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.verticesNode.Next;
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

                        break;
                    }
                }
            }
        }

        private bool CheckFinalPathIntersectionCircularly(int playerPosition)
        {
            if (Static.Maths.DoIntersect(gameActivity.playerArray[playerPosition].vertices.Last.Previous.Value, gameActivity.playerArray[playerPosition].vertices.Last.Value, gameActivity.gamePlayArea.vertices.First.Value, gameActivity.gamePlayArea.vertices.Last.Value))
            {
                gameActivity.playerArray[playerPosition].vertices.Last.Value = Static.Maths.LineIntersectionPoint(gameActivity.playerArray[playerPosition].vertices.Last.Previous.Value, gameActivity.playerArray[playerPosition].vertices.Last.Value, gameActivity.gamePlayArea.vertices.First.Value, gameActivity.gamePlayArea.vertices.Last.Value);

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
            if (CheckPlayAreaIntersection(gameActivity.playerArray[playerPosition].vertices.First.Value))
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
            if (CheckPlayAreaIntersection(gameActivity.playerArray[playerPosition].vertices.Last.Value))
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
            LatLng firstExtendedPosition = Static.Maths.ExtendLineSegment(gameActivity, value, Static.Maths.FindCentroid(gameActivity));
            LatLng secondExtendedPosition = Static.Maths.ExtendLineSegment(gameActivity, value, firstExtendedPosition);

            gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.vertices.First.Next;

            while (true)
            {
                if (Static.Maths.DoIntersect(firstExtendedPosition, secondExtendedPosition, gameActivity.gamePlayArea.verticesNode.Previous.Value, gameActivity.gamePlayArea.verticesNode.Value))
                {
                    gameActivity.gamePlayArea.vertices.AddBefore(gameActivity.gamePlayArea.verticesNode, value);

                    return true;
                }

                if (gameActivity.gamePlayArea.verticesNode.Next != null)
                {
                    gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.verticesNode.Next;
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
            if (Static.Maths.DoIntersect(firstExtendedPosition, secondExtendedPosition, gameActivity.gamePlayArea.vertices.First.Value, gameActivity.gamePlayArea.vertices.Last.Value))
            {
                gameActivity.gamePlayArea.vertices.AddLast(value);

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
            gameActivity.playerArray[playerPosition].verticesNode = gameActivity.playerArray[playerPosition].vertices.First;

            while (true)
            {
                firstPolygonVertices.AddLast(gameActivity.playerArray[playerPosition].verticesNode.Value);
                secondPolygonVertices.AddLast(gameActivity.playerArray[playerPosition].verticesNode.Value);

                if (gameActivity.playerArray[playerPosition].verticesNode.Next != null)
                {
                    gameActivity.playerArray[playerPosition].verticesNode = gameActivity.playerArray[playerPosition].verticesNode.Next;
                }
                else
                {
                    break;
                }
            }
        }

        private void AddFirstPolygon(int playerPosition, LinkedList<LatLng> firstPolygonVertices)
        {
            if (gameActivity.gamePlayArea.vertices.Find(gameActivity.playerArray[playerPosition].vertices.Last.Value).Next != null)
            {
                gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.vertices.Find(gameActivity.playerArray[playerPosition].vertices.Last.Value).Next;
            }
            else
            {
                gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.vertices.First;
            }

            while (true)
            {
                if (gameActivity.gamePlayArea.verticesNode.Value != gameActivity.playerArray[playerPosition].vertices.First.Value)
                {
                    firstPolygonVertices.AddLast(gameActivity.gamePlayArea.verticesNode.Value);

                    if (gameActivity.gamePlayArea.verticesNode.Next != null)
                    {
                        gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.verticesNode.Next;
                    }
                    else
                    {
                        gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.vertices.First;
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
            if (gameActivity.gamePlayArea.vertices.Find(gameActivity.playerArray[playerPosition].vertices.Last.Value).Previous != null)
            {
                gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.vertices.Find(gameActivity.playerArray[playerPosition].vertices.Last.Value).Previous;
            }
            else
            {
                gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.vertices.Last;
            }

            while (true)
            {
                if (gameActivity.gamePlayArea.verticesNode.Value != gameActivity.playerArray[playerPosition].vertices.First.Value)
                {
                    secondPolygonVertices.AddLast(gameActivity.gamePlayArea.verticesNode.Value);

                    if (gameActivity.gamePlayArea.verticesNode.Previous != null)
                    {
                        gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.verticesNode.Previous;
                    }
                    else
                    {
                        gameActivity.gamePlayArea.verticesNode = gameActivity.gamePlayArea.vertices.Last;
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
            double firstArea = Static.Maths.PolygonArea(new LinkedList<LatLng>(gameActivity.gamePlayArea.polygons.Last.Previous.Value.Points));
            double secondArea = Static.Maths.PolygonArea(new LinkedList<LatLng>(gameActivity.gamePlayArea.polygons.Last.Value.Points));

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
            gameActivity.gamePlayArea.polygons.First.Value.Points = gameActivity.gamePlayArea.polygons.Last.Value.Points;

            gameActivity.gamePlayArea.polygons.Remove(gameActivity.gamePlayArea.polygons.Last.Value);

            UpdateScore(playerPosition, takenArea, leftArea);
        }

        private void AddSecondArea(int playerPosition, double takenArea, double leftArea)
        {
            gameActivity.gamePlayArea.polygons.First.Value.Points = gameActivity.gamePlayArea.polygons.Last.Previous.Value.Points;

            gameActivity.gamePlayArea.polygons.Remove(gameActivity.gamePlayArea.polygons.Last.Previous.Value);

            UpdateScore(playerPosition, takenArea, leftArea);
        }

        private void UpdateScore(int playerPosition, double takenArea, double leftArea)
        {
            gameActivity.area = (int)((leftArea / gameActivity.initialArea) * 100);
            gameActivity.areaTextView.Text = "Area: " + gameActivity.area.ToString();

            gameActivity.playerArray[playerPosition].score += (int)((takenArea / gameActivity.initialArea) * 100);

            if (playerPosition == gameActivity.playerPosition)
            {
                gameActivity.scoreTextView.Text = "Score: " + gameActivity.playerArray[gameActivity.playerPosition].score.ToString();
            }
        }

        private void UpdatePolygons(int playerPosition)
        {
            gameActivity.gamePlayArea.vertices = new LinkedList<LatLng>(gameActivity.gamePlayArea.polygons.First.Value.Points);
            gameActivity.gamePlayArea.vertices.RemoveLast();

            gameActivity.gamePlayArea.polygons.Last.Value.FillColor = Color.HSVToColor(new float[] { utilities.Colour(playerPosition), 1.0f, 1.0f });
        }
    }
}