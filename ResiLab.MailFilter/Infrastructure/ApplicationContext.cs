namespace ResiLab.MailFilter.Infrastructure {
    internal class ApplicationContext {
        private static ApplicationContext _instance;

        public static ApplicationContext Instance => _instance ?? (_instance = new ApplicationContext());

        public Configuration.Configuration Configuration { get; } = new Configuration.Configuration();
    }
}
