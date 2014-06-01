using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Plotter.GUI.Shapes
{
    public class ShapeFactory
    {
        private Dictionary<String, Shape > _shapes = new Dictionary<String , Shape >(); 
        public void RegisterShape(String name, Shape shape)
        {
            _shapes.Add(name, shape);
        }

        public Shape Create(string name)
        {
            Shape shape = _shapes[name];
            return shape.CreateShape();
        }

        public string[] GetKeys()
        {
            return _shapes.Keys.ToArray<String>();
        }
    }
}
