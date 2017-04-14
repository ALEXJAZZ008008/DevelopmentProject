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
            if (gameActivity.player[gameActivity.playerPosition].positionBool)
            {
                gameActivity.player[gameActivity.playerPosition].positionBool = maths.CheckPosition();
            }

            if (gameActivity.player[gameActivity.playerPosition].positionBool && gameActivity.player[gameActivity.playerPosition].vertices.Count >= 1)
            {
                UpdatePath();
            }
            else
            {
                if(gameActivity.player[gameActivity.playerPosition].deathBool)
                {
                    gameActivity.player[gameActivity.playerPosition].positionBool = true;
                }

                ResetPath(gameActivity.player[gameActivity.playerPosition].positionBool);
            }

            gameActivity.player[gameActivity.playerPosition].positionBool = true;
            gameActivity.player[gameActivity.playerPosition].deathBool = false;
        }

        private void UpdatePath()
        {
            gameActivity.player[gameActivity.playerPosition].verticesNode = gameActivity.player[gameActivity.playerPosition].vertices.AddLast(gameActivity.player[gameActivity.playerPosition].currentPosition);

            if (gameActivity.player[gameActivity.playerPosition].drawingBool == false)
            {
                FindInitialPathIntersection();

                utilities.SetPolyline(gameActivity.player[gameActivity.playerPosition].vertices);

                gameActivity.player[gameActivity.playerPosition].drawingBool = true;
            }
            else
            {
                CheckPathIntercetion();

                utilities.SetPolyline(gameActivity.player[gameActivity.playerPosition].vertices);
            }

        }

        private void FindInitialPathIntersection()
        {
            gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First.Next;

            while (true)
            {
                if (maths.DoIntersect(gameActivity.player[gameActivity.playerPosition].vertices.First.Value, gameActivity.player[gameActivity.playerPosition].vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value))
                {
                    gameActivity.player[gameActivity.playerPosition].vertices.First.Value = maths.LineIntersectionPoint(gameActivity.player[gameActivity.playerPosition].vertices.First.Value, gameActivity.player[gameActivity.playerPosition].vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value);

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
            if (maths.DoIntersect(gameActivity.player[gameActivity.playerPosition].vertices.First.Value, gameActivity.player[gameActivity.playerPosition].vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value))
            {
                gameActivity.player[gameActivity.playerPosition].vertices.First.Value = maths.LineIntersectionPoint(gameActivity.player[gameActivity.playerPosition].vertices.First.Value, gameActivity.player[gameActivity.playerPosition].vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value);

                return true;
            }
            else
            {
                Log.Error("CheckInitialPathIntersectionCirularly", "Error in path");
                Toast.MakeText(gameActivity, "Error in path", ToastLength.Long).Show();

                return false;
            }
        }

        private void CheckPathIntercetion()
        {
            gameActivity.player[gameActivity.playerPosition].verticesNode = gameActivity.player[gameActivity.playerPosition].vertices.First.Next;

            while (true)
            {
                if (maths.DoIntersect(gameActivity.player[gameActivity.playerPosition].vertices.Last.Previous.Value, gameActivity.player[gameActivity.playerPosition].vertices.Last.Value, gameActivity.player[gameActivity.playerPosition].verticesNode.Previous.Value, gameActivity.player[gameActivity.playerPosition].verticesNode.Value))
                {
                    AmendPath(gameActivity.player[gameActivity.playerPosition].vertices.Last.Previous.Value, gameActivity.player[gameActivity.playerPosition].vertices.Last.Value, gameActivity.player[gameActivity.playerPosition].verticesNode.Previous.Value, gameActivity.player[gameActivity.playerPosition].verticesNode.Value);

                    break;
                }

                if (gameActivity.player[gameActivity.playerPosition].verticesNode.Next != null)
                {
                    gameActivity.player[gameActivity.playerPosition].verticesNode = gameActivity.player[gameActivity.playerPosition].verticesNode.Next;
                }
                else
                {
                    break;
                }
            }
        }

        private void AmendPath(LatLng p1, LatLng p2, LatLng q1, LatLng q2)
        {
            LinkedList<LatLng> temporaryVertices = new LinkedList<LatLng>();

            gameActivity.player[gameActivity.playerPosition].verticesNode = gameActivity.player[gameActivity.playerPosition].vertices.First;

            while (true)
            {
                if (gameActivity.player[gameActivity.playerPosition].verticesNode.Value != q2)
                {
                    temporaryVertices.AddLast(gameActivity.player[gameActivity.playerPosition].verticesNode.Value);
                }
                else
                {
                    break;
                }
                
                if (gameActivity.player[gameActivity.playerPosition].verticesNode.Next != null)
                {
                    gameActivity.player[gameActivity.playerPosition].verticesNode = gameActivity.player[gameActivity.playerPosition].verticesNode.Next;
                }
                else
                {
                    break;
                }
            }

            temporaryVertices.AddLast(maths.LineIntersectionPoint(p1, p2, q1, q2));

            temporaryVertices.AddLast(p2);

            gameActivity.player[gameActivity.playerPosition].vertices = temporaryVertices;
        }

        private void ResetPath(bool positionBool)
        {
            if (gameActivity.player[gameActivity.playerPosition].drawingBool != false)
            {
                gameActivity.player[gameActivity.playerPosition].verticesNode = gameActivity.player[gameActivity.playerPosition].vertices.AddLast(gameActivity.player[gameActivity.playerPosition].currentPosition).Previous;

                FindFinalPathIntersection();

                utilities.SetPolyline(gameActivity.player[gameActivity.playerPosition].vertices);

                BuildArea();

                gameActivity.player[gameActivity.playerPosition].polyline = null;

                gameActivity.player[gameActivity.playerPosition].drawingBool = false;
            }

            gameActivity.player[gameActivity.playerPosition].vertices.Clear();

            if (!positionBool)
            {
                gameActivity.player[gameActivity.playerPosition].verticesNode = gameActivity.player[gameActivity.playerPosition].vertices.AddFirst(gameActivity.player[gameActivity.playerPosition].currentPosition);
            }
        }

        private void FindFinalPathIntersection()
        {
            gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First.Next;

            while (true)
            {
                if (maths.DoIntersect(gameActivity.player[gameActivity.playerPosition].vertices.Last.Previous.Value, gameActivity.player[gameActivity.playerPosition].vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value))
                {
                    gameActivity.player[gameActivity.playerPosition].vertices.Last.Value = maths.LineIntersectionPoint(gameActivity.player[gameActivity.playerPosition].vertices.Last.Previous.Value, gameActivity.player[gameActivity.playerPosition].vertices.Last.Value, gameActivity.playArea.verticesNode.Previous.Value, gameActivity.playArea.verticesNode.Value);

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
            if (maths.DoIntersect(gameActivity.player[gameActivity.playerPosition].vertices.Last.Previous.Value, gameActivity.player[gameActivity.playerPosition].vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value))
            {
                gameActivity.player[gameActivity.playerPosition].vertices.Last.Value = maths.LineIntersectionPoint(gameActivity.player[gameActivity.playerPosition].vertices.Last.Previous.Value, gameActivity.player[gameActivity.playerPosition].vertices.Last.Value, gameActivity.playArea.vertices.First.Value, gameActivity.playArea.vertices.Last.Value);

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
            if(CheckPlayAreaIntersection(gameActivity.player[gameActivity.playerPosition].vertices.First.Value))
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
            if (CheckPlayAreaIntersection(gameActivity.player[gameActivity.playerPosition].vertices.Last.Value))
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
            gameActivity.player[gameActivity.playerPosition].verticesNode = gameActivity.player[gameActivity.playerPosition].vertices.First;

            while (true)
            {
                firstPolygonVertices.AddLast(gameActivity.player[gameActivity.playerPosition].verticesNode.Value);
                secondPolygonVertices.AddLast(gameActivity.player[gameActivity.playerPosition].verticesNode.Value);

                if (gameActivity.player[gameActivity.playerPosition].verticesNode.Next != null)
                {
                    gameActivity.player[gameActivity.playerPosition].verticesNode = gameActivity.player[gameActivity.playerPosition].verticesNode.Next;
                }
                else
                {
                    break;
                }
            }
        }

        private void AddFirstPolygon(LinkedList<LatLng> firstPolygonVertices)
        {
            if (gameActivity.playArea.vertices.Find(gameActivity.player[gameActivity.playerPosition].vertices.Last.Value).Next != null)
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.Find(gameActivity.player[gameActivity.playerPosition].vertices.Last.Value).Next;
            }
            else
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.First;
            }

            while (true)
            {
                if (gameActivity.playArea.verticesNode.Value != gameActivity.player[gameActivity.playerPosition].vertices.First.Value)
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
            if (gameActivity.playArea.vertices.Find(gameActivity.player[gameActivity.playerPosition].vertices.Last.Value).Previous != null)
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.Find(gameActivity.player[gameActivity.playerPosition].vertices.Last.Value).Previous;
            }
            else
            {
                gameActivity.playArea.verticesNode = gameActivity.playArea.vertices.Last;
            }

            while (true)
            {
                if (gameActivity.playArea.verticesNode.Value != gameActivity.player[gameActivity.playerPosition].vertices.First.Value)
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
                AddFirstArea(firstArea, secondArea);
            }
            else
            {
                AddSecondArea(secondArea, firstArea);
            }

            UpdatePolygons();
        }

        private void AddFirstArea(double takenArea, double leftArea)
        {
            gameActivity.playArea.polygons.First.Value.Points = gameActivity.playArea.polygons.Last.Value.Points;            

            gameActivity.playArea.polygons.Remove(gameActivity.playArea.polygons.Last.Value);

            UpdateScore(takenArea, leftArea);
        }

        private void AddSecondArea(double takenArea, double leftArea)
        {
            gameActivity.playArea.polygons.First.Value.Points = gameActivity.playArea.polygons.Last.Previous.Value.Points;

            gameActivity.playArea.polygons.Remove(gameActivity.playArea.polygons.Last.Previous.Value);

            UpdateScore(takenArea, leftArea);
        }

        private void UpdateScore(double takenArea, double leftArea)
        {
            gameActivity.area = (int)((leftArea / gameActivity.initialArea) * 100);
            gameActivity.areaTextView.Text = "Area: " + gameActivity.area.ToString();

            gameActivity.player[gameActivity.playerPosition].score += (int)((takenArea / gameActivity.initialArea) * 100);
            gameActivity.scoreTextView.Text = "Score: " + gameActivity.player[gameActivity.playerPosition].score.ToString();
        }

        private void UpdatePolygons()
        {
            gameActivity.playArea.vertices = new LinkedList<LatLng>(gameActivity.playArea.polygons.First.Value.Points);
            gameActivity.playArea.vertices.RemoveLast();

            gameActivity.playArea.polygons.Last.Value.FillColor = Color.HSVToColor(new float[] { utilities.Colour(gameActivity.playerPosition), 1.0f, 1.0f });
        }
    }
}