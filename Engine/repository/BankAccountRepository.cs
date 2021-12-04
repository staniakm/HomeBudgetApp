using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Engine.repository
{
    public class BankAccountRepository
    {
        private readonly SqlEngine sqlEngine;

        public BankAccountRepository()
        {
            this.sqlEngine = SqlEngine.GetInstance();
        }


        /// <summary>
        /// Aktualizauje kolekcję kont. Można ustawiać bezpośrednio do datacontextu.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<BankAccount> GetBankAccountsCollection()
        {
            ObservableCollection<BankAccount> konta = new ObservableCollection<BankAccount>();
            DataTable dt = sqlEngine.GetData(@"SELECT account_name, a.id, money, a.description, o.id as owner_id, o.owner_name FROM account a
                                                join account_owner o on a.owner = o.id 
                                                where del = false order by a.id
                                                                ;");
            foreach (DataRow item in dt.Rows)
            {
                konta.Add(new BankAccount((int)item["id"],
                    Convert.ToString(item["account_name"]),
                    Convert.ToDecimal(item["money"]),
                    Convert.ToString(item["description"]),
                    Convert.ToInt32(item["owner_id"]),
                    Convert.ToString(item["owner_name"])
                    ));
            }
            return konta;
        }

        public void ModifyBankAccount(Dictionary<string, string> tmpDic)
        {
            sqlEngine.SQLexecuteNonQuerryProcedure("call dbo.bankAccountModification", tmpDic);
        }

    }
}
