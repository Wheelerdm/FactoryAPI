using FactoryAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FactoryAPI
{
    public class FactoryNodeBuilder
    {

        private DB1Context _context;
        public FactoryNodeBuilder(DB1Context context)
        {
            _context = context;
        }

        public Factory CreateFactory(FactoryNode factoryNode)
        {
            Factory factory = new Factory();
            factory.lowerBound = factoryNode.lowerBound;
            factory.upperBound = factoryNode.upperBound;
            factory.name = factoryNode.name;
            factory.parentID = factoryNode.parentID;

            Factory parent = _context.Factory.Find(factoryNode.parentID);
            if (parent is null)
            {
                throw new Exception("Invalid parentID.  Can't have a child w/o a parent.");
            }

            try
            {
                _context.Factory.Add(factory);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //Log error
                Console.WriteLine(factory.name + " Failed to create new factory. " + ex.ToString());
                throw;
            }

            foreach (FactoryNode child in factoryNode.children)
            {
                child.parentID = factory.ID;
                CreateFactory(child);
            }

            return factory;


        }
        public FactoryNode CreateFactoryNode(Factory factory)
        {
            FactoryNode fn = new FactoryNode();
            fn.ID = factory.ID;
            fn.lowerBound = factory.lowerBound;
            fn.upperBound = factory.upperBound;
            fn.name = factory.name;
            fn.parentID = factory.parentID;
            fn.children = new List<FactoryNode>();

            if (factory == null)
            {
                return fn;
            }

            var children = _context.Factory
                     .Where(f => f.parentID == factory.ID).OrderBy(o => o.ID);

            //Check children's children
            if (children.Count() == 0)
            {
                return fn;
            }

            foreach (Factory f in children.ToList())
            {
                fn.children.Add(CreateFactoryNode(f));
            }

            return fn;

        }
    }
}
