using System.Collections.Generic;
using Navigation;

namespace Gameplay
{
    public class PlayerCommand
    {
        public List<int> UnitIds { get; set; }

        public CommandType Type { get; set; }

        public DeterministicVector3 Target { get; set; }
    }
}