//TODO: Error con el generador de laberintos (prueba con semilla: 757191438, -379204873, -1124538791, -2130386984, 20293741, 332626077).
using System;
using System.Collections.Generic;
using Godot;
using MazeRunner.Scripts.Data;
namespace MazeRunner.Scripts.Logic
{
    public class MazeGenerator
    {
        public (int x, int y)[] Directions { get; } = { (2, 0), (-2, 0), (0, 2), (0, -2) };
        public Tile[,] Maze { get; private set; }
        public int Size { get; set; }
        public int Seed { get; set; }
        public (int x, int y) SpawnerCoord { get; set; }
        public (int x, int y) ExitCoord { get; set; }
        private readonly Random _random;
        public List<Portal> portalTiles = new();

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
            _random = new(Seed);
        }

        public bool IsInsideBounds((int x, int y) coord) => coord.x >= 0 && coord.y >= 0 && coord.x < Maze.GetLength(0) && coord.y < Maze.GetLength(1);

        void GenerateMazeRandomizedDFS((int x, int y) currentCoord, bool[,] maskVisitedCoords)
        {
            maskVisitedCoords[currentCoord.x, currentCoord.y] = true;
            Maze[currentCoord.x, currentCoord.y] = new Empty(currentCoord.x, currentCoord.y);
            Shuffle(Directions);
            foreach ((int x, int y) in Directions)
            {
                (int x, int y) neighbourCoord = (x + currentCoord.x, y + currentCoord.y);
                if (IsInsideBounds(neighbourCoord) && !maskVisitedCoords[neighbourCoord.x, neighbourCoord.y])
                {
                    Maze[neighbourCoord.x, neighbourCoord.y] = new Empty(neighbourCoord.x, neighbourCoord.y);

                    (int x, int y) inBetweenCoord = ((neighbourCoord.x + currentCoord.x) / 2, (neighbourCoord.y + currentCoord.y) / 2);
                    Maze[inBetweenCoord.x, inBetweenCoord.y] = new Empty(inBetweenCoord.x, inBetweenCoord.y);

                    GenerateMazeRandomizedDFS(neighbourCoord, maskVisitedCoords);
                }
            }
        }

        void Shuffle((int x, int y)[] coordsArray)
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
            GenerateMazeRandomizedDFS(GetInitialCoord(), maskVisitedCoords);
            CreateSpawner();
            CreateExit();
            CreatePortals();
        }

        (int x, int y) GetInitialCoord()
        {
            int x; int y;
            do
            {
                x = _random.Next(Size);
                y = _random.Next(Size);
            } while (x % 2 == 0 || y % 2 == 0);
            return (x, y);
        }


        void CreateExit()
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

                    if (allCoordsAreWalls == true)
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

                    if (allCoordsAreWalls == true)
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

                    if (allCoordsAreWalls == true)
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

                    if (allCoordsAreWalls == true)
                    {
                        coords.Add((Size - 1, i));
                    }
                }
            }

            int index = _random.Next(coords.Count);
            ExitCoord = (coords[index].x, coords[index].y);
            CreateExit(ExitCoord.x, ExitCoord.y);
        }

        void CreateSpawner()
        {
            List<(int x, int y)> coords = new();
            for (int i = 0; i < Size; i++)
            {
                if (Maze[i, 0 + 1] is Empty)
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
            CreateSpawner(coords[index].x, coords[index].y);
        }

        void CreateSpawner(int x, int y)
        {
            Maze[x, y] = new Spawner(x, y);
        }

        void CreateExit(int x, int y)
        {
            Maze[x, y] = new Exit(x, y);
        }

        void CreatePortal((int x, int y) position, (int x, int y) finalPosition)
        {
            Maze[position.x, position.y] = new Portal(position.x, position.y, true, finalPosition);
        }

        void CreatePortals()
        {
            List<(int x, int y)> posibleCoords = new();

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (Maze[x, y] is Empty && Maze[x, y] is not Spawner && Maze[x, y] is not Exit)
                    {
                        posibleCoords.Add((x, y));
                    }
                }
            }

            Portal portal = new Portal(posibleCoords[1].x, posibleCoords[1].y, true, (posibleCoords[5].x, posibleCoords[5].y));
            Maze[posibleCoords[1].x, posibleCoords[1].y] = portal;
            portalTiles.Add(portal);
        }
    }
}