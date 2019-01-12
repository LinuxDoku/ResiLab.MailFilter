using System;
using MailKit;
using MailKit.Search;
using MimeKit;
using ResiLab.MailFilter.Infrastructure.Extensions;
using ResiLab.MailFilter.Learning.Model;
using ResiLab.MailFilter.Learning.Parser;

namespace ResiLab.MailFilter.Learning {
    public class MailBoxFolderAnalyzer {
        /// <summary>
        /// Analyze the mail box folder which contains spam messages.
        /// This generates learning data which is later used to create rules for new inbox messages.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="result"></param>
        public static void Analyze(IMailFolder folder, LearningDataSet result) {
            ProcessMessagesInFolder(folder, message => {
                result.Adresses.AddOrIgnore(message?.Sender?.Address);
                result.SenderNames.AddOrIgnore(message?.Sender?.Name);
                result.Subjects.AddOrIgnore(message?.Subject);

                var urls = UrlParser.Parse(message?.TextBody);
                foreach (var url in urls) {
                    result.Urls.AddOrIgnore(url);
                }
            });

            result.LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Analyze the mail box folder which contains messages which are classified as no spam.
        /// This is later used to decide if a rule should be generated based on learned data from spam messages.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="result"></param>
        public static void AnalyzeWhitelist(IMailFolder folder, LearningDataSet result) {
            ProcessMessagesInFolder(folder, message => {
                result.WhitelistAdresses.AddOrIgnore(message?.Sender?.Address);
            });

            result.LastUpdate = DateTime.Now;
        }

        private static void ProcessMessagesInFolder(IMailFolder folder, Action<MimeMessage> processMessage) {
            var messages = folder.Search(SearchQuery.All);

            if (messages.Count > 0) {
                foreach (var messageUid in messages) {
                    var message = folder.GetMessage(messageUid);

                    processMessage(message);
                }
            }
        }
    }
}