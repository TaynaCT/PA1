using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.SupportClasses;
using Assets.Scripts.Map;
using Assets.Scripts.Player;

namespace Assets.Scripts.AI
{
    public static class Dijkstra
    {
        private struct Node
        {
            public Indice Here { get; set; }
            public Indice From { get; set; }
            public float Cost { get; set; }

            public Node(Indice here, Indice from, float cost)
            {
                Here = here;
                From = from;
                Cost = cost;
            }
        }

        public static List<Indice> GetShortestPath(Matrix board, Indice start, Indice end, MovementType movementType)
        {
            Dictionary<Indice, Node> open = new Dictionary<Indice, Node>();
            Dictionary<Indice, Node> closed = new Dictionary<Indice, Node>();
            Node current = new Node(start, start, 0);

            open.Add(start, current);

            while (open.Count > 0)
            {
                //current = open.Aggregate((a, b) => a.Value.Cost < b.Value.Cost ? a : b).Value;

                current.Cost = float.PositiveInfinity;

                foreach (var pair in open)
                {
                    if (pair.Value.Cost < current.Cost)
                    {
                        current = pair.Value;
                    }
                }

                if (current.Here == end) break;

                List<Indice> adjacents = board.GetAdjTile(current.Here.X, current.Here.Y);
               
                for (int i = 0; i < adjacents.Count; i++)
                {
                    Node processing = new Node(adjacents[i], current.Here, 1 / board.GetWalkSpeed(adjacents[i], movementType));

                    if (float.IsInfinity(processing.Cost) || !board.CanMoveTo(start, processing.Here))
                    {
                        if (!closed.ContainsKey(processing.Here))
                        {
                            closed.Add(processing.Here, processing);
                        }
                        continue;
                    }

                    if (closed.ContainsKey(processing.Here)) continue;

                    processing.Cost += current.Cost;

                    if (!open.ContainsKey(processing.Here))
                    {
                        open.Add(processing.Here, processing);
                    }
                }

                open.Remove(current.Here);
                closed.Add(current.Here, current);
            }

            List<Indice> path = new List<Indice>();

            while (current.Here != current.From)
            {
                path.Add(current.Here);
                current = closed[current.From];
            }

            path.Reverse();

            return path;
        }

        public static List<Indice> TilesWithinDistance(Matrix board, Indice start, MovementType movementType, float distance)
        {
            List<Indice> range = new List<Indice>();
            Dictionary<Indice, Node> open = new Dictionary<Indice, Node>();
            Dictionary<Indice, Node> closed = new Dictionary<Indice, Node>();
            Node current = new Node(start, start, 0);

            open.Add(start, current);

            while (open.Count > 0)
            {
                //current = open.Aggregate((a, b) => a.Value.Cost < b.Value.Cost ? a : b).Value;

                current.Cost = float.PositiveInfinity;

                foreach (var pair in open)
                {
                    if (pair.Value.Cost < current.Cost)
                    {
                        current = pair.Value;
                    }
                }

                if (current.Cost > distance)
                {
                    open.Remove(current.Here);
                    closed.Add(current.Here, current);
                    continue;
                }

                List<Indice> adjacents = board.GetAdjTile(current.Here.X, current.Here.Y);
                for (int i = 0; i < adjacents.Count; i++)
                {
                    Node processing = new Node(adjacents[i], current.Here, 1 / board.GetWalkSpeed(adjacents[i], movementType));

                    if (float.IsInfinity(processing.Cost) || !board.CanMoveTo(start, processing.Here))
                    {
                        if (!closed.ContainsKey(processing.Here))
                        {
                            closed.Add(processing.Here, processing);
                        }
                        continue;
                    }

                    if (closed.ContainsKey(processing.Here)) continue;

                    processing.Cost += current.Cost;

                    if (!open.ContainsKey(processing.Here))
                    {
                        open.Add(processing.Here, processing);
                    }
                }

                open.Remove(current.Here);
                closed.Add(current.Here, current);
                range.Add(current.Here);
            }

            return range;
        }

