using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorIron.Shared
{
    public class ServoInfo
    {
        public int Angle1 { get; set; }
        public int Angle2 { get; set; }
        public int MinPulse { get; set; } = 500;
        public int MaxPulse { get; set; } = 2500;
    }
}
