using System;

namespace Pilot2.Services.Chat
{
	interface IWriter
	{
		void WriteError(string text);
		void WriteLine(string text);
	}
}
