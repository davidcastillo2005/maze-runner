using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts.Logic
{
    public class MapGenerator
    {
        public static Tile[,] GenerateMap(int width, int height)
        {
            Tile[,] map = new Tile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y] = new Empty(x, y);
                }
            }
            CreateBoundaries(map);
            return map;
        }

        public static void CreateBoundaries(Tile[,] map)
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
    }
}
