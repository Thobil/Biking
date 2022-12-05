using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biking
{
    internal class TrajectoryInfos
    {
        public float distance { get; set; }
        public float duration { get; set; }
        public List<string> instructions { get; set; }
        public TrajectoryInfos(float duration, float distance, List<string> instructions)
        {
            this.duration = duration;
            this.distance = distance;
            this.instructions = instructions;
        }
    }
}
