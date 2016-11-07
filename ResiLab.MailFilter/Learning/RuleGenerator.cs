using System;
using System.Collections.Generic;
using ResiLab.MailFilter.Configuration;
using ResiLab.MailFilter.Learning.Model;

namespace ResiLab.MailFilter.Learning {
    public class RuleGenerator {
        private readonly LearningDataSet _learningDataSet;
        private readonly string _targetFolder;

        public RuleGenerator(LearningDataSet learningDataSet, string targetFolder) {
            _learningDataSet = learningDataSet;
            _targetFolder = targetFolder;
        }

        /// <summary>
        /// Generate rules based on input learning data.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Rule> Generate() {
            var rules = new List<Rule>();

            // reject email from every address listed in the learnign data
            foreach (var address in _learningDataSet.Adresses) {
                rules.Add(GenerateAddressRule(address));
            }

            // reject mails with this subject
            foreach (var subject in _learningDataSet.Subjects) {
                rules.Add(GenerateSubjectRule(subject));
            }

            // reject mails containing these links
            // TODO: think about whitelisting content of mailbox, social media links could cause problems

            return rules;
        }

        private Rule GenerateAddressRule(string address) {
            return GenerateRule(RuleType.SenderEquals, address);
        }

        private Rule GenerateSubjectRule(string subject) {
            return GenerateRule(RuleType.SubjectEquals, subject);
        }
        
        private Rule GenerateRule(RuleType type, string value) {
            return new Rule {
                Destination = _targetFolder,
                MarkAsRead = false,
                RuleName = "Learning-" + Guid.NewGuid(),
                Type = type,
                Value = value
            };
        }
    }
}