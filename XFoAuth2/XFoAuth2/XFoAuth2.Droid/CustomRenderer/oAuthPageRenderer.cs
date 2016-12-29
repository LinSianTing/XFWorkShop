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
using Xamarin.Forms;
using XFoAuth2.Views;
using XFoAuth2.Droid;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;
using XFoAuth2.Helpers;
using Prism.Unity;
using Prism.Navigation;
using Microsoft.Practices.Unity;
using Prism.Events;
using XFoAuth2.Models;
using Newtonsoft.Json;
using XFoAuth2.Droid.CustomRenderer;

[assembly: ExportRenderer(typeof(oAuthPage), typeof(oAuthPageRenderer))]
namespace XFoAuth2.Droid.CustomRenderer
{
    // https://developer.xamarin.com/guides/xamarin-forms/custom-renderer/contentpage/
    /// <summary>
    /// �Ȼs�� OAuth2 �n�J�����n Renderer �������]�w
    /// </summary>
    public class oAuthPageRenderer : PageRenderer
    {
        /// <summary>
        /// �o�ӭ����O�_�w�g��ܥX�ӤF
        /// </summary>
        private bool _isShown;

        /// <summary>
        /// OnElementChanged method is called when the corresponding Xamarin.Forms control is created.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (_isShown) return;
            _isShown = true;

            OAuth2Authenticator auth;
            OAuth2AuthenticatorParameter fooPara = new OAuth2AuthenticatorParameter();

            #region �̾ګ��w�� OAuth2 �{�Ҥ覡�A���o�ӻ{�ҭn�Ψ쪺�����Ѽ�
            if (AuthenticationHelper.OAuthType == OAuthTypeEnum.Google)
            {
                fooPara = AuthenticationHelper.OAuthParas.FirstOrDefault(x => x.Type == OAuthTypeEnum.Google);

            }
            else if (AuthenticationHelper.OAuthType == OAuthTypeEnum.Facebook)
            {
                fooPara = AuthenticationHelper.OAuthParas.FirstOrDefault(x => x.Type == OAuthTypeEnum.Facebook);
            }
            #endregion

            var fooauthorizeUrl = fooPara.authorizeUrl;
            var fooredirectUrl = fooPara.redirectUrl;
            var fooaccessTokenUrl = fooPara.accessTokenUrl;
            var fooclientId = fooPara.clientId;
            var fooclientSecret = fooPara.clientSecret;
            var fooscope = fooPara.scope;
            auth = new OAuth2Authenticator(
                fooclientId,
                fooclientSecret,
                fooscope,
                new Uri(fooauthorizeUrl),
                new Uri(fooredirectUrl),
                new Uri(fooaccessTokenUrl));

            auth.Completed += OnAuthenticationCompleted;

            // Display the UI
            var activity = Context as Activity;
            activity?.StartActivity(auth.GetUI(activity));
        }

        /// <summary>
        /// �{�ҧ����᪺ Callback �ƥ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            // ���o Prism �̩ۨʪA�ȨϥΨ쪺�e��
            IUnityContainer fooContainer = (XFoAuth2.App.Current as PrismApplication).Container;
            // ���o IAccountStore ������ڹ�@�����O����
            var fooIEventAggregator = fooContainer.Resolve<IEventAggregator>();

            if (e.IsAuthenticated)
            {
                await AuthenticationHelper.FetchUserProfile(e.Account);
                fooIEventAggregator.GetEvent<AuthEvent>().Publish(AuthEventEnum.�������Ҧ��\);
            }
            else
            {
                fooIEventAggregator.GetEvent<AuthEvent>().Publish(AuthEventEnum.�������ҥ���);
            }
        }
    }
}