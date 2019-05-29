using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace crypt
{
    public class Cryptography
    {
        /// <summary>
        /// Encrypts a given string
        /// <param name="value">A string to encrypt.</param>
        /// </summary>
        /// <returns><see cref="string"/>Encrypted string.</returns>
        public string Encrypt(string value)
        {
            Random rand = new Random();
            char[] stringToEnc = value.ToCharArray();
            int index = 0;
            string encString = "";
            foreach (char c in stringToEnc)
            {
                stringToEnc[index] += (char)index;
                encString += stringToEnc[index];
                encString += (char)rand.Next(32, 128);
                index++;
            }
            return (encString);
        }

        /// <summary>
        /// Decrypts a given string
        /// <param name="value">A string to decrypt.</param>
        /// </summary>
        /// <returns><see cref="string"/>Decrypted string.</returns>
        public string Decrypt(string value)
        {
            List<Char> allTheStuff = new List<Char>();
            int index = 0, i = 0;
            string decString = "";

            foreach (char c in value)
            {
                if ((i % 2) == 0)
                {
                    allTheStuff.Add(c);
                }
                i++;
            }
            for (index = 0; index < allTheStuff.Count; index++)
            {
                char tempStorage = allTheStuff[index];
                allTheStuff[index] = (char)(tempStorage - index);
                decString += allTheStuff[index];
            }
            return (decString);
        }
    }
}