using System;

namespace Pilot2.Services.Chat
{
	interface IWriter
	{
		void Write(string text);
		void WriteLine(string text);
		void WriteLineInfo(string text);
		void WriteLineError(string text);
	}
}
