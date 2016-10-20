using System;
using MailKit;
using MailKit.Search;
using MimeKit;
using ResiLab.MailFilter.Infrastructure.Extensions;
using ResiLab.MailFilter.Learning.Model;
using ResiLab.MailFilter.Learning.Parser;

namespace ResiLab.MailFilter.Learning {
    public class MailBoxFolderAnalyzer {
        public static void Analyze(IMailFolder folder, LearningDataSet result) {
            var messages = folder.Search(SearchQuery.All);

            if (messages.Count > 0) {
                foreach (var messageUid in messages) {
                    var message = folder.GetMessage(messageUid);

                    AnalyzeMessage(message, result);
                }
            }

            result.LastUpdate = DateTime.Now;
        }

        private static void AnalyzeMessage(MimeMessage message, LearningDataSet result) {
            result.Adresses.AddOrIgnore(message?.Sender?.Address);
            result.Subjects.AddOrIgnore(message?.Subject);

            var urls = UrlParser.Parse(message?.TextBody);
            foreach (var url in urls) {
                result.Urls.AddOrIgnore(url);
            }
        }
    }
}