using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts.Logic
{
    public class MapGenerator
    {

        private int _size;
        private int _seed;
        private bool _isRandomSeed;
        private int _fillPercentage;

        public Tile[,] Map { get => _map; set => _map = value; }
        private Tile[,] _map;

        public MapGenerator(int size, int seed, bool isRandomSeed, int fillPercentage)
        {
            _size = size;
            _seed = seed;
            _isRandomSeed = isRandomSeed;
            _fillPercentage = fillPercentage;
            GenerateMap();
        }

        void GenerateMap()
        {
            _map = new Tile[_size, _size];
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    _map[x, y] = new Empty(x, y);
                }
            }
            CreatePlayerSpawn();
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
                        if (!(_map[x, y].GetType() == typeof(Spawner))) _map[x, y] = new Wall(x, y);
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
                        _map[i, j] = new Wall(i, j);
                    }
                    else
                    {
                        _map[i, j] = new Empty(i, j);
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
                    if (x == 0 || y == 0 || x == _size - 1 || y == _size - 1)
                    {
                        if (!((x == 0 && y == 0) || (x == _size - 1 && y == _size - 1) || (x == 0 && y == _size - 1) || (x == _size - 1 && y == 0))) listTilesOnBounds.Add((x, y));
                    }
                }
            }

            int listIndex = new Random().Next(0, listTilesOnBounds.Count - 1);
            (int x, int y) spawnerTile = (listTilesOnBounds[listIndex].x, listTilesOnBounds[listIndex].y);
            _map[spawnerTile.x, spawnerTile.y] = new Spawner(spawnerTile.x, spawnerTile.y);
        }
    }
}
