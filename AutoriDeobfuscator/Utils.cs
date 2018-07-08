using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace AutoriDeobfuscator
{
    class Utils
    {
        public static void RemoveCall(AssemblyDef asm, MethodDef method)
        {
            var staticConstructor = asm.ManifestModule.GlobalType.FindOrCreateStaticConstructor();
            var instr = staticConstructor.Body.Instructions;
            for (int i = 0; i < instr.Count; i++)
            {
                if (instr[i].OpCode == OpCodes.Call &&
                   instr[i].Operand as MethodDef == method)

                    instr.RemoveAt(i);
            }
        }

        /// <summary>
        /// This method is used for StringEncryption class
        /// </summary>
        /// <returns>decrypted string</returns>
        public static string DecryptXorString(string text, string key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                uint num = (uint)c;
                int index = i % key.Length;
                char c2 = key[index];
                uint num2 = (uint)c2;
                uint num3 = num ^ num2;
                char value = (char)num3;
                stringBuilder.Append(value);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// This method is used for StringEncryption class
        /// </summary>
        /// <returns>decrypted string</returns>
        public static string DecryptRijndaelManagedString(string s)
        {
            string password = "75rdRnXOJf8gG1ra";
            string s2 = "DWYCW8G7Rh5ITTJO";
            string s3 = "9PWrQPWzds7wKIA4";
            byte[] array = Convert.FromBase64String(s);
            byte[] bytes = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(s2)).GetBytes(32);
            RijndaelManaged rijndaelManaged = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };
            ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, Encoding.ASCII.GetBytes(s3));
            MemoryStream memoryStream = new MemoryStream(array);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
            byte[] array2 = new byte[array.Length];
            int count = cryptoStream.Read(array2, 0, array2.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(array2, 0, count).TrimEnd("\0".ToCharArray());
        }
    }
}
