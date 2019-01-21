using System.Collections.Generic;

namespace ResiLab.MailFilter.Configuration {
    public class Configuration {
        /// <summary>
        ///     If logging all rule executions is enabled.
        /// </summary>
        public bool LogEnabled { get; set; } = true;

        /// <summary>
        ///     Mailboxes.
        /// </summary>
        public List<MailBox> MailBoxes { get; set; } = new List<MailBox>();

        /// <summary>
        ///     Configuration of the webservice.
        /// </summary>
        public WebServiceConfiguration WebService { get; set; } = new WebServiceConfiguration();
    }
}