using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JxCode.Common
{
    public class AESHelper
    {
        private readonly static byte[] NORMAL_AES_IV = Encoding.UTF8.GetBytes("0000000000000000");
        //16位
        private readonly static byte[] NORMAL_AES_KEY = Encoding.UTF8.GetBytes("__jxaes1024key__");

        private static byte[] CutBuffer(byte[] key, int length)
        {
            if (key.Length == length) return key;
            byte[] buf = new byte[length];
            if (key.Length < length)
            {
                Array.Copy(key, buf, key.Length);
            }
            else
            {
                Array.Copy(key, buf, length);
            }
            return buf;
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="m">明文</param>
        /// <param name="key">密钥</param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] m, byte[] key = null, byte[] IV = null)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = key != null ? CutBuffer(key, 16) : CutBuffer(NORMAL_AES_KEY, 16);
                aes.IV = IV != null ? IV : NORMAL_AES_IV;
                aes.Padding = PaddingMode.PKCS7;
                var t = aes.CreateEncryptor();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, t, CryptoStreamMode.Write))
                    {
                        cs.Write(m, 0, m.Length);
                        cs.FlushFinalBlock();
                        var result = ms.ToArray();
                        return result;
                    }
                }

            }
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="p">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] p, byte[] key = null, byte[] IV = null)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = key != null ? CutBuffer(key, 16) : CutBuffer(NORMAL_AES_KEY, 16);
                aes.IV = IV != null ? IV : NORMAL_AES_IV;
                aes.Padding = PaddingMode.PKCS7;
                var t = aes.CreateDecryptor();
                using (MemoryStream ms = new MemoryStream(p))
                {
                    using (CryptoStream cs = new CryptoStream(ms, t, CryptoStreamMode.Read))
                    {
                        byte[] m = new byte[p.Length];
                        int len = cs.Read(m, 0, m.Length);

                        byte[] rtn = new byte[len];
                        Array.Copy(m, rtn, len);

                        return rtn;
                    }
                }
            }
        }

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptString(string content, string key = null)
        {
            byte[] _key = key == null ? null : Encoding.UTF8.GetBytes(key);

            byte[] str = Encoding.UTF8.GetBytes(content);
            byte[] p = Encrypt(str, _key);
            string rtn = Convert.ToBase64String(p);
            return rtn;
        }

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptString(string content, string key = null)
        {
            byte[] _key = key == null ? null : Encoding.UTF8.GetBytes(key);

            byte[] str = Convert.FromBase64String(content);
            byte[] m = Decrypt(str, _key);
            return Encoding.UTF8.GetString(m);
        }
    }
}
