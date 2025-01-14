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
            Size = 6 * levelDifficulty + 1;
            MazeGenerator = new(Size, seed, isRandomSeed);
        }

        public void AddSkill(Skill skill, Token token)
        {
            if (!token.Skill.Contains(skill))
            {
                token.Skill.Add(skill);
            }
        }

        public void RemoveSkill(Skill skill, Token token)
        {
            if (token.Skill.Contains(skill))
            {
                token.Skill.Remove(skill);
            }
        }
    }
}
