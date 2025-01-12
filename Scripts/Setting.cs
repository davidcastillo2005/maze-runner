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
        public int Size { get; set; }
        /// <summary>
        /// Instance of the Map Generator class.
        /// </summary>
        public MazeGenerator MazeGenerator { get; private set; }

        public Setting(int levelDifficulty, int seed, bool isRandomSeed)
        {
            //Size is supported by the Level Difficulty.
            Size = 4 * levelDifficulty + 1;
            //Create a new instance of the Map Generator class.
            MazeGenerator = new(Size, seed, isRandomSeed);
            //Get map generated from Map Generator.
        }
    }
}
