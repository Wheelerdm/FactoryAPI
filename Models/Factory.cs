using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FactoryAPI
{
    public class Factory
    {
        public int ID { get; set; }
        public string name { get; set; }
        public int parentID { get; set; }
        public int lowerBound { get; set; }
        public int upperBound { get; set; }
    }
}
