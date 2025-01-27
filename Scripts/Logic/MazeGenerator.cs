//TODO: Error con el generador de laberintos (prueba con semilla: 757191438, -379204873, -1124538791, -2130386984, 20293741, 332626077).

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts.Logic;

public class MazeGenerator
{
    public (int x, int y)[] Directions { get; private set; } = { (2, 0), (-2, 0), (0, 2), (0, -2) };
    public Tile[,] Maze { get; private set; }
    public int Size { get; }
    public int Seed { get; }
    public (int x, int y) SpawnerCoord;
    public (int x, int y) ExitCoord { get; private set; }
    public Random Random { get; private set; }
    private List<(int x, int y)> emptyCoords = new();
    public List<(int x, int y)> trapCoords = new();

    public MazeGenerator(int size, int seed, bool isRandomSeed)
    {
        Size = size % 2 == 0 ? size + 1 : size;
        Seed = isRandomSeed ? (int)DateTime.Now.Ticks : seed;
        Random = new Random(Seed);
    }

    public bool IsInsideBounds(int x, int y) => x >= 0 && y >= 0 && x < Maze.GetLength(0) && y < Maze.GetLength(1);

    private void GenerateMazeRandomizedDfs((int x, int y) currentCoord, bool[,] maskVisitedCoords)
    {
        maskVisitedCoords[currentCoord.x, currentCoord.y] = true;
        Maze[currentCoord.x, currentCoord.y] = new Empty(currentCoord.x, currentCoord.y);
        Shuffle(Directions);
        foreach ((int x, int y) in Directions)
        {
            (int x, int y) neighbourCoord = (x + currentCoord.x, y + currentCoord.y);
            if (IsInsideBounds(neighbourCoord.x, neighbourCoord.y) &&
                !maskVisitedCoords[neighbourCoord.x, neighbourCoord.y])
            {
                Maze[neighbourCoord.x, neighbourCoord.y] = new Empty(neighbourCoord.x, neighbourCoord.y);

                (int x, int y) inBetweenCoord = ((neighbourCoord.x + currentCoord.x) / 2,
                    (neighbourCoord.y + currentCoord.y) / 2);
                Maze[inBetweenCoord.x, inBetweenCoord.y] = new Empty(inBetweenCoord.x, inBetweenCoord.y);

                GenerateMazeRandomizedDfs(neighbourCoord, maskVisitedCoords);
            }
        }
    }

    private void Shuffle((int x, int y)[] coordsArray)
    {
        for (int i = coordsArray.Length - 1; i > 0; i--)
        {
            int j = Random.Next(0, i + 1);
            (coordsArray[j], coordsArray[i]) = (coordsArray[i], coordsArray[j]);
        }
    }

