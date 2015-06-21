// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common
{
	/// <summary>
	/// The types of messages to write to
	/// the console.
	/// </summary>
	[Flags]
	public enum ConsoleMsgType
	{
		None = 0,
		Info = 1,
		Status = 2,
		Notice = 4,
		Warning = 8,
		Error = 16,
		Debug = 32,
		SQL = 64,
		FatalError = 128,
		PacketDebug = 256
	}

	/// <summary>
	/// This class do special handling for console
	/// outputs, like omitting messages or rewritting lines.
	/// </summary>
	public static class ConsoleUtils
	{
		static object WriteLock = new object();

		/// <summary>
		/// Stores what should be omitted.
		/// </summary>
		private static ConsoleMsgType noDisplay = new ConsoleMsgType();

		/// <summary>
		/// Displays the emulator name
		/// </summary>
		public static void ShowHeader()
		{
			Console.BackgroundColor = ConsoleColor.Green;
			Console.ForegroundColor = ConsoleColor.White;
			
			Console.Write("====================================================\r\n");
			Console.Write("=                 Tartarus Emulator                =\r\n");
			Console.Write("=      _____          _                            =\r\n");
			Console.Write("=     |_   _|        | |                           =\r\n");
			Console.Write("=       | | __ _ _ __| |_ __ _ _ __ _   _ ___      =\r\n");
			Console.Write("=       | |/ _` | '__| __/ _` | '__| | | / __|     =\r\n");
			Console.Write("=       | | (_| | |  | || (_| | |  | |_| \\__ \\     =\r\n");
			Console.Write("=       \\_/\\__,_|_|   \\__\\__,_|_|   \\__,_|___/     =\r\n");
			Console.Write("=                                                  =\r\n");
			Console.Write("=                                                  =\r\n");
			Console.Write("====================================================\r\n");
			
			Console.ResetColor();
		}

		/// <summary>
		/// Writes a message to the console, using prefixes.
		/// </summary>
		/// <param name="type">the type of output</param>
		/// <param name="text">the message</param>
		/// <param name="replacers">text replacers</param>
		public static void Write(ConsoleMsgType type, string text, params object[] replacers)
        {
			// If this type of message must be silenced
			if (noDisplay.HasFlag(type))
				return;

			lock(WriteLock)
			{
				switch (type)
				{
					case ConsoleMsgType.Status:
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write("[Status] ");
						Console.ResetColor();
						break;

					case ConsoleMsgType.SQL:
						Console.ForegroundColor = ConsoleColor.Magenta;
						Console.Write("[SQL] ");
						Console.ResetColor();
						break;

					case ConsoleMsgType.Info:
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write("[Info] ");
						Console.ResetColor();
						break;

					case ConsoleMsgType.Notice:
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write("[Notice] ");
						Console.ResetColor();
						break;

					case ConsoleMsgType.Warning:
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.Write("[Warning] ");
						Console.ResetColor();
						break;

					case ConsoleMsgType.Debug:
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.Write("[Debug] ");
						Console.ResetColor();
						break;

					case ConsoleMsgType.Error:
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write("[Error] ");
						Console.ResetColor();
						break;

					case ConsoleMsgType.FatalError:
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write("[Fatal Error] ");
						Console.ResetColor();
						break;

					case ConsoleMsgType.PacketDebug:
						// When doing a packet debug, it must not have replacers
						// because packet data might have '{' which will crash
						// the server
						Console.Write(text);
						return;

					default:
						Console.WriteLine("In ConsoleUtils -> Write(): Invalid type flag passed\n");
						break;
				}
				Console.Write(text, replacers);
			}
        }

		/// <summary>
		/// Rewrites the content of a line with a new one.
		/// </summary>
		/// <param name="type">the type of the new message</param>
		/// <param name="text">the message</param>
		/// <param name="replacers">text replacers</param>
		public static void ReWriteLine(ConsoleMsgType type, string text, params object[] replacers)
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(" ");
            Console.SetCursorPosition(0, currentLineCursor);
            Write(type, text);
        }

		/// <summary>
		/// Sets the omitting settings.
		/// </summary>
		/// <param name="settings">settings</param>
		public static void SetDisplaySettings(ConsoleMsgType settings)
        { noDisplay = settings; }

		// Adapted From: http://www.codeproject.com/Articles/36747/Quick-and-Dirty-HexDump-of-a-Byte-Array
		/// <summary>
		/// Display the byte array with its ASCII correspondence
		/// </summary>
		/// <param name="bytes">the byte array</param>
		/// <param name="description">a description of the data</param>
		/// <param name="packetId">Id of the packet</param>
		/// <param name="packetLen">the lenght of the packet</param>
		/// <param name="bytesPerLine">how many bytes in each line</param>
		public static void HexDump(byte[] bytes, string description = "", int packetId = 0, int packetLen = 0, int bytesPerLine = 16)
		{
			if (bytes == null)
			{
				Write(ConsoleMsgType.Warning, "NULL byte array to dump.\n");
				return;
			}

			int bytesLength = bytes.Length;

			char[] HexChars = "0123456789ABCDEF".ToCharArray();

			int firstHexColumn =
				  8                   // 8 characters for the address
				+ 3;                  // 3 spaces

			int firstCharColumn = firstHexColumn
				+ bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
				+ (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
				+ 2;                  // 2 spaces 

			int lineLength = firstCharColumn
				+ bytesPerLine           // - characters to show the ascii value
				+ Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

			char[] line = (new String(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
			int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
			StringBuilder result = new StringBuilder(expectedLines * lineLength);

			for (int i = 0; i < bytesLength; i += bytesPerLine)
			{
				line[0] = HexChars[(i >> 28) & 0xF];
				line[1] = HexChars[(i >> 24) & 0xF];
				line[2] = HexChars[(i >> 20) & 0xF];
				line[3] = HexChars[(i >> 16) & 0xF];
				line[4] = HexChars[(i >> 12) & 0xF];
				line[5] = HexChars[(i >> 8) & 0xF];
				line[6] = HexChars[(i >> 4) & 0xF];
				line[7] = HexChars[(i >> 0) & 0xF];

				int hexColumn = firstHexColumn;
				int charColumn = firstCharColumn;

				for (int j = 0; j < bytesPerLine; j++)
				{
					if (j > 0 && (j & 7) == 0) hexColumn++;
					if (i + j >= bytesLength)
					{
						line[hexColumn] = ' ';
						line[hexColumn + 1] = ' ';
						line[charColumn] = ' ';
					}
					else
					{
						byte b = bytes[i + j];
						line[hexColumn] = HexChars[(b >> 4) & 0xF];
						line[hexColumn + 1] = HexChars[b & 0xF];
						line[charColumn] = (b < 32 ? '·' : (char)b);
					}
					hexColumn += 3;
					charColumn++;
				}
				result.Append(line);
			}

			Write(ConsoleMsgType.PacketDebug, String.Format("Description: {0}\n", description));
			if (packetId > 0)
				Write(ConsoleMsgType.PacketDebug, String.Format("Packet ID: {0} (0x{0:X2}) ------ Lenght: {1}\n", packetId, packetLen));
			Write(ConsoleMsgType.PacketDebug, result.ToString() + "\n");

			return;
		}
	}
}
