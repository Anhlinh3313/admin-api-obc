using System;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class EntityProc : IEntityProc
    {
        private readonly string _query;
        private readonly SqlParameter[] _pars;

        public EntityProc(string query, SqlParameter[] pars)
        {
            _query = query;
            _pars = pars;
        }

        public SqlParameter[] GetParams()
        {
            return _pars;
        }

        public string GetQuery()
        {
            return _query;
        }
    }
}
