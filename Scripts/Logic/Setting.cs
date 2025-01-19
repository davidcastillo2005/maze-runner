using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
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
        public bool[] SkillBools { get; private set; } = { false };

        public Setting(int levelDifficulty, int seed, bool isRandomSeed)
        {
            Size = 6 * levelDifficulty + 1;
            MazeGenerator = new(Size, seed, isRandomSeed);
        }

        public void CheckSkill(int index)
        {
            if (!SkillBools[index])
            {
                SkillBools[index] = true;
            }
            else
            {
                SkillBools[index] = false;
            }
        }
    }
}