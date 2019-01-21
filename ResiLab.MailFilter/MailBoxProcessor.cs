using System;
using System.Collections.Generic;
using System.Linq;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using ResiLab.MailFilter.Configuration;
using ResiLab.MailFilter.Filter;
using ResiLab.MailFilter.Infrastructure;
using ResiLab.MailFilter.Learning;
using ResiLab.MailFilter.Learning.Model;

namespace ResiLab.MailFilter {
    public class MailBoxProcessor {
        private readonly MailBox _mailBox;
        private readonly LearningStorage _learningStorage;
        private readonly List<Rule> _rules;
        private LearningDataSet _learningData;
        
        public MailBoxProcessor(MailBox mailBox) {
            _mailBox = mailBox;
            _learningStorage = new LearningStorage(mailBox);
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

            // load already cached learning data
            _learningData = _learningStorage.Read();

            var needToUpdateData = _learningData.LastUpdate < DateTime.Now.Subtract(TimeSpan.FromMinutes(_mailBox.Spam.AnalysisInterval));
            if (needToUpdateData) {
                // analyze classified spam
                var spamTargetFolder = inbox.GetSubfolder(_mailBox.Spam.Target);
                spamTargetFolder.Open(FolderAccess.ReadOnly);

                MailBoxFolderAnalyzer.Analyze(spamTargetFolder, _learningData);
                
                // collect data from whitelist
                if (_mailBox.Spam.Whitelist?.Folder != null) {
                    var whiteListFolder = inbox.GetSubfolder(_mailBox.Spam.Whitelist.Folder);
                    whiteListFolder.Open(FolderAccess.ReadOnly);

                    MailBoxFolderAnalyzer.AnalyzeWhitelist(whiteListFolder, _learningData);
                }

                _learningStorage.Save(_learningData);
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

                    Performance.Increment(Performance.ProcessedMails);

                    var matchingRule = GetMatchingRule(message);
                    if (matchingRule != null) {
                        Performance.Increment(Performance.ProcessedMailsWithRuleMatch);

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
            Rule resultRule;

            // use rules configured by the user first
            resultRule = _rules.FirstOrDefault(
                rule => rule is GeneratedRule == false && RuleMatcher.IsRuleMatching(message, rule)
            );
            
            // try to find a generated rule when the message is not white listed
            if (resultRule == null && IsMessageWhitelisted(message) == false) {
                resultRule = _rules.FirstOrDefault(
                    rule => rule is GeneratedRule && RuleMatcher.IsRuleMatching(message, rule)
                );
            }

            return resultRule;
        }

        private bool IsMessageWhitelisted(MimeMessage message) {
            // messages without sender can not be whitelisted
            if (message.Sender?.Address == null) {
                return false;
            }

            return _learningData.WhitelistAdresses?.Contains(message.Sender.Address) ?? false;
        }
    }
}