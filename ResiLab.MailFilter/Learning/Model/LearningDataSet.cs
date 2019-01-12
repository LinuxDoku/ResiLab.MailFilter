using System;
using System.Collections.Generic;

namespace ResiLab.MailFilter.Learning.Model {
    /// <summary>
    ///     Learning data set contains the saved state of a learning process.
    /// </summary>
    public class LearningDataSet {
        /// <summary>
        ///     Identifier of this learning data set.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        ///     Last update of this learning data set.
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        ///     Adresses which are found in the learning data.
        /// </summary>
        public List<string> Adresses { get; set; } = new List<string>();

        /// <summary>
        ///     Sender names which are found in the learning data.
        /// </summary>
        public List<string> SenderNames { get; set; } = new List<string>();

        /// <summary>
        ///     Urls found in the messages of the learning data.
        /// </summary>
        public List<string> Urls { get; set; } = new List<string>();

        /// <summary>
        ///     Subjects found in the learning data.
        /// </summary>
        public List<string> Subjects { get; set; } = new List<string>();

        /// <summary>
        ///     Whitelist of adresses which should not be handled as spam.
        /// </summary>
        public List<string> WhitelistAdresses { get; set; } = new List<string>();
    }
}