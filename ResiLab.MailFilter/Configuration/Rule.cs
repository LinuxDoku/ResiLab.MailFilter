namespace ResiLab.MailFilter.Configuration {
    public class Rule {
        /// <summary>
        ///     Name of the rule.
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        ///     What rule logic shoudl be used.
        /// </summary>
        public RuleType Type { get; set; }

        /// <summary>
        ///     Parameter for the rule.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Where the message should be moved when the rule affects the message.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        ///     Mark the message as read.
        /// </summary>
        public bool MarkAsRead { get; set; } = true;
    }
}