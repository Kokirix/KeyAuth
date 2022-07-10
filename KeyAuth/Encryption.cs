using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KeyAuth {
    internal class Encryption {
        public static string ToString(byte[] ba) {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public static byte[] StringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        public static byte[] ToBytes(string hex) {
            byte[] bytes = new byte[hex.Length * sizeof(char)];
            System.Buffer.BlockCopy(hex.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public static string Encryptor(string Text, byte[] Key, byte[] IV) {
            Aes AES = Aes.Create();

            AES.Mode = CipherMode.CBC;
            AES.Key = Key;
            AES.IV = IV;

            using (MemoryStream Memory = new MemoryStream()) {
                using (ICryptoTransform Decryptor = AES.CreateEncryptor()) {
                    using (CryptoStream Crpyto = new CryptoStream(Memory, Decryptor, CryptoStreamMode.Write)) {
                        byte[] Bytes = ToBytes(Text);
                        Crpyto.Write(Bytes, 0, Bytes.Length);
                        Crpyto.FlushFinalBlock();
                        byte[] aBytes = Memory.ToArray();
                        return ToString(aBytes);
                    }
                }
            }
        }
        public static string Decryptor(string Message, byte[] Key, byte[] IV) {
            Aes Encryptor = Aes.Create();

            Encryptor.Mode = CipherMode.CBC;
            Encryptor.Key = Key;
            Encryptor.IV = IV;

            using (MemoryStream Memory = new MemoryStream()) {
                using (ICryptoTransform Decryption = Encryptor.CreateDecryptor()) {
                    using (CryptoStream Crypto = new CryptoStream(Memory, Decryption, CryptoStreamMode.Write)) {
                        byte[] CipherBytes = ToBytes(Message);
                        Crypto.Write(CipherBytes, 0, CipherBytes.Length);
                        Crypto.FlushFinalBlock();
                        byte[] pBytes = Memory.ToArray();
                        return Encoding.Default.GetString(pBytes, 0, pBytes.Length);
                    }
                }
            }
        }
        public static string IV() => Guid.NewGuid().ToString().Substring(0, Guid.NewGuid().ToString().IndexOf("-", StringComparison.Ordinal));
        public static string SHA256(string Object) => ToString(new SHA256Managed().ComputeHash(Encoding.Default.GetBytes(Object)));
        public static string Encrypt(string Message, string EncryptionKey, string IV) {
            byte[] key = Encoding.Default.GetBytes(SHA256(EncryptionKey).Substring(0, 32));
            byte[] iv = Encoding.Default.GetBytes(SHA256(IV).Substring(0, 16));
            return Encryptor(Message, key, iv);
        }
        public static string Decrypt(string Message, string EncryptionKey, string IV) {
            byte[] key = Encoding.Default.GetBytes(SHA256(EncryptionKey).Substring(0, 32));
            byte[] iv = Encoding.Default.GetBytes(SHA256(IV).Substring(0, 16));
            return Decryptor(Message, key, iv);
        }
        public static string MD5(string Path) {
            string Result;
            using (MD5 Crypter = System.Security.Cryptography.MD5.Create()) {
                using (FileStream Stream = File.OpenRead(Path)) {
                    byte[] Value = Crypter.ComputeHash(Stream);
                    Result = BitConverter.ToString(Value).Replace("-", "").ToLowerInvariant();
                }
            }
            return Result;
        }
        public static byte[] Get(string Resource) => Encoding.Default.GetBytes(Resource);
    }
}
// Made for KeyAuth | Completed by [Kokiri#8556]