using System;
using System.Collections.Generic;
using System.Linq;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Framework.Configuration;
using MimeKit;
using ResiLab.MailFilter.Infrastructure;
using ResiLab.MailFilter.Model;

namespace ResiLab.MailFilter {
    public class Program {
        public static void Main(string[] args) {
            Logger.Setup();
    
            try {
                if (args.Length > 0) {
                    if (args.First() == "crypt") {
                        var toCrypt = string.Join(" ", args.Skip(1));
                        Console.WriteLine("Crypted:");
                        Console.WriteLine(Cryptography.Encrypt(toCrypt));
                        return;
                    }
                }

                // configuration
                var configurationBuilder = new ConfigurationBuilder().AddJsonFile("config.json").Build();
                var configuration = new Configuration();
                configurationBuilder.Bind(configuration);

                // do the dirty work
                foreach (var mailBox in configuration.MailBoxes) {
                    Process(mailBox);
                }
            } catch (Exception ex) {
                Logger.Error("An error occurred!", ex);
            }
        }

        private static void Process(MailBox mailBox) {
            using (var client = new ImapClient()) {
                client.Connect(mailBox.Host, mailBox.Port, mailBox.UseSsl);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(mailBox.Username, Cryptography.Decrypt(mailBox.Password));

                var inbox = client.Inbox as ImapFolder;
                inbox.Open(FolderAccess.ReadWrite);
                inbox.Status(StatusItems.Unread);

                if (inbox.Unread > 0) {
                    var unreadMessageUids = inbox.Search(SearchQuery.NotSeen);
                    var toMove = new Dictionary<string, List<UniqueId>>();
                    var markAsRead = new List<UniqueId>();

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

                    if (toMove.Any()) {
                        // TODO: logging
                        foreach (var destination in toMove.Keys) {
                            inbox.MoveTo(toMove[destination], inbox.GetSubfolder(destination));
                        }

                        inbox.AddFlags(markAsRead, MessageFlags.Seen, true);
                    }
                }

                client.Disconnect(true);
            }
        }

        private static Rule GetMatchingRule(MimeMessage message, MailBox mailBox) {
            return mailBox.Rules.FirstOrDefault(rule => IsRuleMatching(message, rule));
        }

        private static bool IsRuleMatching(MimeMessage message, Rule rule) {
            switch (rule.Type) {
                case RuleType.SenderEquals:
                    return message.Sender.Address == rule.Value;

                case RuleType.SenderContains:
                    return message.Sender.Address.Contains(rule.Value);

                case RuleType.SenderEndsWith:
                    return message.Sender.Address.EndsWith(rule.Value);

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
    }
}
