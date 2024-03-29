﻿using System.Text.RegularExpressions;
using Tungsten_Interpreter.Utilities.Parser.Methods;
using Tungsten_Interpreter.Utilities.Variables;

namespace Tungsten_Interpreter.Utilities.Parser.UserMethods
{
    public class VariableDelete : IMethod, IUsing, ILexer
    {
        public string Name { get; set; } = "DELETE";
        public string Path { get; set; } = "Variables";
        public Regex RegexCode { get; set; } = new Regex(@"^delete$|WSdelete");

        // Removes Variables from Memory
        public void Execute(string[] para)
        {
            string[] args = TextMethods.CalcStringForward(String.Join(" ", para, 1, para.Length - 1), '<', '>').Replace(",", " ").Split(" ");
            args = args.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            foreach (string arg in args)
            {
                VariableSetup.RemoveEntry(arg);
            }
        }
    }
}