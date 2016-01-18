using System;
using System.Collections.Generic;
using MailKit.Security;

namespace ResiLab.MailFilter.Model {
    public class MailBox {
        /// <summary>
        /// (Protocol) Type of the mail box.
        /// </summary>
        public MailBoxServer Type { get; set; }

        /// <summary>
        /// Hostname.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; } = 993;

        /// <summary>
        /// Use ssl to connect to the server.
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// Username of the mailbox.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password to access the mailbox.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Interval to execute the rules.
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Rules to apply on this mailbox.
        /// </summary>
        public List<Rule> Rules { get; set; } = new List<Rule>();
    }
}