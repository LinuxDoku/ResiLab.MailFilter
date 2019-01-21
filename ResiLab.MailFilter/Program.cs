using System;
using System.Linq;
using ResiLab.MailFilter.Infrastructure;
using Topshelf;

namespace ResiLab.MailFilter {
    public class Program {
        public static void Main(string[] args) {
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, eventArgs) => {
                    Logger.Error("Unhandled exception occurred!", eventArgs.ExceptionObject as Exception);
                };

            // cli
            if (args.Length > 0) {
                if (args.First() == "crypt") {
                    var toCrypt = string.Join(" ", args.Skip(1));
                    Console.WriteLine("Crypted:");
                    Console.WriteLine(Cryptography.Encrypt(toCrypt));
                    return;
                }
            }

            // service host
            HostFactory.Run(host => {
                host.Service<MailFilterService>(setup => {
                    setup.ConstructUsing(name => new MailFilterService());

                    setup.WhenStarted(service => service.Start());
                    setup.WhenStopped(service => service.Stop());
                });

                host.SetServiceName("ResiLabMailFilter");
                host.SetDisplayName("ResiLab Mail Filter");
                host.SetDescription("Filter MailBoxes with Rules.");

                host.RunAsLocalSystem();
                host.StartAutomatically();

                host.EnableServiceRecovery(recovery => {
                    recovery.OnCrashOnly();
                    recovery.RestartService(1);
                });
            });
        }
    }
}