using System;
using System.Data;
using System.Data.SqlClient;

namespace Bespoke.Common.Data
{
    /// <summary>
	/// Provides a wrapper for ADO.NET data access using the SQL data provider.
    /// </summary>
    public class SqlDataProvider : IDisposable
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
        public SqlDataReader DataReader
        {
            get
            {
                return mDataReader;
            }
        }

        /// <summary>
		/// Gets or sets the query to perform.
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
        /// 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return (mConnection.State == ConnectionState.Open);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SqlParameterCollection Parameters
        {
            get
            {
                return mCommand.Parameters;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlDataProvider(string connectionString)
        {
            mConnectionString = connectionString;
            mConnection = new SqlConnection(mConnectionString);
            mCommand = new SqlCommand();
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
                        return (T)DefaultIntegerObject;

                    default:
                        return default(T);
                }
            }
            else
            {
                return (T)value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            ClosePersistentConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenPersistentConnection()
        {
            if (mConnection.State != ConnectionState.Open)
            {
                mConnection.Open();
            }

            mUsePersistentConnection = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClosePersistentConnection()
        {
            if (mConnection.State != ConnectionState.Closed)
            {
                mConnection.Close();
            }

            mUsePersistentConnection = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void AddParameter(SqlParameter parameter)
        {
            mCommand.Parameters.Add(parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlParameter AddParameter(string name, SqlDbType type, ParameterDirection direction, object value)
        {
            SqlParameter parameter = new SqlParameter(name, type);
            parameter.Direction = direction;
            parameter.Value = value;
            mCommand.Parameters.Add(parameter);

            return parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlParameter AddParameter(string name, SqlDbType type, ParameterDirection direction, int size, object value)
        {
            SqlParameter parameter = new SqlParameter(name, type, size);
            parameter.Direction = direction;
            parameter.Value = value;
            mCommand.Parameters.Add(parameter);

            return parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public SqlParameter AddOutputParameter(string name, SqlDbType type, int size)
        {
            SqlParameter parameter = new SqlParameter(name, type, size);
            parameter.Direction = ParameterDirection.Output;
            mCommand.Parameters.Add(parameter);

            return parameter;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        public SqlParameter AddReturnValueParameter()
        {
            SqlParameter parameter = new SqlParameter("@ReturnValue", SqlDbType.Int);
            parameter.Direction =  ParameterDirection.ReturnValue;
            mCommand.Parameters.Add(parameter);

            return parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isolationLevel"></param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            mTransaction = mConnection.BeginTransaction(isolationLevel);
            mCommand.Transaction = mTransaction;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public void ExecuteQuery()
        {
            try
            {
                if (mUsePersistentConnection == false)
                {
                    mConnection.Open();
                }

                mDataReader = mCommand.ExecuteReader();
            }
            finally
            {
                if (mUsePersistentConnection == false)
                {
                    mConnection.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            try
            {
                if (mUsePersistentConnection == false)
                {
                    mConnection.Open();
                }

                return mCommand.ExecuteNonQuery();
            }
            finally
            {
                if (mUsePersistentConnection == false)
                {
                    mConnection.Close();
                }
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            try
            {
                if (mUsePersistentConnection == false)
                {
                    mConnection.Open();
                }

                return mCommand.ExecuteScalar();
            }
            finally
            {
                if (mUsePersistentConnection == false)
                {
                    mConnection.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndQuery()
        {
            if (mDataReader != null)
            {
                mDataReader.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetQuery()
        {
            if (mTransaction != null)
            {
                mTransaction.Commit();
                mTransaction = null;
            }

            EndQuery();
            mCommand.CommandText = String.Empty;
            mCommand.Parameters.Clear();            
            mDataReader = null;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        public void SetCommandText(CommandType commandType, string commandText)
        {
            mCommand.CommandType = commandType;
            mCommand.CommandText = commandText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ReadValue<T>(string key)
        {
            T value = default(T);
            Type type = typeof(T);

            if ((mDataReader != null) && (mDataReader.HasRows))
            {
                if (type.IsEnum)
                {
                    object valueObject = (Convert.ToInt32(mDataReader[key]));
                    value = (T)valueObject;
                }
                else
                {
                    switch (type.Name)
                    {
                        case "Nullable`1":
                            {
                                if (type.FullName.Contains("Boolean"))
                                {
                                    byte valueByte = Convert.ToByte(mDataReader[key]);
                                    object valueObject;
                                    if (valueByte == byte.MaxValue)
                                    {
                                        valueObject = null;
                                    }
                                    else
                                    {
                                        valueObject = Convert.ToBoolean(valueByte);
                                    }

                                    value = (T)valueObject;
                                }
                                else
                                {
                                    value = (T)mDataReader[key];
                                }
                            }
                            break;

                        case "Boolean":
                            {
                                object valueObject = Convert.ToBoolean(mDataReader[key]);
                                value = (T)valueObject;
                            }
                            break;

                        default:
                            {
                                value = SqlDataProvider.CheckDBNull<T>(mDataReader[key]);
                            }
                            break;
                    }
                }
            }

            return value;            
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly int DefaultKey = -1;

        private static readonly object DefaultIntegerObject = -1;

        private string mConnectionString;
        private SqlConnection mConnection;
        private SqlCommand mCommand;
        private SqlDataReader mDataReader;
        private SqlTransaction mTransaction;
        private bool mUsePersistentConnection;
    }
}
