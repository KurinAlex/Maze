using System.Collections;
using System.Collections.Generic;

namespace Maze;

public static class Program
{
    static readonly Random rand = new();
    static int UnvisitedDir(Grid grid, int v, BitArray visited)
    {
        int[] candidates = new int[4];
        int numCandidates = 0;
        foreach (int dir in new int[] { Grid.Up, Grid.Right, Grid.Down, Grid.Left })
        {
            int neighbor = grid.Neighbor(v, dir);
            if (neighbor != Grid.NoVertex && !visited[neighbor])
            {
                candidates[numCandidates++] = dir;
            }
        }
        return numCandidates == 0 ? -1 : candidates[rand.Next(numCandidates)];
    }

    static void GenerateMaze(Grid grid, int v, BitArray visited)
    {
        visited[v] = true;
        for (int dir = UnvisitedDir(grid, v, visited); dir != -1; dir = UnvisitedDir(grid, v, visited))
        {
            grid.AddEdge(v, dir);
            GenerateMaze(grid, grid.Neighbor(v, dir), visited);
        }
    }

    static Grid GenerateMaze(int width, int height, int startCol, int startRow)
    {
        var grid = new Grid(width, height);
        var visited = new BitArray(width * height);
        GenerateMaze(grid, grid.Vertex(startCol, startRow), visited);
        return grid;
    }

    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;

        var grid = GenerateMaze(30, 30, 0, 0);

        Console.Write(grid);

        Console.ReadLine();
    }
}