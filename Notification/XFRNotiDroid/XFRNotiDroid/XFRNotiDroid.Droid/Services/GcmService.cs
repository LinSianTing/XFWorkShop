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
using Gcm.Client;
using XFRNotiDroid.Helpers;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Android.Support.V4.App;
using Microsoft.WindowsAzure.MobileServices;
using Android.Media;
using Android.Support.V4.Content;
using Android.Util;
using XFRNotiDroid.Models;
using Newtonsoft.Json;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]

namespace XFRNotiDroid.Droid.Services
{
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class PushHandlerBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
    {
        // �o�̭n��J Google Console ���� �M�׽s��
        public static string[] SENDER_IDS = new string[] { "300143732939" };
    }

    [Service]
    public class GcmService : GcmServiceBase
    {
        public static string RegistrationID { get; private set; }

        public GcmService()
            : base(PushHandlerBroadcastReceiver.SENDER_IDS) { }

        protected override void OnRegistered(Context context, string registrationId)
        {
            Log.Verbose("PushHandlerBroadcastReceiver", "GCM Registered: " + registrationId);
            RegistrationID = registrationId;

            #region �i�� Azure Mobile �A�ȵ��U
            // ���o Azure Mobile Client ����
            var push = GlobalHelper.AzureMobileClient.GetPush();

            // �ϥΥD������A�i�� Azure Mobile ���U
            MainActivity.CurrentActivity.RunOnUiThread(() => Register(push, null));
            #endregion
        }

        public async void Register(Microsoft.WindowsAzure.MobileServices.Push push, IEnumerable<string> tags)
        {
            #region �i��Azure �������ϵ��U
            try
            {
                // �]�w�n�����Ӧ۩� Azure �������Ϫ������T���榡
                string templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\", \"title\":\"$(titleParam)\", \"args\":\"$(argsParam)\"}}"; ;

                JObject templates = new JObject();
                templates["genericMessage"] = new JObject
                {
                    { "body", templateBodyGCM}
                };

                // ���U�n�����������T���榡
                await push.RegisterAsync(RegistrationID, templates);
                Log.Info("Push Installation Id", push.InstallationId.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Debugger.Break();
            }
            #endregion
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            #region ������q���T�������줧��A���U�ӭn�B�z���u�@
            Log.Info("PushHandlerBroadcastReceiver", "GCM Message Received!");

            #region �إߥ��a�T���q������
            string message = intent.Extras.GetString("message");
            string title = intent.Extras.GetString("title");
            string args = intent.Extras.GetString("args");
            if (!string.IsNullOrEmpty(message))
            {
                var fooNotificationPayload = new NotificationPayload()
                {
                    Page = args,
                    Title = title,
                    Message = message,
                };
                createNotification(fooNotificationPayload);
            }
            #endregion
            #endregion
        }

        void createNotification(NotificationPayload notificationPayload)
        {
            #region ���ͥ��a�ݪ������q��

            #region ���ͤ@�� Intent �A�åB�N�n�ǻ��L�h�������ѼơA�ϥ� PutExtra ��i�h
            var uiIntent = new Intent(this, typeof(MainActivity));
            // �]�w��ϥ��I��o�ӳq������A�n�ǻ��L�h�����
            var fooPayload = JsonConvert.SerializeObject(notificationPayload);
            uiIntent.PutExtra("NotificationPayload", fooPayload);
            #endregion

            //�إ� Notification Builder ����
            Notification.Builder builder = new Notification.Builder(this);

            //�]�w���a�ݱ��������e
            var notification = builder
                // �]�w���I��o�ӥ��a�ݳq�����ث�A�n��ܪ� Activity
                .SetContentIntent(PendingIntent.GetActivity(this, 0, uiIntent, PendingIntentFlags.OneShot))
                .SetSmallIcon(Android.Resource.Drawable.SymActionEmail)
                .SetTicker(notificationPayload.Title)
                .SetContentTitle(notificationPayload.Title)
                .SetContentText(notificationPayload.Message)
                //�]�w�����n��
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                //��ϥΪ��I��o�ӱ����q���A�N�|�۰ʲ����o�ӳq��
                .SetAutoCancel(true).Build();

            //���� notification ����
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

            // ��ܥ��a�q��:
            notificationManager.Notify(1, notification);
            #endregion

        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            Log.Error("PushHandlerBroadcastReceiver", "Unregistered RegisterationId : " + registrationId);
        }

        protected override void OnError(Context context, string errorId)
        {
            Log.Error("PushHandlerBroadcastReceiver", "GCM Error: " + errorId);
        }
    }

}