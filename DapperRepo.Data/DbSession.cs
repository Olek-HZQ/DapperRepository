using System;
using System.Data;
using DapperRepo.Core.Data;

namespace DapperRepo.Data
{
    public class DbSession : IDbSession
    {
        public DbSession(IDbConnection conn)
        {
            Connection = conn;
        }

        public IDbConnection Connection { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public IDbTransaction BeginTrans(IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            Transaction = Connection.BeginTransaction(isolation);

            return Transaction;
        }

        public void Commit()
        {
            Transaction.Commit();
        }

        public void Rollback()
        {
            Transaction.Rollback();
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                Connection.Dispose();
                Connection = null;
            }

            if (Transaction != null)
            {
                Transaction.Dispose();
                Transaction = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
