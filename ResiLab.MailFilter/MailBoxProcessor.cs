﻿using System;
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
using ResiLab.MailFilter.Configuration;
using ResiLab.MailFilter.Filter;
using ResiLab.MailFilter.Infrastructure;
using ResiLab.MailFilter.Learning;
using ResiLab.MailFilter.Learning.Model;

namespace ResiLab.MailFilter {
    public class MailBoxProcessor {
        private readonly MailBox _mailBox;
        private readonly List<Rule> _rules;
        private LearningDataSet _learningData;

        public MailBoxProcessor(MailBox mailBox) {
            _mailBox = mailBox;
            _rules = new List<Rule>();
        }

        public void Process() {
            using (var client = new ImapClient()) {
                Connect(client);

                var inbox = client.Inbox as ImapFolder;

                // process the analysis
                ProcessAnalysis(inbox);

                // add rules from different sources to this processor
                AddRules();

                // process rules
                ProcessRules(inbox);

                client.Disconnect(true);
            }
        }

        private void Connect(MailService client) {
            client.Connect(_mailBox.Host, _mailBox.Port, _mailBox.UseSsl);
            if (client.AuthenticationMechanisms.Contains("XOAUTH2")) {
                client.AuthenticationMechanisms.Remove("XOAUTH2");
            }
            client.Authenticate(_mailBox.Username, Cryptography.Decrypt(_mailBox.Password));
        }

        private void ProcessAnalysis(IMailFolder inbox) {
            if ((_mailBox.Spam == null) || (_mailBox.Spam.EnableSpamProtection == false)) {
                return;
            }

            _learningData = ReadLearningDataSet();
            var needToUpdateData = _learningData.LastUpdate < DateTime.Now.Subtract(TimeSpan.FromMinutes(_mailBox.Spam.AnalysisInterval));
            if (needToUpdateData) {
                var targetFolder = inbox.GetSubfolder(_mailBox.Spam.Target);
                targetFolder.Open(FolderAccess.ReadOnly);

                MailBoxFolderAnalyzer.Analyze(targetFolder, _learningData);

                SaveLearningDataSet(_learningData);
            }
        }

        private void AddRules() {
            // add user configured rules
            _rules.AddRange(_mailBox.Rules);

            // generate rules based on analysis data
            var ruleGenerator = new RuleGenerator(_learningData, _mailBox.Spam.Target);
            _rules.AddRange(ruleGenerator.Generate());
        }

        private void ProcessRules(IMailFolder inbox) {
            inbox.Open(FolderAccess.ReadWrite);
            inbox.Status(StatusItems.Unread);

            if (inbox.Unread > 0) {
                var unreadMessageUids = inbox.Search(SearchQuery.NotSeen);
                var toMove = new Dictionary<string, List<UniqueId>>();
                var markAsRead = new List<UniqueId>();

                // process unread messages
                foreach (var unreadMessageUid in unreadMessageUids) {
                    var message = inbox.GetMessage(unreadMessageUid);

                    var matchingRule = GetMatchingRule(message);
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

        private Rule GetMatchingRule(MimeMessage message) {
            return _rules.FirstOrDefault(rule => RuleMatcher.IsRuleMatching(message, rule));
        }
        
        private LearningDataSet ReadLearningDataSet() {
            var fileName = GetHash(_mailBox);

            if (File.Exists(fileName) == false) {
                return new LearningDataSet {
                    Identifier = _mailBox.Identifier
                };
            }

            var json = Cryptography.Decrypt(File.ReadAllText(fileName));
            return JsonConvert.DeserializeObject<LearningDataSet>(json);
        }

        private void SaveLearningDataSet(LearningDataSet dataSet) {
            var json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            File.WriteAllText(GetHash(_mailBox), Cryptography.Encrypt(json));
        }

        // TODO move from here
        private static string GetHash(MailBox mailBox) {
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(mailBox.Identifier)));
        }
    }
}