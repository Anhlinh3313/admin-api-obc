using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Infrastructure
{
    public sealed class Connection
    {
        private static Connection mInstance = null;
        private string mConnectionString;
        private static readonly object mPadlock = new object();

        private Connection(string connectionString)
        {
            mConnectionString = connectionString;
        }

        public static Connection Instance
        {
            get
            {
                lock (mPadlock)
                {
                    if (mInstance == null)
                    {
                        throw new Exception("Connection is not initialization!");
                    }
                    return mInstance;
                }
            }
        }

        public string GetConnectionString()
        {
            return mInstance.mConnectionString;
        }

        public static void Create(string connectionString)
        {
            if (mInstance != null)
            {
                throw new Exception("Object already created");
            }
            mInstance = new Connection(connectionString);
        }
    }
}
