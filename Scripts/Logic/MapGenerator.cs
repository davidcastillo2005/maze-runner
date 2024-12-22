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
            return map;
        }
    }
}
