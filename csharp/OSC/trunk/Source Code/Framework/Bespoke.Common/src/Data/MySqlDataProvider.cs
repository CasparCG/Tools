using System;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Bespoke.Common.Data
{
	/// <summary>
	/// Provides a wrapper for ADO.NET data access using the MySQL .NET
	/// data provider.
	/// </summary>
	public class MySqlDataProvider
	{
		/// <summary>
		/// Gets the ConnectionString value.
		/// </summary>
		public string ConnectionString
		{
			get
			{
				return mConnectionString;
			}
		}

		/// <summary>
		/// Gets a reference to the DataReader object.
		/// </summary>
		public MySqlDataReader DataReader
		{
			get
			{
				return mDataReader;
			}
		}

		/// <summary>
		/// Gets or sets the actual query to perform.
		/// </summary>
		public string CommandText
		{
			get
			{
				return mCommand.CommandText;
			}
			set
			{
				mCommand.CommandText = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of query to execute.
		/// </summary>
		public CommandType CommandType
		{
			get
			{
				return mCommand.CommandType;
			}
			set
			{
				mCommand.CommandType = value;
			}
		}

		/// <summary>
		/// Gets a value indicating if the command is setup to execute
		/// within a database transaction. The value equals "true" if
		/// it will execute within a transaction, otherwise it equals
		/// "false".
		/// </summary>
		public bool InTransaction
		{
			get
			{
				return mCommand.Transaction != null;
			}
		}

		/// <summary>
		/// Gets a value indicating if the connection is open. The value equals
		/// "true" if the connection is open, otherwise it equals "false".
		/// </summary>
		public bool IsConnectionOpen
		{
			get
			{
				return mPersistentConnectionReferences > 0;
			}
		}

		/// <summary>
		/// Instantiates a new instance of the MySqlDataProvider class.
		/// </summary>
		/// <param name="connectionString">The database connection string.</param>
		public MySqlDataProvider(string connectionString)
		{
			mConnectionString = connectionString;
			mConnection = new MySqlConnection(mConnectionString);
			mCommand = new MySqlCommand();
			mCommand.Connection = mConnection;
			mCommand.CommandType = CommandType.StoredProcedure;
		}

		/// <summary>
		/// Checks the database value for null.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="value">The database value.</param>
		/// <returns>Returns the devault value for the property type if
		/// the value is null, otherwise returns the database value.
		/// </returns>
		public static T CheckDBNull<T>(object value)
		{
			if (value == DBNull.Value)
			{
				Type type = typeof(T);
				switch (type.Name)
				{
					case "Int16":
					case "Int32":
					case "Int64":
						return (T)DEFAULT_INTEGER_OBJECT;

					default:
						return default(T);
				}
			}
			else
			{
				return (T)value;
			}
		}

		/// <overrrides>
		/// Adds a stored procedure parameter object to the command
		/// object parameters collection.
		/// </overrrides>
		/// <summary>
		/// Adds a stored procedure parameter object to the command
		/// object parameters collection.
		/// </summary>
		/// <param name="parameter">The parameter object.</param>
		public void AddParameter(MySqlParameter parameter)
		{
			mCommand.Parameters.Add(parameter);
		}

		/// <summary>
		/// Creates a parameter object that does not require a size from the
		/// passed values and adds it to the command object parameters collection.
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="type">The parameter type.</param>
		/// <param name="direction">The parameter direction.</param>
		/// <param name="value">The parameter value.</param>
		/// <returns>Returns a reference to the added parameter object.</returns>
		public MySqlParameter AddParameter(string name, MySqlDbType type, ParameterDirection direction, object value)
		{
			MySqlParameter parameter = new MySqlParameter(name, type);
			parameter.Direction = direction;
			parameter.Value = value;
			mCommand.Parameters.Add(parameter);

			return parameter;
		}

		/// <summary>
		/// Creates a parameter object that requires a size from the passed values
		/// and adds it to the command object parameters collection.
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="type">The parameter type.</param>
		/// <param name="direction">The parameter direction.</param>
		/// <param name="size">The parameter value size.</param>
		/// <param name="value">The parameter value.</param>
		/// <returns>Returns a reference to the added parameter object.</returns>
		public MySqlParameter AddParameter(string name, MySqlDbType type, ParameterDirection direction, int size, object value)
		{
			MySqlParameter parameter = new MySqlParameter(name, type, size);
			parameter.Direction = direction;
			parameter.Value = value;
			mCommand.Parameters.Add(parameter);

			return parameter;
		}

		/// <summary>
		/// Creates a return value parameter and adds it to the command object parameters collection.
		/// </summary>
		/// <returns></returns>
		public MySqlParameter AddReturnValueParameter()
		{
			MySqlParameter parameter = new MySqlParameter("?_ReturnValue", SqlDbType.Int);
			parameter.Direction = ParameterDirection.Output;
			mCommand.Parameters.Add(parameter);

			return parameter;
		}

		/// <summary>
		/// Sets the command object to begin a transaction with the specified isolation level.
		/// </summary>
		/// <param name="isolationLevel">The isolation level.</param>
		public void BeginTransaction(IsolationLevel isolationLevel)
		{
			mTransaction = mConnection.BeginTransaction(isolationLevel);
			mCommand.Transaction = mTransaction;
		}

		/// <summary>
		/// Decrements the connection reference count and closes the connection
		/// if there are no more connection references.
		/// </summary>
		public void ClosePersitentConnection()
		{
			mPersistentConnectionReferences--;

			if (mPersistentConnectionReferences == 0)
			{
				mConnection.Close();
			}
		}

		/// <summary>
		/// Removes all connection reference counters and closes the connection.
		/// </summary>
		public void CloseAllPersitentConnections()
		{
			while (IsConnectionOpen)
			{
				ClosePersitentConnection();
			}
		}

		/// <summary>
		/// Commits the transaction.
		/// </summary>
		public void CommitTransaction()
		{
			if (mTransaction == null)
			{
				throw new DataException("Not in a transaction.");
			}

			mTransaction.Commit();
		}

		/// <summary>
		/// Opens the connection or increments the connection reference
		/// count if the connection is already open.
		/// </summary>
		public void OpenPersistentConnection()
		{
			if (mPersistentConnectionReferences == 0)
			{
				mConnection.Open();
			}

			mPersistentConnectionReferences++;
		}

		/// <summary>
		/// Executes a result query.
		/// </summary>
		public void ExecuteQuery()
		{
			if (IsConnectionOpen == false)
			{
				OpenTemporaryConnection();
			}

			mDataReader = mCommand.ExecuteReader();
		}

		/// <summary>
		/// Executes a non-result query.
		/// </summary>
		/// <returns>Returns a count of the affected rows.</returns>
		public int ExecuteNonQuery()
		{
			if (IsConnectionOpen == false)
			{
				OpenTemporaryConnection();
			}

			return mCommand.ExecuteNonQuery();
		}

		/// <summary>
		/// Executes a query that returns a scalar value.
		/// </summary>
		/// <returns>Returns the scalar object.</returns>
		public object ExecuteScalar()
		{
			if (IsConnectionOpen == false)
			{
				OpenTemporaryConnection();
			}

			return mCommand.ExecuteScalar();
		}

		/// <summary>
		/// Prepares for the next query by committing any pending transactions,
		/// closing the data reader and clearing the command text and parameters.
		/// </summary>
		public void ResetQuery()
		{
			if (mTransaction != null)
			{
				mTransaction.Commit();
				mTransaction = null;
			}

			if (mDataReader != null)
			{
				mDataReader.Close();
				mDataReader = null;
			}

			if (mTemporaryConnectionMade)
			{
				CloseTemporaryConnection();
			}

			mCommand.CommandText = String.Empty;
			mCommand.Parameters.Clear();
		}

		/// <summary>
		/// Performs a rollback on the transaction.
		/// </summary>
		public void RollbackTransaction()
		{
			if (mTransaction == null)
			{
				throw new DataException("Not in a transaction.");
			}

			mTransaction.Rollback();
		}

		/// <summary>
		/// Sets the command type and text from the specified parameters.
		/// </summary>
		/// <param name="commandType">The command type.</param>
		/// <param name="commandText">The command text.</param>
		public void SetCommandText(CommandType commandType, string commandText)
		{
			mCommand.CommandType = commandType;
			mCommand.CommandText = commandText;
		}

		#region Private Methods

		/// <summary>
		/// Make a temporary (non-persistent) database connection.
		/// </summary>
		private void OpenTemporaryConnection()
		{
			if (mPersistentConnectionReferences == 0)
			{
				mConnection.Open();
				mTemporaryConnectionMade = true;
			}
		}

		/// <summary>
		/// Close a temporary database connection.
		/// </summary>
		private void CloseTemporaryConnection()
		{
			if (mPersistentConnectionReferences == 0 && mTemporaryConnectionMade)
			{
				mConnection.Close();
				mTemporaryConnectionMade = false;
			}
		}

		#endregion

		private static readonly object DEFAULT_INTEGER_OBJECT = -1;

		private string mConnectionString;
		private MySqlConnection mConnection;
		private MySqlCommand mCommand;
		private MySqlDataReader mDataReader;
		private MySqlTransaction mTransaction;
		private int mPersistentConnectionReferences;
		private bool mTemporaryConnectionMade;
	}
}