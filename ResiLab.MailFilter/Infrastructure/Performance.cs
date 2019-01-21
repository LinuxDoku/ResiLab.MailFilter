﻿using System.Diagnostics;

namespace ResiLab.MailFilter.Infrastructure {
    public static class Performance {
        private const string Category = "ResiLab.MailFilter";  

        public const string ProcessedMails = nameof(ProcessedMails);
        public const string ProcessedMailsWithRuleMatch = nameof(ProcessedMailsWithRuleMatch);

        public const string ProcessedAnalysisMails = nameof(ProcessedAnalysisMails);
        public const string GeneratedRule = nameof(GeneratedRule);

        static Performance() {
            if (!PerformanceCounterCategory.Exists(Category)) {
                var collection = new CounterCreationDataCollection {
                    new CounterCreationData(
                        ProcessedMails,
                        "Count of processed mails.",
                        PerformanceCounterType.SampleCounter
                    ),
                    new CounterCreationData(
                        ProcessedMailsWithRuleMatch,
                        "Count of processed mails which are then matched by a rule.",
                        PerformanceCounterType.SampleCounter
                    ),
                    new CounterCreationData(
                        ProcessedAnalysisMails,
                        "Count of mails processed by the analysis.",
                        PerformanceCounterType.SampleCounter
                    ),
                    new CounterCreationData(
                        GeneratedRule,
                        "Count of Rules generated by the analysis.",
                        PerformanceCounterType.SampleCounter
                    )
                };

                PerformanceCounterCategory.Create(
                    Category, 
                    "Performance and Activity of the ResiLab MailFilter Service.", 
                    PerformanceCounterCategoryType.SingleInstance, 
                    collection
                );
            } 
        }

        public static void Increment(string counterName) {
            var counter = new PerformanceCounter(Category, counterName, false);

            counter.Increment();
        }
    }
}
