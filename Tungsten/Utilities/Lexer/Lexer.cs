﻿using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Tungsten_Interpreter;
using Tungsten_Interpreter.Utilities.AST;
using Tungsten_Interpreter.Utilities.Parser;
using Tungsten_Interpreter.Utilities.Variables;

namespace Lexer
{
    public class TungstenLexer
    {
        public static readonly string[] splitChars =
        {
            " ",
            //"\n",
            "\r",
            "\t",
            //";"
        };

        public static readonly string[] lineChars =
        {
            "WS",
            "\0",
            "NL"
        };

        public sealed class TokenAssign
        {
            public TokenAssign(string token, Regex regex)
            {
                Token = token;
                this.regex = regex;
            }

            public string Token { get; set; }
            public Regex regex { get; set; }
        }

        static List<TokenAssign> LexerInit()
        {
            // Handle Syntax
            List<TokenAssign> ta = new List<TokenAssign>()
            {
                new TokenAssign("WS", new Regex(@"\s+|\t")),
                new TokenAssign("NL", new Regex(@";|\n+|\r+|[\r\n]+|\*\/")),
                new TokenAssign("SB", new Regex(@"{|WS{")),
                new TokenAssign("EB", new Regex(@"}|WS}")),
                new TokenAssign("WSASSIGN", new Regex(@":|WS:"))
            };

            // Handle Keywords
            var tokens = from t in Assembly.GetExecutingAssembly().GetTypes()
                         where t.GetInterfaces().Contains(typeof(ILexer))
                                  && t.GetConstructor(Type.EmptyTypes) != null
                         select Activator.CreateInstance(t) as ILexer;

            foreach (var t in tokens)
            {
                ta.Add(new TokenAssign(t.Name, t.RegexCode));
            }

            return ta;
        }

        public static string Lexer(string[] args)
        {
            List<TokenAssign> tokens = LexerInit();
            StringBuilder str = new StringBuilder();

            int bracketNum = 0;
            bool insideString = false;

            for (int i = 0; i < args.Length; i++)
            {
                string temp = args[i];
                for (int j = 0; j < temp.Length; j++)
                {
                    if (temp[j] == '[')
                    {
                        insideString = true;
                    }
                    else if (temp[j] == ']')
                    {
                        insideString = false;
                    }
                    else if (temp[j] == '{')
                    {
                        temp = temp.Insert(j, "NL");
                        temp += "WS" + bracketNum + "NL";
                        bracketNum++;
                        break;
                    }
                    else if (temp[j] == '}')
                    {
                        temp = temp.Insert(j, "NL");
                        bracketNum--;
                        temp += "WS" + bracketNum + "NL";
                        break;
                    }
                }

                for (int j = 0; j < tokens.Count; j++)
                {
                    if (insideString == false)
                    {
                        temp = Regex.Replace(temp, tokens[j].regex.ToString(), tokens[j].Token);
                    }
                }

                str.Append(temp + "WS");
            }

            return str.ToString();
        }

        public static List<string[]> ConstructLines(string args)
        {
            Span<string> strings = args.Split("NL", StringSplitOptions.RemoveEmptyEntries);
            List<string[]> lines = new List<string[]>();

            for (int i = 0; i < strings.Length; i++)
            {
                //VariableSetup.lines.Add(strings[i].Split("WS", StringSplitOptions.RemoveEmptyEntries));
                lines.Add(strings[i].Split("WS", StringSplitOptions.RemoveEmptyEntries));
            }

            return lines;
        }

        public static void CreateNodes(List<string[]> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                // Cleans Lexer Input
                #region Cleaning & Init
                string[] words = lines[i];

                words = words.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                if (words.Length == 0 || words[0].StartsWith("/*"))
                {
                    if (i >= lines.Count || i == lines.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        i++;
                    }
                    continue;
                }

                #endregion

                #region AstGeneration

                if (Program.methods.ContainsKey(words[0]))
                {
                    Program.methods[words[0]].AstConstructor(words);
                }

                #endregion
            }

        }
    }
}