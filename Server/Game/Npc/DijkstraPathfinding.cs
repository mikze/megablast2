using System;
using System.Collections.Generic;

class DijkstraGrid
{
    private static readonly (int y, int x)[] Directions = 
        { (0, 1), (1, 0), (0, -1), (-1, 0) }; // Right, Down, Left, Up

    public static List<(int y, int x)> Dijkstra(int[,] grid, (int y, int x) start, (int y, int x) end)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Distance array, initialized to max value
        int[,] distance = new int[rows, cols];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                distance[i, j] = int.MaxValue;

        // Parent dictionary to reconstruct the path
        var parent = new Dictionary<(int y, int x), (int y, int x)>();

        // Min-Heap Priority Queue for Dijkstra using a SortedSet for simplicity
        var priorityQueue = new SortedSet<(int cost, int y, int x)>(Comparer<(int, int, int)>.Create((a, b) =>
        {
            int result = a.cost.CompareTo(b.cost);
            if (result == 0)
                result = a.y.CompareTo(b.y);
            if (result == 0)
                result = a.x.CompareTo(b.x);
            return result;
        }));

        // Initialize starting point
        distance[start.y, start.x] = 0;
        priorityQueue.Add((0, start.y, start.x));

        // Processing loop
        while (priorityQueue.Count > 0)
        {
            var current = priorityQueue.Min;
            priorityQueue.Remove(current);

            int currentY = current.y;
            int currentX = current.x;
            int currentCost = current.cost;

            // If the end point is reached, stop
            if ((currentY, currentX) == end)
                break;

            // Explore neighbors
            foreach (var (dy, dx) in Directions)
            {
                int newY = currentY + dy;
                int newX = currentX + dx;

                // Check bounds and if the cell is not a wall
                if (newY >= 0 && newY < rows && newX >= 0 && newX < cols &&
                    (grid[newY, newX] == 0 || grid[newY, newX] == 9))
                {
                    int newCost = currentCost + 1; // Distance increment is 1 for each step
                    
                    // Relaxation step
                    if (newCost < distance[newY, newX])
                    {
                        // Update distance and re-add to the priority queue
                        if (distance[newY, newX] != int.MaxValue)
                            priorityQueue.Remove((distance[newY, newX], newY, newX));
                            
                        distance[newY, newX] = newCost;
                        parent[(newY, newX)] = (currentY, currentX); // Track parent
                        priorityQueue.Add((newCost, newY, newX));
                    }
                }
            }
        }

        // Reconstruct path
        var path = new List<(int y, int x)>();
        var currentPoint = end;

        // Work backwards from end to start using the parent map
        while (currentPoint != start)
        {
            if (!parent.ContainsKey(currentPoint))
            {
                // If no valid path exists, return an empty list
                return new List<(int y, int x)>();
            }

            path.Add(currentPoint);
            currentPoint = parent[currentPoint];
        }

        // Add the starting point and reverse the path to get the correct order
        path.Add(start);
        path.Reverse();

        return path;
    }

    // Utility to print the grid
    public static void PrintGrid(int[,] grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
                Console.Write(grid[i, j] + "\t");
            Console.WriteLine();
        }
    }
}

// // Example usage
// class Program
// {
//     static void Main(string[] args)
//     {
//         int[,] grid = {
//             { 0, 1, 0, 0, 9 },
//             { 0, 1, 4, 1, 0 },
//             { 9, 0, 0, 1, 0 },
//             { 1, 1, 0, 0, 0 },
//             { 0, 0, 9, 1, 9 }
//         };
//
//         var start = (y: 0, x: 0); // Starting point
//         var end = (y: 4, x: 4);   // Target point
//         
//         var path = DijkstraGrid.Dijkstra(grid, start, end);
//
//         Console.WriteLine("Shortest Path:");
//         foreach (var step in path)
//         {
//             Console.WriteLine($"({step.y}, {step.x})");
//         }
//     }
// }