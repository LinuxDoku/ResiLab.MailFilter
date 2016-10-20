namespace ResiLab.MailFilter.Model {
    public class Spam {
        /// <summary>
        ///     Enable the spam protection mechanism.
        /// </summary>
        public bool EnableSpamProtection { get; set; } = false;

        /// <summary>
        ///     Interval, in which the spam mails are analyzed.
        /// </summary>
        public int AnalysisInterval { get; set; } = 60;

        /// <summary>
        ///     Target folder for all regocnized spam.
        /// </summary>
        public string Target { get; set; } = "Spam";
    }
}