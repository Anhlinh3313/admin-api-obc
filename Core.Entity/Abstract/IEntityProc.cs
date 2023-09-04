using System;
using Microsoft.Data.SqlClient;

namespace Core.Entity.Abstract
{
    public interface IEntityProc : IEntityProcView
    {
		SqlParameter[] GetParams();
		string GetQuery();
    }
}
