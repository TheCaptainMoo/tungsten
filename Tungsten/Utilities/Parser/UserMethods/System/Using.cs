﻿using System.Reflection;
using System.Text.RegularExpressions;
using Tungsten_Interpreter.Utilities.Parser.Methods;
using Tungsten_Interpreter.Utilities.Variables;

namespace Tungsten_Interpreter.Utilities.Parser.UserMethods.System
{
    internal class Using : IMethod, ILexer
    {
        public string Name { get; set; } = "ACTIVATE";
        public Regex RegexCode { get; set; } = new Regex(@"activate|\$");

        // Gets All Methods With a Path
        public static IEnumerable<IUsing> usings = from t in Assembly.GetExecutingAssembly().GetTypes()
                     where t.GetInterfaces().Contains(typeof(IUsing))
                              && t.GetConstructor(Type.EmptyTypes) != null
                     select Activator.CreateInstance(t) as IUsing;

        // Saves the Paths to Memory
        public void Execute(string[] para)
        {
            string path = TextMethods.CalcString(String.Join(" ", para, 1, para.Length - 1), '<', '>');

            foreach(var u in usings)
            {
                if (u.Path == path)
                {
                    VariableSetup.usingMethods.Add(u.Name);
                }
            }
        }
    }
}
