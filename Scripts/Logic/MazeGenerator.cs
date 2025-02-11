using System;
using System.Collections.Generic;
using System.Linq;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts.Logic;

public class MazeGenerator
{
    public (int x, int y)[] Directions { get; private set; } = { (2, 0), (-2, 0), (0, 2), (0, -2) };
    public Tile[,] Maze { get; private set; }
    public int Size { get; }
    public int Seed { get; }
    public (int x, int y) SpawnerCoord { get; private set; }
    public (int x, int y) ExitCoord { get; private set; }

    private Random _random { get; set; }
    private List<(int x, int y)> _emptyCoords = new();
    private List<(int x, int y)> _trapCoords = new();

    public MazeGenerator(int size, int seed, bool isRandomSeed)
    {
        if (size <= 5) size = 5;
        else Size = size % 2 == 0 ? size + 1 : size;

        Seed = isRandomSeed ? (int)DateTime.Now.Ticks : seed;
        _random = new Random(Seed);
    }
    public bool IsInsideBounds(int x, int y) => x >= 0 && y >= 0 && x < Maze.GetLength(0) && y < Maze.GetLength(1);

    private void GenerateMazeRandomizedDfs((int x, int y) currentCoord, bool[,] maskVisitedCoords)
    {
        maskVisitedCoords[currentCoord.x, currentCoord.y] = true;
        Maze[currentCoord.x, currentCoord.y] = new Empty(currentCoord.x, currentCoord.y);
        foreach ((int x, int y) in Shuffle(Directions))
        {
            (int x, int y) neighbourCoord = (x + currentCoord.x, y + currentCoord.y);
            if (IsInsideBounds(neighbourCoord.x, neighbourCoord.y) &&
                !maskVisitedCoords[neighbourCoord.x, neighbourCoord.y])
            {
                Maze[neighbourCoord.x, neighbourCoord.y] = new Empty(neighbourCoord.x, neighbourCoord.y);

                (int x, int y) inBetweenCoord = (currentCoord.x + (int)(x * 0.5),
                    currentCoord.y + (int)(y * 0.5));
                Maze[inBetweenCoord.x, inBetweenCoord.y] = new Empty(inBetweenCoord.x, inBetweenCoord.y);
                maskVisitedCoords[inBetweenCoord.x, inBetweenCoord.y] = true;
                GenerateMazeRandomizedDfs(neighbourCoord, maskVisitedCoords);
            }
        }
    }
    private (int x, int y)[] Shuffle((int x, int y)[] coordsArray)
    {
        var newCoordsArray = coordsArray.ToArray();
        for (int i = coordsArray.Length - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (newCoordsArray[j], newCoordsArray[i]) = (newCoordsArray[i], newCoordsArray[j]);
        }
        return newCoordsArray;
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
        GenerateTraps(5);
    }
    private (int x, int y) GetInitialCoord()
    {
        int x;
        int y;
        do
        {
            x = _random.Next(Size);
            y = _random.Next(Size);
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

        int index = _random.Next(possibleCoords.Count);
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

        int index = _random.Next(possibleCoords.Count);
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
                _emptyCoords.Add((i, j));
            }
        }
    }
    private void GenerateTraps(float percentage)
    {
        GenerateSpikesTraps(percentage * 0.333f);
        GenerateStickyTraps(percentage * 0.333f);
        GenerateTrampolineTraps(percentage * 0.333f);
    }
    private void GenerateSpikesTraps(float percentage)
    {
        int num = (int)Math.Floor(_emptyCoords.Count * percentage / 100);

        for (int i = 0; i <= num; i++)
        {
            int index = _random.Next(_emptyCoords.Count);
            Spikes spikes = new(_emptyCoords[index].x, _emptyCoords[index].y, true);
            if (Maze[_emptyCoords[index].x, _emptyCoords[index].y] is Spikes or Portal or Shock) continue;
            if (Maze[_emptyCoords[index].x, _emptyCoords[index].y] is Wall) continue;
            Maze[_emptyCoords[index].x, _emptyCoords[index].y] = spikes;
            _trapCoords.Add((spikes.X, spikes.Y));
        }
    }
    private void GenerateTrampolineTraps(float percentage)
    {
        int num = (int)Math.Floor(_emptyCoords.Count * percentage / 100);

        int i = 0;
        while (i <= num)
        {
            int index = _random.Next(_emptyCoords.Count);
            Portal trampoline = new(_emptyCoords[index].x, _emptyCoords[index].y, true);
            if (Maze[_emptyCoords[index].x, _emptyCoords[index].y] is Spikes or Portal or Shock) continue;
            if (Maze[_emptyCoords[index].x, _emptyCoords[index].y] is Wall) continue;

            int numWalls = 0;
            (int x, int y)[] dir = { (0, 1), (0, -1), (1, 0), (-1, 0) };
            foreach (var (x, y) in Directions)
            {
                (int x, int y) nCoord = (_emptyCoords[index].x + x, _emptyCoords[index].y + y);
                if (IsInsideBounds(nCoord.x, nCoord.y) && Maze[nCoord.x, nCoord.y] is Empty or Spikes or Portal)
                {
                    (int x, int y) inBetweenCoord;
                    inBetweenCoord = ((int)((nCoord.x + _emptyCoords[index].x) * 0.5f), (int)((nCoord.y + _emptyCoords[index].y) * 0.5f));
                    if (Maze[inBetweenCoord.x, inBetweenCoord.y] is Wall)
                    {
                        numWalls++;
                    }
                }
            }

            if (numWalls > 1)
            {
                Maze[_emptyCoords[index].x, _emptyCoords[index].y] = trampoline;
                _trapCoords.Add((trampoline.X, trampoline.Y));
                i++;
            }
        }
    }
    private void GenerateStickyTraps(float percentage)
    {
        int num = (int)Math.Floor(_emptyCoords.Count * percentage / 100);

        for (int i = 0; i <= num; i++)
        {
            int index = _random.Next(_emptyCoords.Count);
            Shock sticky = new(_emptyCoords[index].x, _emptyCoords[index].y, true);
            if (Maze[_emptyCoords[index].x, _emptyCoords[index].y] is Spikes or Portal or Shock) continue;
            if (Maze[_emptyCoords[index].x, _emptyCoords[index].y] is Wall) continue;
            Maze[_emptyCoords[index].x, _emptyCoords[index].y] = sticky;
            _trapCoords.Add((sticky.X, sticky.Y));
        }
    }
}