using System;
using System.Collections.Generic;
using System.Data;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts.Logic
{
    public class MazeGenerator
    {
        public (int x, int y)[] Directions { get => _directions; set => _directions = value; }
        private (int x, int y)[] _directions = new (int x, int y)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

        public Tile[,] Maze { get => _maze; set => _maze = value; }
        private Tile[,] _maze;

        private int _size;
        private int _seed;
        private bool _isRandomSeed;
        private int _fillPercentage;

        public Dictionary<(int x, int y), List<(int x, int y)>> _neighboursList;

        public MazeGenerator(int size, int seed, bool isRandomSeed, int fillPercentage)
        {
            _size = size % 2 == 0 ? size : size + 1;
            _seed = seed;
            _isRandomSeed = isRandomSeed;
            _fillPercentage = fillPercentage;
            GenerateMaze();
        }

        public bool IsInsideBounds((int x, int y) coord) => coord.x >= 0 && coord.y >= 0 && coord.x < _maze.GetLength(0) && coord.y < _maze.GetLength(1);

        void AddCoordToNeighboursList((int x, int y) coord)
        {
            if (IsInsideBounds(coord))
            {
                List<(int x, int y)> neighbourCoords = new();
                foreach (var item in _directions)
                {
                    (int x, int y) newCoord = (item.x + coord.x, item.y + coord.y);
                    if (IsInsideBounds(newCoord))
                    {
                        _neighboursList.Add(coord, neighbourCoords);
                    }
                }
            }
        }



        // void CreateMaze((int x, int y) start)
        // {
        //     Stack<(int x, int y)> stack = new();
        //     List<(int x, int y)> visitedCoords = new();
        //     stack.Push(start);
        //     visitedCoords.Add(start);

        //     Random random = new(_seed);

        //     while (stack.Count > 0)
        //     {
        //         var current = stack.Peek();
        //         var neighbours = _neighboursList.ContainsKey(current) ? _neighboursList[current] : new List<(int x, int y)>();
        //         var unvisitedNeighbours = neighbours.Where(n => !visitedCoords.Contains(n)).ToList();

        //         if (unvisitedNeighbours.Count > 0)
        //         {
        //             var next = unvisitedNeighbours[random.Next(unvisitedNeighbours.Count)];
        //             visitedCoords.Add(next);
        //             stack.Push(next);
        //         }
        //         else
        //         {
        //             stack.Pop();
        //         }
        //     }
        // }

        void GenerateMaze()
        {
            _maze = new Tile[_size, _size];
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    _maze[x, y] = new Empty(x, y);
                }
            }
            CreatePlayerSpawn();
            CreateExit();
            CreateBoundaries();
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
            if (_isRandomSeed)
            {
                _seed = DateTime.Now.Millisecond;
            }
            Random random = new(_seed);

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (random.Next(0, 100) < _fillPercentage)
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

        void CreatePlayerSpawn()
        {
            List<(int x, int y)> listTilesOnBounds = new();

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    if ((x == 0 || y == 0 || x == _size - 1 || y == _size - 1) && _maze[x, y].GetType() != typeof(Exit))
                    {
                        if (!((x == 0 && y == 0) || (x == _size - 1 && y == _size - 1) || (x == 0 && y == _size - 1) || (x == _size - 1 && y == 0))) listTilesOnBounds.Add((x, y));
                    }
                }
            }

            int listIndex = new Random().Next(0, listTilesOnBounds.Count - 1);
            (int x, int y) spawnerTile = (listTilesOnBounds[listIndex].x, listTilesOnBounds[listIndex].y);
            _maze[spawnerTile.x, spawnerTile.y] = new Spawner(spawnerTile.x, spawnerTile.y);
        }

        void CreateExit()
        {
            List<(int x, int y)> listTilesOnBounds = new();

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    if ((x == 0 || y == 0 || x == _size - 1 || y == _size - 1) && _maze[x, y].GetType() != typeof(Spawner))
                    {
                        if (!((x == 0 && y == 0) || (x == _size - 1 && y == _size - 1) || (x == 0 && y == _size - 1) || (x == _size - 1 && y == 0))) listTilesOnBounds.Add((x, y));
                    }
                }
            }

            int listIndex = new Random().Next(0, listTilesOnBounds.Count - 1);
            (int x, int y) exitTile = (listTilesOnBounds[listIndex].x, listTilesOnBounds[listIndex].y);
            _maze[exitTile.x, exitTile.y] = new Exit(exitTile.x, exitTile.y);
        }
    }
}
