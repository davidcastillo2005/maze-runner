namespace MazeRunner.Scripts.Logic
{
    public class Setting
    {
        public int Size { get; set; }
        public MazeGenerator MazeGenerator { get; private set; }

        public bool[] PlayerOneSkillBools { get; private set; } = { false, false, false, false, false, false };
        public bool[] PlayerTwoSkillBools { get; private set; } = { false, false, false, false, false, false };

        public Setting(int levelDifficulty, int seed, bool isRandomSeed)
        {
            Size = 6 * levelDifficulty + 1;
            MazeGenerator = new(Size, seed, isRandomSeed);
        }

        public void CheckSkillPlayerOne(int index)
        {
            for (int i = 0; i < PlayerOneSkillBools.Length; i++)
            {
                if (i == index)
                {
                    PlayerOneSkillBools[i] = true;
                }
                else
                {
                    PlayerOneSkillBools[i] = false;
                }
            }
        }

        public void CheckSkillPlayerTwo(int index)
        {
            for (int i = 0; i < PlayerTwoSkillBools.Length; i++)
            {
                if (i == index)
                {
                    PlayerTwoSkillBools[i] = true;
                }
                else
                {
                    PlayerTwoSkillBools[i] = false;
                }
            }
        }
    }
}