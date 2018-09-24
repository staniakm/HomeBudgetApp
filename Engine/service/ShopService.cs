using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.service
{
    public class ShopService
    {
        private SqlEngine _sql;

        public ShopService(SqlEngine sql)
        {
            _sql = sql;
        }
    }
}
