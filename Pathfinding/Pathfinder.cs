using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinding
{
    public static class Pathfinder
    {
        public static List<Tuple<int, int>> FindPath(bool[,] walkableMap, Tuple<int,int> start, Tuple<int, int> end, int limit = 20)
        {
            // Ported from pseudocode on https://en.wikipedia.org/wiki/A*_search_algorithm
            // Readily copy pasted in
            var width = walkableMap.GetLength(0);
            var height = walkableMap.GetLength(1);

            //    // The set of nodes already evaluated.
            //    closedSet := {}
            var closedSet = new List<Tuple<int, int>>();
            //    // The set of currently discovered nodes that are not evaluated yet.
            //    // Initially, only the start node is known.
            //    openSet := {start}
            var openSet = new List<Tuple<int, int>>();
            openSet.Add(start);
            //    // For each node, which node it can most efficiently be reached from.
            //    // If a node can be reached from many nodes, cameFrom will eventually contain the
            //    // most efficient previous step.
            //    cameFrom := the empty map
            var cameFrom = new Dictionary<Tuple<int, int>, Tuple<int, int>>();

            //    // For each node, the cost of getting from the start node to that node.
            //    gScore := map with default value of Infinity
            var gscore = new int[width, height];
            for (var ix = 0; ix < width; ++ix)
            {
                for (var iy = 0; iy < height; ++iy)
                {
                    gscore[ix, iy] = int.MaxValue;
                }
            }
            //    // The cost of going from start to start is zero.
            //    gScore[start] := 0 
            gscore[start.Item1, start.Item2] = 0;
            //    // For each node, the total cost of getting from the start node to the goal
            //    // by passing by that node. That value is partly known, partly heuristic.
            //    fScore := map with default value of Infinity
            var fscore = new int[width, height];
            for (var ix = 0; ix < width; ++ix)
            {
                for (var iy = 0; iy < height; ++iy)
                {
                    fscore[ix, iy] = int.MaxValue;
                }
            }
            //    // For the first node, that value is completely heuristic.
            //    fScore[start] := heuristic_cost_estimate(start, goal)
            fscore[start.Item1, start.Item2] = Math.Abs(start.Item1 - end.Item1) + Math.Abs(start.Item2 - end.Item2);

                //    while openSet is not empty
            while (openSet.Count > 0)
            {
                //        current := the node in openSet having the lowest fScore[] value
                var current = openSet.Aggregate((curMin, i) => (fscore[i.Item1, i.Item2] < fscore[curMin.Item1, curMin.Item2]) ? i : curMin);
                //        if current = goal
                if (current.Equals(end))
                {
                    //            return reconstruct_path(cameFrom, current)
                    //    total_path := [current]
                    var totalPath = new List <Tuple<int, int>>();
                    totalPath.Add(current);
                    //    while current in cameFrom.Keys:
                    while (cameFrom.Any(i => i.Key.Equals(current)))
                    {
                        //        current := cameFrom[current]
                        current = cameFrom[current];
                        //        total_path.append(current)
                        totalPath.Insert(0, current);
                    }
                    return totalPath;
                }

                //        openSet.Remove(current)
                openSet.Remove(current);
                //        closedSet.Add(current)
                closedSet.Add(current);
                //        for each neighbor of current
                var neighbours = new List<Tuple<int, int>>();
                neighbours.Add(new Tuple<int, int>(current.Item1 - 1, current.Item2 - 1));
                neighbours.Add(new Tuple<int, int>(current.Item1 + 1, current.Item2 - 1));
                neighbours.Add(new Tuple<int, int>(current.Item1,     current.Item2 - 1));
                neighbours.Add(new Tuple<int, int>(current.Item1 - 1, current.Item2));
                neighbours.Add(new Tuple<int, int>(current.Item1,     current.Item2));
                neighbours.Add(new Tuple<int, int>(current.Item1 - 1, current.Item2 + 1));
                neighbours.Add(new Tuple<int, int>(current.Item1 + 1, current.Item2 + 1));
                neighbours.Add(new Tuple<int, int>(current.Item1,     current.Item2 + 1));
                // Is this more efficient then branches? I dunno!
                neighbours = neighbours.Where(i =>
                                              (i.Item1 >= 0) &&
                                              (i.Item1 < width) &&
                                              (i.Item2 >= 0) &&
                                              (i.Item2 < height) &&
                                              (walkableMap[i.Item1, i.Item2]) &&
                                              (Math.Abs(start.Item1 - end.Item1) + Math.Abs(start.Item2 - end.Item2) <= limit)
                                              ).ToList();
                foreach (var neighbour in neighbours)
                {
                    //            if neighbor in closedSet
                    if (closedSet.Any(i => i.Equals(neighbour)))
                    {
                    //                continue		// Ignore the neighbor which is already evaluated.
                        continue;
                    }
                    //            // The distance from start to a neighbor
                    //            tentative_gScore := gScore[current] + dist_between(current, neighbor)
                    var tentativeGScore = gscore[current.Item1, current.Item2] + 1; //1 is always distance for now
                    //            if neighbor not in openSet	// Discover a new node
                    if (!openSet.Any(i => i.Equals(neighbour)))
                    {
                        //                openSet.Add(neighbor)
                        openSet.Add(neighbour);
                    }
                    //            else if tentative_gScore >= gScore[neighbor]
                    else if (tentativeGScore >= gscore[neighbour.Item1, neighbour.Item2])
                    {
                        //                continue		// This is not a better path.
                        continue;
                    }

                    //            // This path is the best until now. Record it!
                    //            cameFrom[neighbor] := current
                    cameFrom[neighbour] = current;
                    //            gScore[neighbor] := tentative_gScore
                    gscore[neighbour.Item1, neighbour.Item2] = tentativeGScore;
                    //            fScore[neighbor] := gScore[neighbor] + heuristic_cost_estimate(neighbor, goal)
                    fscore[neighbour.Item1, neighbour.Item2] = Math.Abs(neighbour.Item1 - end.Item1) + Math.Abs(neighbour.Item2 - end.Item2);
                }
            }

            return null;
        }
    }
}
