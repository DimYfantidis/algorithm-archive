﻿using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;


namespace Graphics
{
    abstract class FloodFill
    {
        // A simple point on the 2 dimensional integer plane.
        private class Point2I
        {
            // Coordinates
            public int x;
            public int y;
            
            public Point2I(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override bool Equals(object? o) 
            {
                if(!(o is Point2I other))
                    return false;
                return this.x.Equals(other.x) && this.y.Equals(other.y);
            }
            
            public override int GetHashCode() 
            {
                return HashCode.Combine(x, y);
            }
        }
        
        // Utility object for comparing List<List<float>> objects.
        private class FloatListEqualityComparer : IEqualityComparer<List<float>>
        {
            public bool Equals(List<float>? one, List<float>? two) 
            {
                if (ReferenceEquals(one, two))
                    return true;
                if (one == null || two == null)
                    return false;
                return one.SequenceEqual(two);
            }

            public int GetHashCode(List<float> list) {
                return list.GetHashCode();
            }
        }
        
        private static bool InBounds(Point2I size, Point2I loc) 
        {
          if (loc.x < 0 || loc.y < 0) {
            return false;
          }
          return loc.x < size.x && loc.y < size.y;
        }

        private static List<Point2I> FindNeighbors(ref List<List<float>> grid, Point2I loc, float oldValue)
        {
            var possibleNeighbors = new List<Point2I>
            {
                new(loc.x, loc.y + 1),
                new(loc.x + 1, loc.y),
                new(loc.x, loc.y - 1),
                new(loc.x - 1, loc.y)
            };
            var neighbors = new List<Point2I>();

            foreach (Point2I possibleNeighbor in possibleNeighbors)
            {
                var size = new Point2I(grid[0].Count, grid.Count);
                var x = possibleNeighbor.x;
                var y = possibleNeighbor.y;
                if (!InBounds(size, possibleNeighbor)) {
                    continue;
                }
                if (grid[x][y].Equals(oldValue)) {
                    neighbors.Add(possibleNeighbor);
                }
            }
            return neighbors;
        }
        
        private static void RecursiveFill(ref List<List<float>> grid, Point2I loc, float oldValue, float newValue) 
        {
            if (oldValue.Equals(newValue)) {
                return;
            }
            grid[loc.x][loc.y] = newValue;

            var possibleNeighbors = FindNeighbors(ref grid, loc, oldValue);
            foreach (Point2I possibleNeighbor in possibleNeighbors) {
                RecursiveFill(ref grid, possibleNeighbor, oldValue, newValue);
            }
        }
        
        private static void  QueueFill(ref List<List<float>> grid, Point2I loc, float oldValue, float newValue) 
        {
            if (oldValue.Equals(newValue)) {
                return;
            }

            var queue = new Queue<Point2I>();
            queue.Enqueue(loc);
            grid[loc.x][loc.y] = newValue;

            while (queue.Count > 0) 
            {
                var currentLoc = queue.Dequeue();
                var possibleNeighbors = FindNeighbors(ref grid, currentLoc, oldValue);
                foreach (Point2I neighbor in possibleNeighbors) 
                {
                    grid[neighbor.x][neighbor.y] = newValue;
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        
        private static void StackFill(ref List<List<float>> grid, Point2I loc, float oldValue, float newValue) 
        {
            if (oldValue.Equals(newValue)) {
                return;
            }

            var stack = new Stack<Point2I>();
            stack.Push(loc);

            while (stack.Count > 0) 
            {
                var currentLoc = stack.Pop();

                var x = currentLoc.x;
                var y = currentLoc.y;

                if (grid[x][y].Equals(oldValue)) 
                {
                    grid[x][y] = newValue;
                    var possibleNeighbors = FindNeighbors(ref grid, currentLoc, oldValue);
                    foreach (Point2I neighbor in possibleNeighbors) {
                        stack.Push(neighbor);
                    }
                }
            }
        }


        private static void Main(string[] args)
        {
            // All neighbouring zeros, adjacent to (1, 1), must be replaced with 3 in the end result.
            var grid = new List<List<float>>
            {
                new(){0, 0, 1, 0, 0},
                new(){0, 0, 1, 0, 0},
                new(){0, 0, 1, 0, 0},
                new(){8, 0, 1, 1, 1},
                new(){0, 0, 0, 0, 0}
            };
            var solutionGrid = new List<List<float>>
            {
                new(){3, 3, 1, 0, 0},
                new(){3, 3, 1, 0, 0},
                new(){3, 3, 1, 0, 0},
                new(){8, 3, 1, 1, 1},
                new(){3, 3, 3, 3, 3}
            };
            var startingPoint = new Point2I(1, 1);
            var gridComparator = new FloatListEqualityComparer();

            var testGrid = new List<List<float>>(grid);
            RecursiveFill(ref testGrid, startingPoint, 0, 3);
            Debug.Assert(testGrid.SequenceEqual(solutionGrid, gridComparator), "Recursive Flood Fill Failed");
            
            testGrid = new List<List<float>>(grid);
            QueueFill(ref testGrid, startingPoint, 0, 3);
            Debug.Assert(testGrid.SequenceEqual(solutionGrid, gridComparator), "Queue Flood Fill Failed");

            testGrid = new List<List<float>>(grid);
            StackFill(ref testGrid, startingPoint, 0, 3);
            Debug.Assert(testGrid.SequenceEqual(solutionGrid, gridComparator), "Stack Flood Fill Failed");

        }
    }
}