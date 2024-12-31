using System;
using System.Collections.Generic;
using System.Data;
using Godot;
using MazeRunner.Scripts.Data;
namespace MazeRunner.Scripts.Logic
{
    public class MazeGenerator
    {
        public (int x, int y)[] Directions { get => _directions; set => _directions = value; }
        private (int x, int y)[] _directions = new (int x, int y)[] { (2, 0), (-2, 0), (0, 2), (0, -2) };

        public Tile[,] Maze { get => _maze; set => _maze = value; }
        private Tile[,] _maze;

        private int _size;
        private int _seed;
        private int _fillPercentage;
        private List<(int x, int y)> _visitedCoords = new();
        private (int x, int y) _initialCoord = (1, 1);
        private Random _random;

        public MazeGenerator(int size, int seed, bool isRandomSeed, int fillPercentage)
        {
            _size = 11;
            _seed = seed;
            _fillPercentage = fillPercentage;
            if (isRandomSeed)
            {
                _seed = DateTime.Now.Millisecond;
            }
            _random = new(_seed);
            GenerateMaze();
        }

        public bool IsInsideBounds((int x, int y) coord) => coord.x >= 0 && coord.y >= 0 && coord.x < _maze.GetLength(0) && coord.y < _maze.GetLength(1);

        void GenerateMazeRandomizedDFS((int x, int y) currentCoord)
        {
            _visitedCoords.Add(currentCoord);
            _maze[currentCoord.x, currentCoord.y] = new Empty(currentCoord.x, currentCoord.y);
            ShuffleA(_directions);
            foreach ((int x, int y) in _directions)
            {
                (int x, int y) neighbourCoord = (x + currentCoord.x, y + currentCoord.y);
                if (!_visitedCoords.Contains(neighbourCoord) && IsInsideBounds(neighbourCoord))
                {
                    _maze[neighbourCoord.x, neighbourCoord.y] = new Empty(neighbourCoord.x, neighbourCoord.y);

                    (int x, int y) inBetweenCoord = ((neighbourCoord.x + currentCoord.x) / 2, (neighbourCoord.y + currentCoord.y) / 2);
                    _maze[inBetweenCoord.x, inBetweenCoord.y] = new Empty(inBetweenCoord.x, inBetweenCoord.y);

                    GenerateMazeRandomizedDFS(neighbourCoord);
                }
            }
        }

        void ShuffleA((int x, int y)[] coordsArray)
        {
            for (int i = coordsArray.Length - 1; i > 0; i--)
            {
                int j = _random.Next(0, i + 1);
                (int x, int y) temp = coordsArray[i];
                coordsArray[i] = coordsArray[j];
                coordsArray[j] = temp;
            }
        }
        void GenerateMaze()
        {
            _maze = new Tile[_size, _size];
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    _maze[x, y] = new Wall(x, y);
                }
            }
            GenerateMazeRandomizedDFS(_initialCoord);
            CreateSpawner(0, 1);
            CreateExit(0, _size - 2);
        }

        (int x, int y) GetSpawnerCoord()
        {
            List<(int x, int y)> spawnerCoord = new();
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    if (_maze[x, y].GetType() == typeof(Spawner))
                    {
                        spawnerCoord.Add((x, y));
                    }
                }
            }
            if (spawnerCoord.Count > 0)
            {
                return spawnerCoord[0];
            }
            else
            {
                throw new Exception();
            }
        }

        void CreateBoundaries()
        {
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    if (x == 0 || y == 0 || x == _size - 1 || y == _size - 1)
                    {
                        if (_maze[x, y].GetType() != typeof(Spawner) && _maze[x, y].GetType() != typeof(Exit)) _maze[x, y] = new Wall(x, y);
                    }
                }
            }
        }

        void RandomizeTiles()
        {

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (_random.Next(0, 100) < _fillPercentage)
                    {
                        _maze[i, j] = new Wall(i, j);
                    }
                    else
                    {
                        _maze[i, j] = new Empty(i, j);
                    }
                }
            }
        }

        void CreateSpawner(int x, int y)
        {
            _maze[x, y] = new Spawner(x, y);
        }

        void CreateExit(int x, int y)
        {
            _maze[x, y] = new Exit(x, y);
        }

        void CreateSpawner()
        {
            List<(int x, int y)> listCoordsOnBounds = new();

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    if ((x == 0 || y == 0 || x == _size - 1 || y == _size - 1) && _maze[x, y].GetType() != typeof(Exit))
                    {
                        foreach (var item in _directions)
                        {
                            (int x, int y) neighbourCoord = (item.x + x, item.y + y);
                            if (_maze[neighbourCoord.x, neighbourCoord.y].GetType() == typeof(Empty))
                            {
                                listCoordsOnBounds.Add((x, y));
                                return;
                            }
                        }
                    }
                }
            }

            int listIndex = _random.Next(0, listCoordsOnBounds.Count - 1);
            (int x, int y) spawnerCoord = (listCoordsOnBounds[listIndex].x, listCoordsOnBounds[listIndex].y);
            CreateSpawner(spawnerCoord.x, spawnerCoord.y);
        }

        void CreateExit()
        {
            List<(int x, int y)> listCoordsOnBounds = new();

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    if ((x == 0 || y == 0 || x == _size - 1 || y == _size - 1) && _maze[x, y].GetType() != typeof(Spawner))
                    {
                        foreach (var item in _directions)
                        {
                            (int x, int y) neighbourCoord = (item.x + x, item.y + y);
                            if (_maze[neighbourCoord.x, neighbourCoord.y].GetType() == typeof(Empty))
                            {
                                listCoordsOnBounds.Add((x, y));
                                return;
                            }
                        }
                    }
                }
            }

            int listIndex = _random.Next(0, listCoordsOnBounds.Count - 1);
            (int x, int y) exitTile = (listCoordsOnBounds[listIndex].x, listCoordsOnBounds[listIndex].y);
            _maze[exitTile.x, exitTile.y] = new Exit(exitTile.x, exitTile.y);
        }
    }
}
