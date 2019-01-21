using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Framework.Configuration;
using Microsoft.Owin.Hosting;
using ResiLab.MailFilter.Infrastructure;
using ResiLab.MailFilter.Web;

namespace ResiLab.MailFilter {
    public class MailFilterService {
        private readonly List<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();

        /// <summary>
        ///     Start service.
        /// </summary>
        public void Start() {
            if (!File.Exists("config.json")) {
                Logger.Error("Configuration file \"config.json\" was not found in the application directory!");
                return;
            }

            // configuration
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("config.json").Build();
            configurationBuilder.Bind(ApplicationContext.Instance.Configuration);

            var configuration = ApplicationContext.Instance.Configuration;

            // start webservice
            if (configuration.WebService.Enabled && string.IsNullOrEmpty(configuration.WebService.Url) == false) {
                try {
                    WebApp.Start<WebStartup>(configuration.WebService.Url);

                    Logger.Info($"Started web service on: {configuration.WebService.Url}");
                } catch(Exception ex) {
                    Logger.Error($"Could not start web service on: {configuration.WebService.Url}", ex);
                }
            }

            // do the dirty work
            foreach (var mailBox in configuration.MailBoxes) {
                var cancellationTokenSource = Scheduler.Interval(mailBox.Interval, () => {
                    var mailBoxProcessor = new MailBoxProcessor(mailBox);

                    try {
                        mailBoxProcessor.Process();
                    }
                    catch (Exception ex) {
                        Logger.Error($"Could not process the MailBox {mailBox.Identifier}!", ex);
                    }
                });

                _cancellationTokenSources.Add(cancellationTokenSource);
            }
        }

        /// <summary>
        ///     Stop service.
        /// </summary>
        public void Stop() {
            foreach (var cancellationTokenSource in _cancellationTokenSources) {
                cancellationTokenSource.Cancel();
            }
        }
    }
}