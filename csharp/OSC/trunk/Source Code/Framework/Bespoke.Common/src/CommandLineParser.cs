using System;
using System.Text;
using System.Collections.Generic;

namespace Bespoke.Common
{
	/// <summary>
	/// 
	/// </summary>
	public class CommandLineParser
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string this[string key]
		{
			get
			{
				return mArguments[key.ToUpper()];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int Count
		{
			get
			{
				return mArguments.Count;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string[] Keys
		{
			get
			{
				string[] keys = new string[mArguments.Keys.Count];
				mArguments.Keys.CopyTo(keys, 0);
				return keys;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string[] Values
		{
			get
			{
				string[] values = new string[mArguments.Values.Count];
				mArguments.Values.CopyTo(values, 0);
				return values;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args"></param>
		public CommandLineParser(string[] args)
		{
            mArguments = new SortedDictionary<string, string>();

            string collapsedArguments = CommandLineParser.CollapseArguments(args);
            if (collapsedArguments.IndexOf(SpecialValueSeparator) != -1)
            {
                ParseSpecialCommandLine(collapsedArguments);
            }
            else
            {
                ParseWindowsCommandLine(args);
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string CollapseArguments(string[] args)
        {
            StringBuilder collapseArguments = new StringBuilder();

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    collapseArguments.Append(arg);
                    collapseArguments.Append(SpaceCharacter);
                }

                // Trim last space
                collapseArguments.Remove(collapseArguments.Length - 1, 1);
            }

            return collapseArguments.ToString();
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool ContainsKey(string key)
		{
			return mArguments.ContainsKey(key.ToUpper());
        }

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void ParseWindowsCommandLine(string[] args)
        {
            string name = String.Empty;
            string value = String.Empty;

            int delimiterIndex = 0;
            foreach (string arg in args)
            {
                // Find a value delimiter in the current name=value pair
                delimiterIndex = arg.IndexOf(ValueDelimiter);
                if (delimiterIndex == -1)
                {
                    name = arg;
                    value = String.Empty;
                }
                else
                {
                    // Parse out the name and value from the pair
                    name = arg.Substring(0, delimiterIndex).Trim().ToUpper();
                    value = arg.Substring(delimiterIndex + 1).Trim().ToUpper();
                }

                mArguments.Add(name, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collapsedArgs"></param>
        private void ParseSpecialCommandLine(string collapsedArgs)
        {
            // Reconstruct a Windows-style command-line
            List<string> args = new List<string>();
            
            string arg = string.Empty;
            bool inQuotedArgument = false;
            foreach (char c in collapsedArgs)
            {
                if (c == SpecialValueSeparator)
                {
                    if (inQuotedArgument)
                    {
                        // End quoted argument
                        if (arg != String.Empty)
                        {
                            args.Add(arg);
                            arg = String.Empty;
                        }
                        inQuotedArgument = false;
                    }
                    else
                    {
                        // Begin quoted argument
                        inQuotedArgument = true;
                    }
                }
                else if ((c == SpaceCharacter) && (inQuotedArgument == false))
                {
                    if (arg != String.Empty)
                    {
                        args.Add(arg);
                        arg = String.Empty;
                    }
                }
                else
                {
                    arg += c;
                }
            }

            if (arg != String.Empty)
            {
                args.Add(arg);
            }

            ParseWindowsCommandLine(args.ToArray());
        }

        #endregion

        private static readonly char SpaceCharacter = ' ';
        private static readonly char SpecialValueSeparator = '`';
		private static readonly string ValueDelimiter = "=";

		private static SortedDictionary<string, string> mArguments;
	}
}
