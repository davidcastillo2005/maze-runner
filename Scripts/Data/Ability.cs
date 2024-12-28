namespace MazeRunner.Scripts.Data
{
    /// <summary>
    /// Skill.
    /// </summary>
    public class Skill
    {
        /// <summary>
        /// Skill name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        private string _name;
        /// <summary>
        /// Skill description.
        /// </summary>
        public string Description { get => _description; set => _description = value; }
        private string _description;

        /// <summary>
        /// Skill constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public Skill(string name, string description)
        {
            //Assign values.
            _name = name;
            _description = description;
        }
    }
}