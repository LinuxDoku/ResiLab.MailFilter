using System;
using System.Linq;
using Microsoft.Framework.Configuration;
using ResiLab.MailFilter.Infrastructure;
using ResiLab.MailFilter.Model;

namespace ResiLab.MailFilter {
    public class Program {
        public static void Main(string[] args) {
            Logger.Setup();
    
            try {
                if (args.Length > 0) {
                    if (args.First() == "crypt") {
                        var toCrypt = string.Join(" ", args.Skip(1));
                        Console.WriteLine("Crypted:");
                        Console.WriteLine(Cryptography.Encrypt(toCrypt));
                        return;
                    }
                }

                // configuration
                var configurationBuilder = new ConfigurationBuilder().AddJsonFile("config.json").Build();
                var configuration = new Configuration();
                configurationBuilder.Bind(configuration);

                // do the dirty work
                foreach (var mailBox in configuration.MailBoxes) {
                    MailBoxProcessor.Process(mailBox);
                }
            } catch (Exception ex) {
                Logger.Error("An error occurred!", ex);
            }
        }
    }
}
