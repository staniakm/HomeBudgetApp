using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.service
{
    
    public class BankAccountService
    {
        private readonly SqlEngine sqlEngine;

        public BankAccountService(SqlEngine sqlEngine)
        {
            this.sqlEngine = sqlEngine;
        }

        public ObservableCollection<BankAccount> GetBankAccounts()
        {
            return sqlEngine.GetBankAccountsCollection();
        }

        public void ModifyBankAccount(Dictionary<string, string> tmpDic)
        {
            sqlEngine.ModifyBankAccount(tmpDic);
        }

    }
}
