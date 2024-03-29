﻿using System.Collections;
using System.Text.RegularExpressions;
using Tungsten_Interpreter.Utilities.Parser.Methods;

namespace Tungsten_Interpreter.Utilities.Variables
{
    public class VariableSetup
    {
        // Misc Variables
        public static Hashtable globalVar = new Hashtable(); // Memory
        public static List<string> usingMethods = new List<string>() { "ACTIVATE" }; // List of Using Methods

        public static List<string[]> lines = new List<string[]>(); // Lines of Code

        // Function Variables
        public static IDictionary<string, FunctionParam> functionParameters = new Dictionary<string, FunctionParam>();
        public static IDictionary<string, FunctionBody> functionBody = new Dictionary<string, FunctionBody>();

        // While Variables
        public static IDictionary<int, int> whileStartPosition = new Dictionary<int, int>();
        public static IDictionary<int, int> whileEndPosition = new Dictionary<int, int>();

        // Adds a Value into Memory
        public static void AddEntry<T>(string name, T value)
        {
            if (globalVar.ContainsKey(name))
            {
                Console.WriteLine("Use 'update' to edit: " + name);
            }
            else
            {
                globalVar.Add(name, value);
            }
        }

        // Deletes a Value from Memory
        public static void RemoveEntry(string name)
        {
            if (globalVar.ContainsKey(name))
            {
                globalVar.Remove(name);
            }
            else
            {
                Console.WriteLine(name + " Doesn't Exist");
            }
        }

        // Updates a Value from Memory
        public static void UpdateEntry<T>(string name, T newValue)
        {
            if (globalVar.ContainsKey(name))
            {
                globalVar[name] = newValue;
            }
            else
            {
                Console.WriteLine(name + " Doesn't Exist");
            }
        }

        // Converts Array of Variable Names into Values
        public static string[] Convert(string[] input, int startIndex)
        {
            List<string> inputList = input.ToList();
            for (int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i].EndsWith('>'))
                {
                    if (TextMethods.CalcString(inputList[i], '<', '>') != inputList[i])
                    {
                        inputList[i] = inputList[i].Replace("<" + TextMethods.CalcString(inputList[i], '<', '>') + ">", "");
                    }
                }

                for (int j = 0; j < inputList[i].Length; j++)
                {
                    if (inputList[i][j] == '(' || inputList[i][j] == ')' || inputList[i][j] == '[' || inputList[i][j] == ']')
                    {
                        inputList[i] = inputList[i].Remove(j, 1);
                    }
                }
            }

            for(int i = startIndex; i < input.Length; i++)
            {
                if (globalVar.ContainsKey(inputList[i]))
                {
                    try
                    {
                        // Handle String[]
                        string[] val = (string[])globalVar[inputList[i]];
                        //input[i] = Regex.Replace(input[i].Replace(inputList[i], val[System.Convert.ToInt32(TextMethods.CalcString(input[i], '<', '>'))]), @"<[0-9]>", "") ;

                        if (int.TryParse(TextMethods.CalcString(input[i], '<', '>'), out int num))
                        {
                            input[i] = Regex.Replace(input[i].Replace(inputList[i], val[/*System.Convert.ToInt32(TextMethods.CalcString(input, '<', '>'))*/num]), @"<[0-9]+>", "");
                        }
                        else
                        {
                            input[i] = Regex.Replace(input[i].Replace(inputList[i], val[System.Convert.ToInt32(globalVar[TextMethods.CalcString(input[i], '<', '>')])]), @"<[a-zA-Z]+>", "");
                        }
                    }
                    catch
                    {
                        // Handle Non-Array
                        input[i] = input[i].Replace(inputList[i], globalVar[inputList[i]].ToString());
                    }
                }
            }

            return input;
        }

        // Converts a Variable Name into Value
        public static string Convert(string input)
        {
            string comparator = input;
            if(TextMethods.CalcString(comparator, '<', '>') !=  comparator)
            {
                comparator = comparator.Replace("<" + TextMethods.CalcString(comparator, '<', '>') + ">", "");
            }

            for(int i = 0; i < comparator.Length; i++)
            {
                if (comparator[i] == '(' || comparator[i] == ')' || comparator[i] == '[' || comparator[i] == ']')
                {
                    comparator = comparator.Remove(i, 1);
                }
            }

            try
            {
                // Handle Matrix[]
                string[] val = (string[])globalVar[comparator];
                if (int.TryParse(TextMethods.CalcString(input, '<', '>'), out int num)) {
                    input = Regex.Replace(input.Replace(comparator, val[/*System.Convert.ToInt32(TextMethods.CalcString(input, '<', '>'))*/num]), @"<[0-9]+>", "");
                }
                else
                {
                    input = Regex.Replace(input.Replace(comparator, val[System.Convert.ToInt32(globalVar[TextMethods.CalcString(input, '<', '>')])]), @"<[a-zA-Z]+>", "");
                }
            }
            catch (Exception e)
            {
                // Handle Non-Array
                input = input.Replace(comparator, globalVar[comparator].ToString());
            }


            //if (globalVar.ContainsKey(input))
            //{
            //   return globalVar[input].ToString();
            //}

            return input;
        }

        // Formats Input To Be Readable
        public static string[] Format(string[] input, int startIndex)
        {
            string[] removeable = {
                ">",
                "<",
                ",",
                "(",
                ")"
            };

            for (int i = startIndex; i < input.Length; i++)
            {
                for (int j = 0; j < removeable.Length; j++)
                {
                    input[i] = input[i].Replace(removeable[j], "");
                }
            }

            return input;
        } 
        
        // Cleans Memory
        public static void Clean()
        {
            globalVar = new Hashtable();
            usingMethods = new List<string>() { "ACTIVATE" };
            lines = new List<string[]>();

            functionParameters = new Dictionary<string, FunctionParam>();
            functionBody = new Dictionary<string, FunctionBody>();

            whileStartPosition = new Dictionary<int, int>();
            whileEndPosition = new Dictionary<int, int>();
        }
    }
}