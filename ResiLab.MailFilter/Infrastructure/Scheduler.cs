using System;
using System.Threading;
using System.Threading.Tasks;

namespace ResiLab.MailFilter.Infrastructure {
    public static class Scheduler {
        public static CancellationTokenSource Interval(TimeSpan interval, Action action) {
            var canellationToken = new CancellationTokenSource();
            Action wrappedAction = null;

            wrappedAction = async () => {
                action();
                await Task.Delay(interval, canellationToken.Token);
                await Task.Run(wrappedAction, canellationToken.Token);
            };

            Task.Run(wrappedAction, canellationToken.Token);

            return canellationToken;
        } 
    }
}