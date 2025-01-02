//TODO: Error con el generador de laberintos (prueba con semilla: 757191438, -379204873, -1124538791).

using System;
using System.Collections.Generic;
using MazeRunner.Scripts.Data;
namespace MazeRunner.Scripts.Logic
{
    public class MazeGenerator
    {
        public (int x, int y)[] Directions { get => _directions; set => _directions = value; }
        private (int x, int y)[] _directions = new (int x, int y)[] { (2, 0), (-2, 0), (0, 2), (0, -2) };

        public Tile[,] Maze { get => _maze; set => _maze = value; }
        private Tile[,] _maze;

        public int Size => _size;
        private int _size;

        public int Seed => _seed;
        private int _seed;

        public (int x, int y) SpawnerCoord => _spawnerCoord;
        private (int x, int y) _spawnerCoord;

        public (int x, int y) ExitCoord => _exitCoord;
        private (int x, int y) _exitCoord;

        private readonly Random _random;

        public MazeGenerator(int size, int seed, bool isRandomSeed)
        {
            _size = size % 2 == 0 ? size + 1 : size;
            if (isRandomSeed)
            {
                _seed = (int)DateTime.Now.Ticks;
            }
            else
            {
                _seed = seed;
            }
            _random = new(_seed);
        }

        public bool IsInsideBounds((int x, int y) coord) => coord.x >= 0 && coord.y >= 0 && coord.x < _maze.GetLength(0) && coord.y < _maze.GetLength(1);

        void GenerateMazeRandomizedDFS((int x, int y) currentCoord, bool[,] maskVisitedCoords)
        {
            maskVisitedCoords[currentCoord.x, currentCoord.y] = true;
            _maze[currentCoord.x, currentCoord.y] = new Empty(currentCoord.x, currentCoord.y);
            Shuffle(_directions);
            foreach ((int x, int y) in _directions)
            {
                (int x, int y) neighbourCoord = (x + currentCoord.x, y + currentCoord.y);
                if (IsInsideBounds(neighbourCoord) && !maskVisitedCoords[neighbourCoord.x, neighbourCoord.y])
                {
                    _maze[neighbourCoord.x, neighbourCoord.y] = new Empty(neighbourCoord.x, neighbourCoord.y);

                    (int x, int y) inBetweenCoord = ((neighbourCoord.x + currentCoord.x) / 2, (neighbourCoord.y + currentCoord.y) / 2);
                    _maze[inBetweenCoord.x, inBetweenCoord.y] = new Empty(inBetweenCoord.x, inBetweenCoord.y);

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
            InitializeMaze();
            bool[,] maskVisitedCoords = InitializeVisitedCoords();
            GenerateMazeRandomizedDFS(GetInitialCoord(), maskVisitedCoords);
            CreateSpawner();
            CreateExit();
        }

        private void InitializeMaze()
        {
            _maze = new Tile[_size, _size];
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    _maze[x, y] = new Wall(x, y);
                }
            }
        }

        private bool[,] InitializeVisitedCoords()
        {
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    (new bool[_size, _size])[x, y] = false;
                }
            }
            return new bool[_size, _size];
        }

        (int x, int y) GetInitialCoord()
        {
            int x; int y;
            do
            {
                x = _random.Next(_size);
                y = _random.Next(_size);
            } while (x % 2 == 0 || y % 2 == 0);
            return (x, y);
        }

        void CreateExit()
        {
            List<(int x, int y)> coords = new();
            for (int i = 0; i < _size; i++)
            {
                if (_maze[i, 0 + 1] is Empty && _maze[i, 0] is not Spawner)
                {
                    coords.Add((i, 0));
                }
                if (_maze[0 + 1, i] is Empty && _maze[0, i] is not Spawner)
                {
                    coords.Add((0, i));
                }
                if (_maze[i, _size - 1 - 1] is Empty && _maze[i, _size - 1] is not Spawner)
                {
                    coords.Add((i, _size - 1));
                }
                if (_maze[_size - 1 - 1, i] is Empty && _maze[_size - 1, i] is not Spawner)
                {
                    coords.Add((i, _size - 1));
                }
            }

            int index = _random.Next(coords.Count);
            _exitCoord = (coords[index].x, coords[index].y);
            CreateExit(ExitCoord.x, ExitCoord.y);
        }

        void CreateSpawner()
        {
            List<(int x, int y)> coords = new();
            for (int i = 0; i < _size; i++)
            {
                if (_maze[i, 0 + 1] is Empty)
                {
                    coords.Add((i, 0));
                }
                if (_maze[0 + 1, i] is Empty)
                {
                    coords.Add((0, i));
                }
                if (_maze[i, _size - 1 - 1] is Empty)
                {
                    coords.Add((i, _size - 1));
                }
                if (_maze[_size - 1 - 1, i] is Empty)
                {
                    coords.Add((i, _size - 1));
                }
            }

            int index = _random.Next(coords.Count);
            _spawnerCoord = (coords[index].x, coords[index].y);
            CreateSpawner(coords[index].x, coords[index].y);
        }

        void CreateSpawner(int x, int y)
        {
            _maze[x, y] = new Spawner(x, y);
        }

        void CreateExit(int x, int y)
        {
            _maze[x, y] = new Exit(x, y);
        }
    }
}
