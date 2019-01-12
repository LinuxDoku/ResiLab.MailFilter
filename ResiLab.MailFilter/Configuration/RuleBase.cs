namespace ResiLab.MailFilter.Configuration {
    /// <summary>
    ///     Base type for new rules, which can be processed by the rule validation.
    /// </summary>
    public abstract class RuleBase {
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
    }
}
