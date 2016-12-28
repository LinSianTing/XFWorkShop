using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using XFoAuth2.iOS.Services;
using XFoAuth2.Services;


[assembly: Xamarin.Forms.Dependency(typeof(MyiOSAccountStore))]
namespace XFoAuth2.iOS.Services
{
    /// <summary>
    /// �Ω�ѨM Xamarin.Auth �M��A�{���q(2016.12.20)�L�k�ϥΩ� UWP ���x�һs�@������
    /// �w�� iOS ���x�A�ä��ݭn������Ȼs�ơA�]���AiOS ���x�N�|�ϥ� Xamarin.Auth���Ѫ����إ\��
    /// </summary>
    class MyiOSAccountStore : IAccountStore
    {
        public Task<Account> GetAccount()
        {
            return Task.FromResult<Account>(null);
        }

        public string GetPlatform()
        {
            return "iOS";
        }

        public Task SaveAccount(Account account)
        {
            return Task.FromResult(0);
        }
    }
}