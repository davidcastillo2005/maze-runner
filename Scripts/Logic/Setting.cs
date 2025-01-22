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
        public bool[] SkillBools { get; private set; } = { false, false, false };

        public Setting(int levelDifficulty, int seed, bool isRandomSeed)
        {
            Size = 6 * levelDifficulty + 1;
            MazeGenerator = new(Size, seed, isRandomSeed);
        }

        public void CheckSkill(int index)
        {
            for (int i = 0; i < SkillBools.Length; i++)
            {
                if (i == index)
                {
                    SkillBools[i] = true;
                }
                else
                {
                    SkillBools[i] = false;
                }
            }
        }
    }
}