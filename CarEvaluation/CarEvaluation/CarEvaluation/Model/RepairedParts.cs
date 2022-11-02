using System;
using System.Collections.Generic;
using System.Text;

namespace CarEvaluation
{
    public class RepairedParts
    {
        public int idRepairedParts { get; set; }
        public string Description { get; set; }
        public int DetailId { get; set; }
        public Detail Detail { get; set; }
        public int CarId { get; set; }
        //public Car Car { get; set; }
    }
}
