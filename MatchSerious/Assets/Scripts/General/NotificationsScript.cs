using UnityEngine;
using System;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{
    void Start()
    {
        SetupAndroidNotifications();
    }

    void SetupAndroidNotifications()
    {
        // Notification channel (required for Android 8+)
        var channel = new AndroidNotificationChannel()
        {
            Id = "fatigue_channel",
            Name = "Fatigue Alerts",
            Importance = Importance.High,
            Description = "Employee wellness notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        // 1. Break Reminder (Every 90 mins during work hours)
        ScheduleAndroidNotification(
            title: "🧠 Time to Recharge!",
            text: "You've been active for 90 minutes. Take a 10-minute break!",
            fireTime: DateTime.Now.AddHours(1.5),
            repeatInterval: new TimeSpan(1, 30, 0)
        );

        // 2. Daily Check-In (9:00 AM daily)
        ScheduleAndroidNotification(
            title: "📋 Quick Check-In!",
            text: "How are you feeling today? Complete your 1-minute wellness survey.",
            fireTime: NextOccurrence(9, 0),
            repeatInterval: new TimeSpan(24, 0, 0)
        );

        // 3. Wellness Tip (3:00 PM daily)
        ScheduleAndroidNotification(
            title: "💡 Wellness Tip of the Day",
            text: "Pro tip: A 5-minute walk boosts creativity. Try it now!",
            fireTime: NextOccurrence(15, 0),
            repeatInterval: new TimeSpan(24, 0, 0)
        );

        // 4. Fatigue Score Update (5:00 PM daily)
        ScheduleAndroidNotification(
            title: "📊 Your Fatigue Score",
            text: "Your daily fatigue report is ready. Check your trends!",
            fireTime: NextOccurrence(17, 0),
            repeatInterval: new TimeSpan(24, 0, 0)
        );

        // 5. Team Encouragement (Every Friday at 4:00 PM)
        ScheduleAndroidNotification(
            title: "🌟 Team Effort!",
            text: "Your team's fatigue levels improved this week! Keep it up!",
            fireTime: NextFridayAt16(),
            repeatInterval: new TimeSpan(7, 0, 0, 0)
        );
    }

    DateTime NextOccurrence(int hour, int minute)
    {
        DateTime now = DateTime.Now;
        DateTime next = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
        return (now < next) ? next : next.AddDays(1);
    }

    DateTime NextFridayAt16()
    {
        DateTime now = DateTime.Now;
        int daysUntilFriday = ((int)DayOfWeek.Friday - (int)now.DayOfWeek + 7) % 7;
        DateTime nextFriday = now.AddDays(daysUntilFriday).Date + new TimeSpan(16, 0, 0);
        return (nextFriday > now) ? nextFriday : nextFriday.AddDays(7);
    }

    void ScheduleAndroidNotification(string title, string text, DateTime fireTime, TimeSpan repeatInterval)
    {
        var notification = new AndroidNotification
        {
            Title = title,
            Text = text,
            FireTime = fireTime,
            RepeatInterval = repeatInterval
        };
        AndroidNotificationCenter.SendNotification(notification, "fatigue_channel");
    }
}