using System.Linq;
using MimeKit;
using ResiLab.MailFilter.Configuration;

namespace ResiLab.MailFilter.Filter {
    public class RuleMatcher {
        public static bool IsRuleMatching(MimeMessage message, Rule rule) {
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
    }
}