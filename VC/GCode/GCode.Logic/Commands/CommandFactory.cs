using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace GCode.Logic.Commands
{
    public class CommandFactory
    {
		private Dictionary<String, Type> _shapes = new Dictionary<String, Type>(); 
        
		public CommandFactory()
		{
			RegisterAll();
		}

		public void RegisterShape(String name, Type shape)
        {
            _shapes.Add(name, shape);
        }

        public Command Create(string name)
        {
			if (IsRegistered(name))
			{
				Type shape = _shapes[name];
				return (Command)Activator.CreateInstance(shape); ;
			}
			return null;
        }

        public string[] GetKeys()
        {
            return _shapes.Keys.ToArray<String>();
        }

		public bool IsRegistered(string name) { return _shapes.ContainsKey(name); } 

		private void RegisterAll()
		{
			Assembly ass = Assembly.GetExecutingAssembly();

			foreach (Type t in ass.GetTypes())
			{
				if (t.IsClass)
				{
					IsGCommandAttribute isgcode = t.GetCustomAttribute<IsGCommandAttribute>();
					if (isgcode != null)
					{
						string ascodes = isgcode.RegisterAs;
						if (string.IsNullOrEmpty(ascodes))
							ascodes = t.Name.Substring(0,3);

						foreach (string ascode in ascodes.Split(','))
						{
							RegisterShape(ascode, t);
						}
					}
				}
			}
		}
    }
}
