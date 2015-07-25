// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace common
{
	public class Database : IDisposable
	{
		private readonly MySqlConnection Connection;

		/// <summary>
		/// Creates a database connection
		/// </summary>
		/// <param name="conString"></param>
		public Database(string conString)
		{
			Connection = new MySqlConnection(conString);
		}

		/// <summary>
		/// Tests the current connection
		/// </summary>
		/// <returns>true if success, false otherwise.</returns>
		public bool Test()
		{
			if (!Open())
				return false;
			return true;
		}

		/// <summary>
		/// Opens a MySQL Connection
		/// </summary>
		protected bool Open()
		{
			try
			{
				if (Connection == null)
				{
					ConsoleUtils.Write(ConsoleMsgType.SQL, "Trying to open a non-existant connection.\n");
					return false;
				}

				if (Connection.State != System.Data.ConnectionState.Open)
				{
					Connection.Open();
				}

				return true;
			}
			catch (Exception e)
			{
				ConsoleUtils.Write(ConsoleMsgType.SQL, "Failed to open a MySQL Connection. Error: {0}\n", e.Message);
				return false;
			}
		}

		/// <summary>
		/// Executes a query to retrieve data
		/// </summary>
		/// <param name="query">the query</param>
		/// <param name="parNames">parameters name</param>
		/// <param name="parVals">parameters value</param>
		/// <returns>A reader with the result</returns>
		public MySqlDataReader ReaderQuery(string query, string[] parNames, object[] parVals)
		{
			if (!Open()) return null;

			if (parNames == null) parNames = Globals.NullStrArray;
			if (parVals == null) parVals = Globals.NullObjArray;
			using (MySqlCommand cmd = new MySqlCommand(query, Connection))
			{
				for(int i = 0; i < parNames.Length; i++)
				{
					cmd.Parameters.AddWithValue(parNames[i], parVals[i]);
				}

				return cmd.ExecuteReader();
			}
		}

		/// <summary>
		/// Executes a query to change data
		/// </summary>
		/// <param name="query">the query</param>
		/// <param name="parNames">parameters name</param>
		/// <param name="parVals">parameters value</param>
		/// <returns></returns>
		public long WriteQuery(string query, string[] parNames, object[] parVals)
		{
			if (!Open()) return -1;

			using (MySqlCommand cmd = new MySqlCommand(query, Connection))
			{
				for(int i = 0; i < parNames.Length; i++)
				{
					cmd.Parameters.AddWithValue(parNames[i], parVals[i]);
				}

				cmd.ExecuteNonQuery();

				return cmd.LastInsertedId;
			}
		}

		// ============================
		// Garbage Collection Methods
		// ============================
		
		/// <summary>
		/// Free this object from memory
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this); // Garbage  Collector doesn't need to finalize this
		}

		/// <summary>
		/// Deconstructor (Finalize)
		/// </summary>
		~Database()
		{
			Dispose(false);
		}

		protected void Dispose(bool disposing)
		{
			if (Connection != null)
			{
				if (Connection.State != ConnectionState.Closed)
				{
					Connection.Dispose();
				}
			}
		}
	}
}
