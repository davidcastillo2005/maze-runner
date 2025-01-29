namespace MazeRunner.Scripts.Logic;

public class Setting
{
    public int Size { get; set; }
    public MazeGenerator MazeGenerator { get; private set; }

    public Setting(int levelDifficulty, int seed, bool isRandomSeed)
    {
        Size = 6 * levelDifficulty + 1;
        MazeGenerator = new(Size, seed, isRandomSeed);
    }
}