using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.Helper
{
    // Using my old ugly a star mess

    class UglyPath
    {

        //Required classes
        public class coord
        {
            public int X;
            public int Y;
            public coord() { X = 0; Y = 0; }
            public coord(int _X, int _Y) { X = _X; Y = _Y; }
            public static bool operator ==(coord first, coord second)
            {
                return ((first.X == second.X) && (first.Y == second.Y));
            }
            public static bool operator !=(coord first, coord second)
            {
                return !((first.X == second.X) && (first.Y == second.Y));
            }
        }

        public class workingPathVertex : IComparable<workingPathVertex>
        {
            public coord V;
            public int G;
            public int H;
            public int F() { return G + H; }
            public workingPathVertex parent;
            public workingPathVertex(coord _V, workingPathVertex _Parent, int _G, int _H)
            {
                V = _V;
                parent = _Parent;
                G = _G;
                H = _H;
            }
            public int CompareTo(workingPathVertex that)
            {
                return (that.F() > F()) ? -1 : 1;
            }
        }

        //Functions
        //Relies on two 2d boolean arrays - which can fairly easily be written out.
        //One is CheckCollision(int, int) which returns whether the tile can be collided 
        //  with - and whether the thing on the tile can be collided with. So - players 
        //  would make it return to
        //The other is TileCollides (int,int) which returns whether the tile can be
        //  collided with. Behaviour is controlled from "playercollides" arg
        //Some debugging codes still in there, and the comments are old, barely updated
        //  and possibly flatly wrong.

        public delegate bool TCheckCollision(int x, int y);

        public static List<coord> GetPath(coord start,
            coord end,
            TCheckCollision CheckCollision)
        {
            List<workingPathVertex> openList = new List<workingPathVertex>();
            List<workingPathVertex> closedList = new List<workingPathVertex>();

            workingPathVertex startV = new workingPathVertex(start, null, 0, Manhatten(start, end));
            workingPathVertex V = startV;

            const int STRAIGHT_COST = 10;
            const int DIAG_COST = 14;

            for (int ix = -1; ix < 2; ix++)
            {
                for (int iy = -1; iy < 2; iy++)
                {
                    if (!(ix == 0 && iy == 0))
                    {
                        coord i = new coord(V.V.X + ix, V.V.Y + iy);
                        if (!CheckCollision(i.X, i.Y))
                        {
                            openList.Add(new workingPathVertex(i, V, V.G + (((ix == 0) || (iy == 0)) ? STRAIGHT_COST : DIAG_COST), Manhatten(i, end)));
                        }
                    }
                }
            }

            openList.Remove(V);
            closedList.Add(V);

            bool finished = false;
            bool succeeded = false;
            workingPathVertex endVertex = null;

            bool surroundtarget = false;

            if (CheckCollision(end.X, end.Y))
            {
                surroundtarget = true;
                bool surroundPossible = false;
                for (int ix = -1; ix < 2; ix++)
                {
                    for (int iy = -1; iy < 2; iy++)
                    {
                        if (!(ix == 0 && iy == 0))
                        {
                            coord i = new coord(V.V.X + ix, V.V.Y + iy);
                            surroundPossible = (!CheckCollision(i.X, i.Y));
                        }
                    }
                }
                finished = !surroundPossible;
            };

            //This is lazy looping I know, but it's late, and I'm tired, and it should work
            while (!finished)
            {


                V = openList.Min();
                openList.Remove(V);
                closedList.Add(V);

                ////Debuggy stuff
                if (!surroundtarget)
                {
                    if ((V.V.X == end.X) && (V.V.Y) == end.Y) { finished = true; succeeded = true; endVertex = V; };
                }
                else
                {
                    if ((Math.Abs(V.V.X - end.X) <= 1) && ((Math.Abs(V.V.Y - end.Y) <= 1))) { finished = true; succeeded = true; endVertex = V; };
                }


                //Find lowest F
                for (int ix = -1; ix < 2; ix++)
                {
                    for (int iy = -1; iy < 2; iy++)
                    {
                        if (!(ix == 0 && iy == 0))
                        {
                            coord i = new coord(V.V.X + ix, V.V.Y + iy);
                            workingPathVertex closePath = closedList.Find((f) => { return ((f.V.X == i.X) && (f.V.Y == i.Y)); });
                            if ((closePath == null) && (!CheckCollision(i.X, i.Y)))
                            {
                                workingPathVertex iPath = openList.Find((f) => { return ((f.V.X == i.X) && (f.V.Y == i.Y)); });
                                if (iPath != null)
                                {
                                    //If iPath is already on openList                                
                                    int GviaV = V.G + (((ix == 0) || (iy == 0)) ? STRAIGHT_COST : DIAG_COST);
                                    if (GviaV < iPath.G) { iPath.G = GviaV; iPath.parent = V; }
                                }
                                else
                                {
                                    //If iPath is not on the openlist
                                    iPath = new workingPathVertex(i, V, V.G + (((ix == 0) || (iy == 0)) ? STRAIGHT_COST : DIAG_COST), Manhatten(i, end));
                                    openList.Add(iPath);
                                }
                            }
                        }
                    }
                }
                if (openList.Count <= 0) { finished = true; }
            }

            if (succeeded)
            {
                //Console.WriteLine("SUCCESS: ");
                List<coord> path = new List<coord>();

                workingPathVertex nextVertex = endVertex;
                while (nextVertex != null)
                {
                    //Console.WriteLine(nextVertex.V.X.ToString() + " " + nextVertex.V.Y.ToString());
                    path.Add(nextVertex.V);
                    nextVertex = nextVertex.parent;
                }
                path.Reverse();
                path.RemoveAt(0);
                return path;
            }
            else
            {
                //Console.WriteLine("FAILED");
                return null;
            }



        }

        //Calculates manhatten distance
        static int Manhatten(coord A, coord B)
        {
            return (Math.Abs(A.X - B.X) + Math.Abs(A.Y + B.Y));
        }
    }

}
