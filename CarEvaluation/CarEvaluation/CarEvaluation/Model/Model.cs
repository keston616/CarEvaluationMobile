using System;
using System.Collections.Generic;
using System.Text;

namespace CarEvaluation
{

    public class Model
    {
        public int idModel { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        //public List<Car> Cars { get; set; } = new List<Car>();
    }
}
