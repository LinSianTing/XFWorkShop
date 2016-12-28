using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Auth;
using XFoAuth2.Services;
using XFoAuth2.Droid.Services;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(MyAndroidAccountStore))]
namespace XFoAuth2.Droid.Services
{
    /// <summary>
    /// �Ω�ѨM Xamarin.Auth �M��A�{���q(2016.12.20)�L�k�ϥΩ� UWP ���x�һs�@������
    /// �w�� Android ���x�A�ä��ݭn������Ȼs�ơA�]���AAndroid���x�N�|�ϥ� Xamarin.Auth���Ѫ����إ\��
    /// </summary>
    public class MyAndroidAccountStore : IAccountStore
    {
        public Task<Account> GetAccount()
        {
            return Task.FromResult<Account>(null);
        }

        public string GetPlatform()
        {
            return "Android";
        }

        public Task SaveAccount(Account account)
        {
            return Task.FromResult(0);
        }
    }
}