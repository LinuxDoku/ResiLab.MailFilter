namespace ResiLab.MailFilter.Configuration {
    public class WebServiceConfiguration {
        /// <summary>
        /// Whether the web service should be started or not.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Host address of the web service.
        /// </summary>
        public string Url { get; set; }
    }
}
