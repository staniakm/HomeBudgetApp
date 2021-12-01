using Engine.repository;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine.service
{

    public class BankAccountService
    {
        private readonly BankAccountRepository repository;

        public BankAccountService(BankAccountRepository repository)
        {
            this.repository = repository;
        }

        public ObservableCollection<BankAccount> GetBankAccounts()
        {
            return repository.GetBankAccountsCollection();
        }

        public void ModifyBankAccount(Dictionary<string, string> tmpDic)
        {
            repository.ModifyBankAccount(tmpDic);
        }

    }
}
