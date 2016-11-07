using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Framework.Configuration;
using ResiLab.MailFilter.Infrastructure;

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
            var configuration = new Configuration.Configuration();
            configurationBuilder.Bind(configuration);

            // do the dirty work
            foreach (var mailBox in configuration.MailBoxes) {
                var cancellationTokenSource = Scheduler.Interval(mailBox.Interval, () => {
                    try {
                        MailBoxProcessor.Process(mailBox);
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