namespace ResiLab.MailFilter.Configuration {
    /// <summary>
    ///     MailBox Rule with action description which is executed when the rules matches.
    /// </summary>
    public class Rule : RuleBase {
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