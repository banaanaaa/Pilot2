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

		public void Write(string text) => Write(text, ConsoleColor.Cyan);
		public void WriteLine(string text) => WriteLine(text, ConsoleColor.Cyan);
		public void WriteLineInfo(string text) => WriteLine(text, ConsoleColor.Magenta);
		public void WriteLineError(string text) => WriteLine(text, ConsoleColor.Red);
		private void Write(string text, ConsoleColor color)
		{
			System.Console.ForegroundColor = color;
			System.Console.Write(text);
			System.Console.ResetColor();
		}
		private void WriteLine(string text, ConsoleColor color)
		{
			System.Console.ForegroundColor = color;
			System.Console.WriteLine(text);
			System.Console.ResetColor();
		}

		/// <summary>
		/// To read line as number or regular line with blocked keys
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public string ReadLine(Formating format)
		{
			Write(">>> ");

			bool isCommand = false;
			string buf = "";
			string regex = Format.GetValueOrDefault(format, @"^[a-zA-Z0-9]+$");
			string oldRegex = regex;

			bool endRead = false;
			while (!endRead)
			{
				var keyInfo = System.Console.ReadKey(true);
				var key = keyInfo.Key;
				var keyString = keyInfo.KeyChar.ToString();

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
								buf += keyString;
								write = keyString;
							}
							break;
						}
				}
				if (isCommand)
				{
					Write(write, ConsoleColor.Yellow);
					if (buf.Length == 0)
					{
						isCommand = false;
						regex = oldRegex;
					}
				}
				else
				{
					Write(write);
				}
			}
			return buf;
		}
	}
}