    public void GenerateMaze()
    {
        Maze = new Tile[Size, Size];
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                Maze[x, y] = new Wall(x, y);
            }
        }

        bool[,] maskVisitedCoords = new bool[Size, Size];
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                maskVisitedCoords[x, y] = false;
            }
        }

        GenerateMazeRandomizedDfs(GetInitialCoord(), maskVisitedCoords);
        GetEmptyCoords();
        GenerateSpawner();
        GenerateExit();
        GenerateTraps(3);
    }

    private (int x, int y) GetInitialCoord()
    {
        int x;
        int y;
        do
        {
            x = Random.Next(Size);
            y = Random.Next(Size);
        } while (x % 2 == 0 || y % 2 == 0);

        return (x, y);
    }

    private void GenerateExit()
    {
        List<(int x, int y)> possibleCoords = new();

        for (int i = 0; i < Size; i++)
        {
            if (Maze[i, 0] is Wall && Maze[i, 1] is Empty or Portal or Spikes)
            {
                (float x, float y) distanceBetweenSpawnerExit;
                distanceBetweenSpawnerExit = (Math.Abs(SpawnerCoord.x - i), Math.Abs(SpawnerCoord.y));
                if (distanceBetweenSpawnerExit.x > Size / 2 && distanceBetweenSpawnerExit.y > Size / 2)
                {
                    possibleCoords.Add((i, 0));
                }

            }
            if (Maze[0, i] is Wall && Maze[1, i] is Empty or Portal or Spikes)
            {
                (float x, float y) distanceBetweenSpawnerExit;
                distanceBetweenSpawnerExit = (Math.Abs(SpawnerCoord.x), Math.Abs(SpawnerCoord.y - i));
                if (distanceBetweenSpawnerExit.x > Size / 2 && distanceBetweenSpawnerExit.y > Size / 2)
                {
                    possibleCoords.Add((0, i));
                }
            }
            if (Maze[i, Size - 1] is Wall && Maze[i, Size - 2] is Empty or Portal or Spikes)
            {
                (float x, float y) distanceBetweenSpawnerExit;
                distanceBetweenSpawnerExit = (Math.Abs(SpawnerCoord.x - i), Math.Abs(SpawnerCoord.y - Size - 1));
                if (distanceBetweenSpawnerExit.x > Size / 2 && distanceBetweenSpawnerExit.y > Size / 2)
                {
                    possibleCoords.Add((i, Size - 1));
                }
            }
            if (Maze[Size - 1, i] is Wall && Maze[Size - 2, i] is Empty or Portal or Spikes)
            {
                (float x, float y) distanceBetweenSpawnerExit;
                distanceBetweenSpawnerExit = (Math.Abs(SpawnerCoord.x - Size - 1), Math.Abs(SpawnerCoord.y - i));
                if (distanceBetweenSpawnerExit.x > Size / 2 && distanceBetweenSpawnerExit.y > Size / 2)
                {
                    possibleCoords.Add((Size - 1, i));
                }
            }
        }

        int index = Random.Next(possibleCoords.Count);
        ExitCoord = (possibleCoords[index].x, possibleCoords[index].y);
        Maze[ExitCoord.x, ExitCoord.y] = new Exit(ExitCoord.x, ExitCoord.y);
    }

    private void GenerateSpawner()
    {
        List<(int x, int y)> possibleCoords = new();
        for (int i = 0; i < Size; i++)
        {
            if (Maze[i, 1] is Empty)
            {
                possibleCoords.Add((i, 0));
            }

            if (Maze[1, i] is Empty)
            {
                possibleCoords.Add((0, i));
            }

            if (Maze[i, Size - 1 - 1] is Empty)
            {
                possibleCoords.Add((i, Size - 1));
            }

            if (Maze[Size - 1 - 1, i] is Empty)
            {
                possibleCoords.Add((Size - 1, i));
            }
        }

        int index = Random.Next(possibleCoords.Count);
        SpawnerCoord = (possibleCoords[index].x, possibleCoords[index].y);
        Maze[SpawnerCoord.x, SpawnerCoord.y] = new Spawner(SpawnerCoord.x, SpawnerCoord.y);
    }

    private void GetEmptyCoords()
    {
        for (int i = 1; i < Size - 1; i++)
        {
            for (int j = 1; j < Size - 1; j++)
            {
                if (Maze[i, j] is Wall) continue;
                emptyCoords.Add((i, j));
            }
        }
    }

    private void GenerateTraps(float percentage)
    {
        GenerateSpikesTraps(percentage / 3);
        GenerateStickyTraps(percentage / 3);
        GenerateTrampolineTraps(percentage / 3);
    }

    private void GenerateSpikesTraps(float percentage)
    {
        int num = (int)Math.Floor(emptyCoords.Count * percentage / 100);

        for (int i = 0; i <= num; i++)
        {
            int index = Random.Next(emptyCoords.Count);
            Spikes spikes = new(emptyCoords[index].x, emptyCoords[index].y, true);
            if (Maze[emptyCoords[index].x, emptyCoords[index].y] is Spikes or Portal or Sticky) continue;
            if (Maze[emptyCoords[index].x, emptyCoords[index].y] is Wall) continue;
            Maze[emptyCoords[index].x, emptyCoords[index].y] = spikes;
            trapCoords.Add((spikes.X, spikes.Y));
        }
    }

    private void GenerateTrampolineTraps(float percentage)
    {
        int num = (int)Math.Floor(emptyCoords.Count * percentage / 100);

        int i = 0;
        while (i <= num)
        {
            int index = Random.Next(emptyCoords.Count);
            Portal trampoline = new(emptyCoords[index].x, emptyCoords[index].y, true);
            if (Maze[emptyCoords[index].x, emptyCoords[index].y] is Spikes or Portal or Sticky) continue;
            if (Maze[emptyCoords[index].x, emptyCoords[index].y] is Wall) continue;

            int numWalls = 0;
            (int x, int y)[] dir = { (0, 1), (0, -1), (1, 0), (-1, 0) };
            foreach (var (x, y) in Directions)
            {
                (int x, int y) nCoord = (emptyCoords[index].x + x, emptyCoords[index].y + y);
                if (IsInsideBounds(nCoord.x, nCoord.y) && Maze[nCoord.x, nCoord.y] is Empty or Spikes or Portal)
                {
                    (int x, int y) inBetweenCoord;
                    inBetweenCoord = ((int)((nCoord.x + emptyCoords[index].x) * 0.5f), (int)((nCoord.y + emptyCoords[index].y) * 0.5f));
                    if (Maze[inBetweenCoord.x, inBetweenCoord.y] is Wall)
                    {
                        numWalls++;
                    }
                }
            }

            if (numWalls > 1)
            {
                Maze[emptyCoords[index].x, emptyCoords[index].y] = trampoline;
                trapCoords.Add((trampoline.X, trampoline.Y));
                i++;
            }
        }
    }

    private void GenerateStickyTraps(float percentage)
    {
        int num = (int)Math.Floor(emptyCoords.Count * percentage / 100);

        for (int i = 0; i <= num; i++)
        {
            int index = Random.Next(emptyCoords.Count);
            Sticky sticky = new(emptyCoords[index].x, emptyCoords[index].y, true);
            if (Maze[emptyCoords[index].x, emptyCoords[index].y] is Spikes or Portal or Sticky) continue;
            if (Maze[emptyCoords[index].x, emptyCoords[index].y] is Wall) continue;
            Maze[emptyCoords[index].x, emptyCoords[index].y] = sticky;
            trapCoords.Add((sticky.X, sticky.Y));
        }
    }
}