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
        /// Instance of the Map Generator class.
        /// </summary>
        public MazeGenerator MazeGenerator { get; private set; }

        /// <summary>
        /// Setting constructor.
        /// </summary>
        /// <param name="levelDifficulty"></param>
        /// <param name="seed"></param>
        /// <param name="isRandomSeed"></param>
        public Setting(int levelDifficulty, int seed, bool isRandomSeed)
        {
            //Size is supported by the Level Difficulty.
            _size = 4 * levelDifficulty + 1;
            //Create a new instance of the Map Generator class.
            MazeGenerator = new(_size, seed, isRandomSeed);
            //Get map generated from Map Generator.
            // _map = _mazeGenerator.Maze;
        }
    }
}
