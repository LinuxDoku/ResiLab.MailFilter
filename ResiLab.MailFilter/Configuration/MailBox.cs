using System;
using System.Collections.Generic;

namespace ResiLab.MailFilter.Configuration {
    public class MailBox {
        private string _identifier;
        private TimeSpan _interval = TimeSpan.FromSeconds(30);

        /// <summary>
        ///     Identifier of the mailbox.
        ///     Defaults to Username@Host:Port
        /// </summary>
        public string Identifier {
            get {
                if (_identifier != null) {
                    return _identifier;
                }

                return Username + "@" + Host + ":" + Port;
            }
            set { _identifier = value; }
        }

        /// <summary>
        ///     (Protocol) Type of the mail box.
        /// </summary>
        public MailBoxServer Type { get; set; }

        /// <summary>
        ///     Hostname.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Port
        /// </summary>
        public int Port { get; set; } = 993;

        /// <summary>
        ///     Use ssl to connect to the server.
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        ///     Username of the mailbox.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Password to access the mailbox.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Interval to execute the rules.
        /// </summary>
        public TimeSpan Interval {
            get {
                // protection before flooding the mail server by wrong or missing config
                if (_interval >= TimeSpan.FromSeconds(5)) {
                    return _interval;
                }

                return TimeSpan.FromSeconds(30); // default value
            }
            set { _interval = value; }
        }

        /// <summary>
        ///     Rules to apply on this mailbox.
        /// </summary>
        public List<Rule> Rules { get; set; } = new List<Rule>();

        /// <summary>
        ///     Spam Filter configuration.
        /// </summary>
        public Spam Spam { get; set; } = new Spam();
    }
}