using System;
using System.Collections.Generic;
using System.ComponentModel;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts.Logic
{
    public class MapGenerator
    {
        public static Tile[,] GenerateMap(int width, int height, int seed, bool isRandomSeed, int fillPercentage)
        {
            Tile[,] map = new Tile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y] = new Empty(x, y);
                }
            }
            RandomizeTiles(map, seed, isRandomSeed, fillPercentage);
            CreateBoundaries(map);
            CreatePlayerSpawn(map);
            return map;
        }

        static void CreateBoundaries(Tile[,] map)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (x == 0 || y == 0 || x == map.GetLength(0) - 1 || y == map.GetLength(1) - 1)
                    {
                        map[x, y] = new Wall(x, y);
                    }
                }
            }
        }

        static void RandomizeTiles(Tile[,] map, int seed, bool isRandomSeed, int fillPercentage)
        {
            if (isRandomSeed)
            {
                seed = DateTime.Now.Millisecond;
            }
            Random random = new(seed);

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (random.Next(0, 100) < fillPercentage)
                    {
                        map[i, j] = new Wall(i, j);
                    }
                    else
                    {
                        map[i, j] = new Empty(i, j);
                    }
                }
            }
        }

        static void CreatePlayerSpawn(Tile[,] map)
        {
            List<(int x, int y)> listNotEmptyTiles = new();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j].GetType() != typeof(Empty))
                    {
                        listNotEmptyTiles.Add((i, j));
                    }
                }
            }
            if (listNotEmptyTiles.Count == map.Length)
            {
                map[1, 1] = new Empty(1, 1);
            }
        }
    }
}
