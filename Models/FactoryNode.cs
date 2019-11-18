using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FactoryAPI.Models
{
    public class FactoryNode: Factory
    {
        public List<FactoryNode> children { get; set; }

    }
}
