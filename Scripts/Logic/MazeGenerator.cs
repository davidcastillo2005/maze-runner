//TODO: Error con el generador de laberintos (prueba con semilla: 757191438, -379204873, -1124538791, -2130386984, 20293741, 332626077).

using System;
using System.Collections.Generic;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts.Logic;

public class MazeGenerator
{
    private readonly (int x, int y)[] _directions = { (2, 0), (-2, 0), (0, 2), (0, -2) };
    public Tile[,] Maze { get; private set; }
    public int Size { get; }
    public int Seed { get; }
    public (int x, int y) SpawnerCoord;
    public (int x, int y) ExitCoord { get; private set; }
    public List<(int x, int y)> SpikesTrapsCoords { get; } = new();
    private readonly Random _random;

    public MazeGenerator(int size, int seed, bool isRandomSeed)
    {
        Size = size % 2 == 0 ? size + 1 : size;
        if (isRandomSeed)
        {
            Seed = (int)DateTime.Now.Ticks;
        }
        else
        {
            Seed = seed;
        }

        _random = new Random(Seed);
    }

    private bool IsInsideBounds((int x, int y) coord) => coord.x >= 0 && coord.y >= 0 &&
                                                         coord.x < Maze.GetLength(0) && coord.y < Maze.GetLength(1);

    private void GenerateMazeRandomizedDfs((int x, int y) currentCoord, bool[,] maskVisitedCoords)
    {
        maskVisitedCoords[currentCoord.x, currentCoord.y] = true;
        Maze[currentCoord.x, currentCoord.y] = new Empty(currentCoord.x, currentCoord.y);
        Shuffle(_directions);
        foreach ((int x, int y) in _directions)
        {
            (int x, int y) neighbourCoord = (x + currentCoord.x, y + currentCoord.y);
            if (IsInsideBounds(neighbourCoord) && !maskVisitedCoords[neighbourCoord.x, neighbourCoord.y])
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
            int j = _random.Next(0, i + 1);
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
        GenerateSpawner();
        GenerateExit();
        GenerateSpikesTrap(15);
    }
    
    private 

    (int x, int y) GetInitialCoord()
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


    void GenerateExit()
    {
        List<(int x, int y)> coords = new();
        for (int i = 0; i < Size; i++)
        {
            if (Maze[i, 0 + 1] is Empty && Maze[i, 0] is not Spawner)
            {
                bool allCoordsAreWalls = true;

                for (int j = 0; j < (Size - 1) / 2; j++)
                {
                    for (int k = 0; k < (Size - 1) / 2; k++)
                    {
                        if (IsInsideBounds((i + j, 0 + k)))
                        {
                            if (Maze[i + j, 0 + k] is Spawner)
                            {
                                allCoordsAreWalls = false;
                                break;
                            }
                        }

                        if (IsInsideBounds((i - j, 0 + k)))
                        {
                            if (Maze[i - j, 0 + k] is Spawner)
                            {
                                allCoordsAreWalls = false;
                                break;
                            }
                        }
                    }
                }

                if (allCoordsAreWalls)
                {
                    coords.Add((i, 0));
                }
            }

            if (Maze[0 + 1, i] is Empty && Maze[0, i] is not Spawner)
            {
                bool allCoordsAreWalls = true;

                for (int j = 0; j < (Size - 1) / 2; j++)
                {
                    for (int k = 0; k < (Size - 1) / 2; k++)
                    {
                        if (IsInsideBounds((0 + k, i + j)))
                        {
                            if (Maze[0 + k, i + j] is Spawner)
                            {
                                allCoordsAreWalls = false;
                                break;
                            }
                        }

                        if (IsInsideBounds((0 + k, i - j)))
                        {
                            if (Maze[0 + k, i - j] is Spawner)
                            {
                                allCoordsAreWalls = false;
                                break;
                            }
                        }
                    }
                }

                if (allCoordsAreWalls)
                {
                    coords.Add((0, i));
                }
            }

            if (Maze[i, Size - 1 - 1] is Empty && Maze[i, Size - 1] is not Spawner)
            {
                bool allCoordsAreWalls = true;

                for (int j = 0; j < (Size - 1) / 2; j++)
                {
                    for (int k = 0; k < (Size - 1) / 2; k++)
                    {
                        if (IsInsideBounds((i + j, Size - 1 - k)))
                        {
                            if (Maze[i + j, Size - 1 - k] is Spawner)
                            {
                                allCoordsAreWalls = false;
                                break;
                            }
                        }

                        if (IsInsideBounds((i - j, Size - 1 - k)))
                        {
                            if (Maze[i - j, Size - 1 - k] is Spawner)
                            {
                                allCoordsAreWalls = false;
                                break;
                            }
                        }
                    }
                }

                if (allCoordsAreWalls)
                {
                    coords.Add((i, Size - 1));
                }
            }

            if (Maze[Size - 1 - 1, i] is Empty && Maze[Size - 1, i] is not Spawner)
            {
                bool allCoordsAreWalls = true;

                for (int j = 0; j < (Size - 1) / 2; j++)
                {
                    for (int k = 0; k < (Size - 1) / 2; k++)
                    {
                        if (IsInsideBounds((Size - 1 - k, i + j)))
                        {
                            if (Maze[Size - 1 - k, i + j] is Spawner)
                            {
                                allCoordsAreWalls = false;
                                break;
                            }
                        }

                        if (IsInsideBounds((Size - 1 - k, i - j)))
                        {
                            if (Maze[Size - 1 - k, i - j] is Spawner)
                            {
                                allCoordsAreWalls = false;
                                break;
                            }
                        }
                    }
                }

                if (allCoordsAreWalls)
                {
                    coords.Add((Size - 1, i));
                }
            }
        }

        int index = _random.Next(coords.Count);
        ExitCoord = (coords[index].x, coords[index].y);
        Maze[ExitCoord.x, ExitCoord.y] = new Exit(ExitCoord.x, ExitCoord.y);
    }

    void GenerateSpawner()
    {
        List<(int x, int y)> coords = new();
        for (int i = 0; i < Size; i++)
        {
            if (Maze[i, 1] is Empty)
            {
                coords.Add((i, 0));
            }

            if (Maze[0 + 1, i] is Empty)
            {
                coords.Add((0, i));
            }

            if (Maze[i, Size - 1 - 1] is Empty)
            {
                coords.Add((i, Size - 1));
            }

            if (Maze[Size - 1 - 1, i] is Empty)
            {
                coords.Add((Size - 1, i));
            }
        }

        int index = _random.Next(coords.Count);
        SpawnerCoord = (coords[index].x, coords[index].y);
        Maze[SpawnerCoord.x, SpawnerCoord.y] = new Spawner(SpawnerCoord.x, SpawnerCoord.y);
    }

    void GenerateSpikesTrap(int num)
    {
        List<(int x, int y)> posibleCoords = new();

        for (int i = 1; i < Size - 1; i++)
        {
            for (int j = 1; j < Size - 1; j++)
            {
                if (Maze[i, j] is Empty and not Spikes and not Spawner and not Exit)
                {
                    posibleCoords.Add((i, j));
                }
            }
        }

        for (int i = 0; i < num + 1; i++)
        {
            int index = _random.Next(posibleCoords.Count);
            Maze[posibleCoords[index].x, posibleCoords[index].y] = new Spikes(posibleCoords[index].x, posibleCoords[index].y, true);
            SpikesTrapsCoords.Add((posibleCoords[index].x, posibleCoords[index].y));
        }
    }
}