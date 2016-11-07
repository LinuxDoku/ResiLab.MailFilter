using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using ResiLab.MailFilter.Configuration;
using ResiLab.MailFilter.Infrastructure;
using ResiLab.MailFilter.Learning.Model;

namespace ResiLab.MailFilter.Learning {
    public class LearningStorage {
        private readonly MailBox _mailBox;

        public LearningStorage(MailBox mailBox) {
            _mailBox = mailBox;
        }

        public LearningDataSet Read() {
            var fileName = GetHash(_mailBox);

            if (File.Exists(fileName) == false) {
                return new LearningDataSet {
                    Identifier = _mailBox.Identifier
                };
            }

            var json = Cryptography.Decrypt(File.ReadAllText(fileName));
            return JsonConvert.DeserializeObject<LearningDataSet>(json);
        }

        public void Save(LearningDataSet dataSet) {
            var json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            File.WriteAllText(GetHash(_mailBox), Cryptography.Encrypt(json));
        }

        /// <summary>
        /// Create a md5 hash from the passed mail box configuration object's identifier.
        /// </summary>
        /// <param name="mailBox"></param>
        /// <returns></returns>
        protected string GetHash(MailBox mailBox) {
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(mailBox.Identifier)));
        }
    }
}