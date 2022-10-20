using System;
using NotificationSamples;
using UnityEngine;

namespace GameNotifications
{
    /// <summary>
    /// 本地消息推送
    /// </summary>
    public static class PushManager
    {
        private static GameObject gameObject;
        private static GameNotificationsManager manager;

        /// <summary>
        /// 发送推送之前需要先初始化一下
        /// </summary>
        public static void Init()
        {
            gameObject = GameObject.Find("PushManager");
            if (gameObject == null)
            {
                gameObject = new GameObject("PushManager");
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }

            if (manager == null)
            {
                var channel = new GameNotificationChannel("android_channel", "Game Notification", "Generic notifications");
                manager = gameObject.AddComponent<GameNotificationsManager>();
                manager.Initialize(channel);

                manager.LocalNotificationDelivered += OnDelivered;
                manager.LocalNotificationExpired += OnExpired;
            }
        }

        /// <summary>
        /// Queue a notification with the given parameters.
        /// </summary>
        /// <param name="title">The title for the notification.</param>
        /// <param name="body">The body text for the notification.</param>
        /// <param name="deliveryTime">The time to deliver the notification.</param>
        /// <param name="badgeNumber">The optional badge number to display on the application icon.</param>
        /// <param name="reschedule">
        /// Whether to reschedule the notification if foregrounding and the notification hasn't yet been shown.
        /// </param>
        /// <param name="channelId">Channel ID to use. If this is null/empty then it will use the default ID. For Android
        /// the channel must be registered in <see cref="GameNotificationsManager.Initialize"/>.</param>
        /// <param name="smallIcon">Notification small icon.</param>
        /// <param name="largeIcon">Notification large icon.</param>
        public static int? SendNotification(string title, string body, DateTime deliveryTime, int? badgeNumber = null,
                                     bool reschedule = false, string channelId = null,
                                     string smallIcon = null, string largeIcon = null)
        {
            IGameNotification notification = manager.CreateNotification();

            if (notification == null)
            {
                return null;
            }
            var channelID = "android_channel";

            notification.Title = title;
            notification.Body = body;
            notification.Group = !string.IsNullOrEmpty(channelId) ? channelId : channelID;
            notification.DeliveryTime = deliveryTime;
            notification.SmallIcon = smallIcon;
            notification.LargeIcon = largeIcon;
            if (badgeNumber != null)
            {
                notification.BadgeNumber = badgeNumber;
            }

            PendingNotification notificationToDisplay = manager.ScheduleNotification(notification);
            notificationToDisplay.Reschedule = reschedule;

            return (int)notification.Id;
        }

        public static void CancelNotification(int id)
        {
            if (manager == null)
            {
                return;
            }
            manager.CancelNotification(id);
        }

        public static void CancelAllNotifications()
        {
            if (manager == null)
            {
                return;
            }
            manager.CancelAllNotifications();
        }

        public static void OnShutdown()
        {
            if (manager != null)
            {
                manager.LocalNotificationDelivered -= OnDelivered;
                manager.LocalNotificationExpired -= OnExpired;
                manager = null;
            }
        }

        private static void OnExpired(PendingNotification obj)
        {
            WLDebug.Log("OnExpired");
        }

        private static void OnDelivered(PendingNotification obj)
        {
            WLDebug.Log("OnDelivered");
        }
    }
}
