using System.Linq;
using MimeKit;
using ResiLab.MailFilter.Configuration;

namespace ResiLab.MailFilter.Filter {
    public class RuleMatcher {
        public static bool IsRuleMatching(MimeMessage message, RuleBase rule) {
            var firstSender = message.From.Cast<MailboxAddress>().First();

            var senderName = firstSender.Name ?? string.Empty;
            var fromAddress = firstSender.Address;

            switch (rule.Type) {

                // Sender Address
                case RuleType.SenderEquals:
                    return fromAddress == rule.Value;

                case RuleType.SenderContains:
                    return fromAddress.Contains(rule.Value);

                case RuleType.SenderEndsWith:
                    return fromAddress.EndsWith(rule.Value);

                // Sender Name
                case RuleType.SenderNameEquals:
                    return senderName == rule.Value;

                case RuleType.SenderNameContains:
                    return senderName.Contains(rule.Value);

                case RuleType.SenderNameBeginsWith:
                    return senderName.StartsWith(rule.Value);

                case RuleType.SenderNameEndsWith:
                    return senderName.EndsWith(rule.Value);

                // Subject
                case RuleType.SubjectEquals:
                    return message.Subject == rule.Value;

                case RuleType.SubjectContains:
                    return message.Subject.Contains(rule.Value);

                case RuleType.SubjectBeginsWith:
                    return message.Subject.StartsWith(rule.Value);

                case RuleType.SubjectEndsWith:
                    return message.Subject.EndsWith(rule.Value);
            }

            return false;
        }
    }
}