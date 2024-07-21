using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class AttendCountModel
    {
        public int JeddahComing { get; set; }

        public int JeddahNotComing { get; set; }

        public int RiyadhComing { get; set; }

        public int RiyadhNotComing { get; set; }

        public int DammamComing { get; set; }

        public int DammamNotComing { get; set; }

        public int TotalComing { get; set; }

        public int TotalNotComing { get; set; }
    }
}
