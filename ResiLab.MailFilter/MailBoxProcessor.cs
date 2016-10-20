using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using Newtonsoft.Json;
using ResiLab.MailFilter.Infrastructure;
using ResiLab.MailFilter.Learning;
using ResiLab.MailFilter.Learning.Model;
using ResiLab.MailFilter.Model;

namespace ResiLab.MailFilter {
    public class MailBoxProcessor {
        public static void Process(MailBox mailBox) {
            using (var client = new ImapClient()) {
                client.Connect(mailBox.Host, mailBox.Port, mailBox.UseSsl);
                if (client.AuthenticationMechanisms.Contains("XOAUTH2")) {
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                }
                client.Authenticate(mailBox.Username, Cryptography.Decrypt(mailBox.Password));

                var inbox = client.Inbox as ImapFolder;

                // process the analysis
                ProcessAnalysis(mailBox, inbox);

                // process rules
                ProcessRules(mailBox, inbox);

                client.Disconnect(true);
            }
        }

        private static void ProcessAnalysis(MailBox mailBox, ImapFolder inbox) {
            if ((mailBox.Spam == null) || (mailBox.Spam.EnableSpamProtection == false)) {
                return;
            }

            var dataSet = ReadLearningDataSet(mailBox);
            if (dataSet.LastUpdate >= DateTime.Now.Subtract(TimeSpan.FromMinutes(mailBox.Spam.AnalysisInterval))) {
                return;
            }

            var targetFolder = inbox.GetSubfolder(mailBox.Spam.Target);
            targetFolder.Open(FolderAccess.ReadOnly);

            MailBoxFolderAnalyzer.Analyze(targetFolder, dataSet);

            SaveLearningDataSet(mailBox, dataSet);
        }

        private static void ProcessRules(MailBox mailBox, ImapFolder inbox) {
            inbox.Open(FolderAccess.ReadWrite);
            inbox.Status(StatusItems.Unread);

            if (inbox.Unread > 0) {
                var unreadMessageUids = inbox.Search(SearchQuery.NotSeen);
                var toMove = new Dictionary<string, List<UniqueId>>();
                var markAsRead = new List<UniqueId>();

                // process unread messages
                foreach (var unreadMessageUid in unreadMessageUids) {
                    var message = inbox.GetMessage(unreadMessageUid);

                    var matchingRule = GetMatchingRule(message, mailBox);
                    if (matchingRule != null) {
                        if (!toMove.ContainsKey(matchingRule.Destination)) {
                            toMove.Add(matchingRule.Destination, new List<UniqueId>());
                        }

                        toMove[matchingRule.Destination].Add(unreadMessageUid);

                        if (matchingRule.MarkAsRead) {
                            markAsRead.Add(unreadMessageUid);
                        }
                    }
                }

                // mark as read
                if (markAsRead.Any()) {
                    inbox.AddFlags(markAsRead, MessageFlags.Seen, true);
                }

                // move to destination
                if (toMove.Any()) {
                    foreach (var destination in toMove.Keys) {
                        inbox.MoveTo(toMove[destination], inbox.GetSubfolder(destination));
                    }
                }
            }
        }

        private static Rule GetMatchingRule(MimeMessage message, MailBox mailBox) {
            return mailBox.Rules.FirstOrDefault(rule => IsRuleMatching(message, rule));
        }

        private static bool IsRuleMatching(MimeMessage message, Rule rule) {
            var fromAddress = message.From.Cast<MailboxAddress>().First().Address;

            switch (rule.Type) {
                case RuleType.SenderEquals:
                    return fromAddress == rule.Value;

                case RuleType.SenderContains:
                    return fromAddress.Contains(rule.Value);

                case RuleType.SenderEndsWith:
                    return fromAddress.EndsWith(rule.Value);

                case RuleType.SubjectContains:
                    return message.Subject.Contains(rule.Value);

                case RuleType.SubjectEquals:
                    return message.Subject == rule.Value;

                case RuleType.SubjectBeginsWith:
                    return message.Subject.StartsWith(rule.Value);

                case RuleType.SubjectEndsWith:
                    return message.Subject.EndsWith(rule.Value);
            }

            return false;
        }

        private static LearningDataSet ReadLearningDataSet(MailBox mailBox) {
            var fileName = GetHash(mailBox);

            if (File.Exists(fileName) == false) {
                return new LearningDataSet {
                    Identifier = mailBox.Identifier
                };
            }

            var json = Cryptography.Decrypt(File.ReadAllText(fileName));
            return JsonConvert.DeserializeObject<LearningDataSet>(json);
        }

        private static void SaveLearningDataSet(MailBox mailBox, LearningDataSet dataSet) {
            var json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            File.WriteAllText(GetHash(mailBox), Cryptography.Encrypt(json));
        }

        private static string GetHash(MailBox mailBox) {
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(mailBox.Identifier)));
        }
    }
}