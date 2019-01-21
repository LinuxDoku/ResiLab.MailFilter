using Microsoft.Owin;
using Owin;
using ResiLab.MailFilter.Infrastructure;
using ResiLab.MailFilter.Learning;
using System.Linq;
using System.Text;

namespace ResiLab.MailFilter.Web {
    /// <summary>
    /// Startup of the webservice which handles incoming http requests.
    /// </summary>
    internal class WebStartup {
        public void Configuration(IAppBuilder app) {
            app.UseErrorPage();

            app.UseWelcomePage("/");

            app.Use(async (context, next) => {
                if (context.Request.Path.Value == "/statistics") {
                    context.Response.Write(Statistics(context));
                    return;
                }

                await next();
            });
        }

        /// <summary>
        /// Show learning statistics of registered mailboxes.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string Statistics(IOwinContext context) {
            var response = new StringBuilder();            

            foreach (var mailBox in ApplicationContext.Instance.Configuration.MailBoxes) {
                var learningStorage = new LearningStorage(mailBox);
                var dataSet = learningStorage.Read();

                response.AppendLine($"Learning Statistics of MailBox '{mailBox.Identifier}'")
                        .AppendLine($"- Count of Adresses: {dataSet.Adresses.Count}")
                        .AppendLine($"- Count of Sender Names: {dataSet.SenderNames.Count}")
                        .AppendLine($"- Count of Urls: {dataSet.Urls.Count}")
                        .AppendLine($"- Count of Subjects: {dataSet.Subjects.Count}")
                        .AppendLine($"- Count of WhitelistAdresses: {dataSet.Subjects.Count}")
                        .AppendLine();
            }

            return "<pre>" + response.ToString() + "</pre>";
        }
    }
}
