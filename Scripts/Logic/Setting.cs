using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts.Logic
{
    /// <summary>
    /// Game setup.
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// Size of the map to be generated.
        /// </summary>
        public int Size { get => _size; set => _size = value; }
        private int _size;
        /// <summary>
        /// Seed for random generation.
        /// </summary>
        public int Seed { get => _seed; set => _seed = value; }
        private int _seed;
        /// <summary>
        /// Map generated.
        /// </summary>
        public Tile[,] Map { get => _map; set => _map = value; }
        private Tile[,] _map;
        /// <summary>
        /// Fill percentage of the map.
        /// </summary>
        private int _fillPercentage = 20;
        /// <summary>
        /// Instance of the Map Generator class.
        /// </summary>
        private MazeGenerator _mazeGenerator;

        /// <summary>
        /// Setting constructor.
        /// </summary>
        /// <param name="levelDifficulty"></param>
        /// <param name="seed"></param>
        /// <param name="isRandomSeed"></param>
        public Setting(int levelDifficulty, int seed, bool isRandomSeed)
        {
            //Size is supported by the Level Difficulty.
            _size = 2 * levelDifficulty + 1;
            //Create a new instance of the Map Generator class.
            _mazeGenerator = new(_size, seed, isRandomSeed, _fillPercentage);
            //Get map generated from Map Generator.
            _map = _mazeGenerator.Maze;
        }
    }
}
