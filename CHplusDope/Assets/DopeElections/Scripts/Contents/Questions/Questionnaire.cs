using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DopeElections.Answer;
using DopeElections.Localizations;
using DopeElections.Progression;
using Localizator;
using Notifications;
using UnityEngine;
using UnityEngine.Playables;

namespace DopeElections.Questions
{
    public class Questionnaire
    {
        private static readonly Regex ImageUrlIdPattern = new Regex("-([0-9]+)\\.(?:svg|png)$");

        public int CantonId { get; }
        public int[] ElectionIds { get; }
        public QuestionCategory[] Categories { get; }
        public Question[] Questions { get; }
        public RaceProgressionTree Progression { get; }

        public Questionnaire(int cantonId, IEnumerable<int> electionIds, IEnumerable<QuestionCategory> categories,
            IEnumerable<Question> questions, RaceProgressionTree progression)
        {
            CantonId = cantonId;
            ElectionIds = electionIds.ToArray();
            Categories = categories.ToArray();
            Questions = questions.ToArray();
            Progression = progression;
        }

        public static Questionnaire Load(Canton canton, Constituency constituency, Election election)
        {
            var result = QuestionnaireSerializer.Load(canton, constituency, election);
            if (result != null)
            {
                UpdateNotifications(result);
            }

            return result;
        }

        private static void UpdateNotifications(Questionnaire questionnaire)
        {
            var assets = DopeElectionsApp.Instance.Assets;
            foreach (var election in questionnaire.ElectionIds
                .Select(id => assets.GetAsset<Election>(id))
                .Where(e => e != null))
            {
                UpdateNotifications(election);
            }
        }

        private static void UpdateNotifications(Election election)
        {
            var reminderTime = Mathf.RoundToInt(3600 * 24 * 2.5f);
            var electionDate = new DateTime(1970, 1, 1).AddSeconds(election.electionTimestamp);
            var reminderDate = electionDate.AddSeconds(-reminderTime);
            // var reminderDate = DateTime.Now.AddSeconds(120); // this is to test notifications
            var currentDate = DateTime.Now;
            if (reminderDate <= currentDate) return;

            var notifications = DopeElectionsApp.Instance.Notifications;
            var pendingNotifications = notifications.PendingNotifications;
            var notificationId = "election-" + election.id;
            var existing = pendingNotifications != null
                ? pendingNotifications.FirstOrDefault(n => n.Notification.Data == notificationId)
                : null;
            IGameNotification notification;
            if (existing != null)
            {
                notification = existing.Notification;
                if (existing.Notification.Id.HasValue)
                {
                    notifications.CancelNotification(existing.Notification.Id.Value);
                }
            }
            else
            {
                notification = notifications.CreateNotification();
                if (notification == null)
                    return; // if no notification is created we're not allowed or it's not possible.
                notification.Data = notificationId;
            }

            var localization = DopeElectionsApp.Instance.Localization ?? new DefaultLocalization();
            var title = localization.GetString(LKey.Notifications.UpcomingElection.Title);
            var text = LocalizationUtility.ApplyReplacements(
                localization.GetString(LKey.Notifications.UpcomingElection.Text), new Dictionary<string, string>
                {
                    ["election"] = election.name,
                    ["year"] = electionDate.Year.ToString(),
                    ["month"] = electionDate.Month.ToString(),
                    ["day"] = electionDate.Day.ToString()
                });

            notification.Title = title;
            notification.Body = text;
            notification.Group = "election-reminders";
            notification.DeliveryTime = reminderDate;
            notifications.ScheduleNotification(notification);
        }
    }
}