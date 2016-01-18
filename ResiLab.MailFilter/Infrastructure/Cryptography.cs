using System;
using System.Security.Cryptography;
using System.Text;

namespace ResiLab.MailFilter.Infrastructure {
    public static class Cryptography {
        public static string Encrypt(string value) {
            return ToBase64String(ProtectedData.Protect(ToBytes(value), null, DataProtectionScope.LocalMachine));
        }

        public static string Decrypt(string valueBase64) {
            return ToString(ProtectedData.Unprotect(ToBytesFromBase64(valueBase64), null, DataProtectionScope.LocalMachine));
        }

        private static string ToString(byte[] bytes) {
            return Encoding.Default.GetString(bytes);
        }

        private static byte[] ToBytes(string str) {
            return Encoding.Default.GetBytes(str);
        }

        private static string ToBase64String(byte[] bytes) {
            return Convert.ToBase64String(bytes);
        }

        private static byte[] ToBytesFromBase64(string str) {
            return Convert.FromBase64String(str);
        }
    }
}