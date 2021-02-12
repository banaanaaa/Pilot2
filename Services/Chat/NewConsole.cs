using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Pilot2.Services.Chat
{
	class NewConsole : IWriter
	{
		public enum Formating { Number, Line };

		private readonly Dictionary<Formating, string> Format = new Dictionary<Formating, string>() {
			{ Formating.Number, @"[0-9]" },
			{ Formating.Line, @"^[a-zA-Z0-9]+$" }
		};

		/// <summary>
		/// To read line as number or regular line with blocked keys
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public string ReadLine(Formating format)
		{
			System.Console.Write(">>> ");

			ConsoleKeyInfo keyInfo;
			ConsoleKey key;
			string keyString;

			bool isCommand = false;

			string buf = "";

			string regex = Format.GetValueOrDefault(format, @"^[a-zA-Z0-9]+$");
			string oldRegex = regex;

			bool endRead = false;
			while (!endRead)
			{
				keyInfo = System.Console.ReadKey(true);
				key = keyInfo.Key;
				keyString = keyInfo.KeyChar.ToString();

				string write = "";

				switch (key)
				{
					case ConsoleKey.Enter:
						{
							if (buf.Length != 0)
							{
								write = "\n";
								endRead = true;
							}
							break;
						}
					case ConsoleKey.Backspace:
						{
							if (buf.Length != 0)
							{
								buf = buf.Remove(buf.Length - 1, 1);
								write = "\b \b";
							}
							break;
						}
					case ConsoleKey.Oem2: // is '/'
						{
							if (buf.Length == 0)
							{
								System.Console.ForegroundColor = ConsoleColor.Yellow;
								if (!regex.Contains("a-zA-Z"))
								{
									regex = @"^[a-zA-Z0-9]+$";
								}
								isCommand = true;
								buf += keyString;
								write = keyString;
							}
							break;
						}
					case ConsoleKey.OemMinus:
						{
							if (buf.Length > 0 && isCommand)
							{
								System.Console.ForegroundColor = ConsoleColor.Yellow;
								buf += keyString;
								write = keyString;
							}
							break;
						}
					case ConsoleKey.Spacebar:
						{
							break;
						}
					default:
						{
							if (Regex.IsMatch(keyString, regex) && (buf.Length > 0 || keyString != "0"))
							{
								if (isCommand)
									System.Console.ForegroundColor = ConsoleColor.Yellow;
								buf += keyString;
								write = keyString;
							}
							break;
						}
				}
				if (buf.Length == 0 && isCommand)
				{
					isCommand = false;
					regex = oldRegex;
				}
				System.Console.Write(write);
				System.Console.ResetColor();
			}
			return buf;
		}

		public void WriteLine(string text)
		{
			System.Console.ForegroundColor = ConsoleColor.Blue;
			System.Console.WriteLine(text);
			System.Console.ResetColor();
		}
	}
}