        public static List<Indice> TilesWithinDistanceRange(Matrix board, Indice start, MovementType movementType, float minDistance, float maxDistance)
        {
            List<Indice> range = new List<Indice>();
            Dictionary<Indice, Node> open = new Dictionary<Indice, Node>();
            Dictionary<Indice, Node> closed = new Dictionary<Indice, Node>();
            Node current = new Node(start, start, 0);

            open.Add(start, current);

            while (open.Count > 0)
            {
                //current = open.Aggregate((a, b) => a.Value.Cost < b.Value.Cost ? a : b).Value;

                current.Cost = float.PositiveInfinity;

                foreach (var pair in open)
                {
                    if (pair.Value.Cost < current.Cost)
                    {
                        current = pair.Value;
                    }
                }

                if (current.Cost > maxDistance)
                {
                    open.Remove(current.Here);
                    closed.Add(current.Here, current);
                    continue;
                }

                List<Indice> adjacents = board.GetAdjTile(current.Here.X, current.Here.Y);
                for (int i = 0; i < adjacents.Count; i++)
                {
                    Node processing = new Node(adjacents[i], current.Here, 1 / board.GetWalkSpeed(adjacents[i], movementType));

                    if (float.IsInfinity(processing.Cost) || !board.CanMoveTo(start, processing.Here))
                    {
                        if (!closed.ContainsKey(processing.Here))
                        {
                            closed.Add(processing.Here, processing);
                        }
                        continue;
                    }

                    if (closed.ContainsKey(processing.Here)) continue;

                    processing.Cost += current.Cost;

                    if (!open.ContainsKey(processing.Here))
                    {
                        open.Add(processing.Here, processing);
                    }
                }

                open.Remove(current.Here);
                closed.Add(current.Here, current);

                if (current.Cost >= minDistance)
                {
                    range.Add(current.Here);
                }
            }

            return range;
        }

        public static float GetTotalPathLength(Matrix board, List<Indice> path, MovementType movementType)
        {
            float total = 0;

            for (int i = 0; i < path.Count; i++)
            {
                total += 1 / board.GetWalkSpeed(path[i], movementType);
            }

            return total;
        }

        public static Indice GetLastTileInRange(Matrix board, List<Indice> path, MovementType movementType, float distance)
        {
            if (path.Count > 0)
            {
                int index = path.Count - 1;

                for (int i = 0; i < path.Count; i++)
                {
                    distance -= 1 / board.GetWalkSpeed(path[i], movementType);

                    if (distance < 0)
                    {
                        index = i - 1;
                        break;
                    }
                }

                while (!board.CanMoveTo(path[0], path[index]) && index > 0)
                {
                    index--;
                }

                return path[index];
            }

            return default(Indice);
        }

        public static bool IsWithinDistance(Matrix board, Indice start, Indice end, MovementType movementType, float distance)
        {
            Dictionary<Indice, Node> open = new Dictionary<Indice, Node>();
            Dictionary<Indice, Node> closed = new Dictionary<Indice, Node>();
            Node current = new Node(start, start, 0);

            open.Add(start, current);

            while (open.Count > 0)
            {
                current.Cost = float.PositiveInfinity;

                foreach (var pair in open)
                {
                    if (pair.Value.Cost < current.Cost)
                    {
                        current = pair.Value;
                    }
                }

                if (current.Cost > distance)
                {
                    open.Remove(current.Here);
                    closed.Add(current.Here, current);
                    continue;
                }

                if (current.Here == end) return true;

                List<Indice> adjacents = board.GetAdjTile(current.Here.X, current.Here.Y);
                for (int i = 0; i < adjacents.Count; i++)
                {
                    Node processing = new Node(adjacents[i], current.Here, 1 / board.GetWalkSpeed(adjacents[i], movementType));

                    if (float.IsInfinity(processing.Cost) || !board.CanMoveTo(start, processing.Here))
                    {
                        if (!closed.ContainsKey(processing.Here))
                        {
                            closed.Add(processing.Here, processing);
                        }
                        continue;
                    }

                    if (closed.ContainsKey(processing.Here)) continue;

                    processing.Cost += current.Cost;

                    if (!open.ContainsKey(processing.Here))
                    {
                        open.Add(processing.Here, processing);
                    }
                }

                open.Remove(current.Here);
                closed.Add(current.Here, current);
            }

            return false;
        }

        public static float GetShortestPathLength(Matrix board, Indice start, Indice end, MovementType movementType)
        {
            return GetTotalPathLength(board, GetShortestPath(board, start, end, movementType), movementType);
        }
    }
}