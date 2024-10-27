using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SaveSystem.Runtime.Encryption
{
    /// <summary>
    /// AES is a symmetric 256-bit encryption algorithm.
    /// Read more: http://en.wikipedia.org/wiki/Advanced_Encryption_Standard
    /// </summary>
    public static class Aes
    {
        /// <summary>
        /// Salt value used along with password for generating password hash.
        /// Must be at least 8 characters long.
        /// </summary>
        private const string SALT = "g46dzQ80";
        
        /// <summary>
        /// Initialization vector used to initialize the AES algorithm.
        /// Must be 16 ASCII characters long.
        /// </summary>
        private const string INIT_VECTOR = "OFRna74m*aze01xY";

        private static readonly byte[] _saltBytes;
        private static readonly byte[] _initVectorBytes;

        static Aes()
        {
            _saltBytes = Encoding.UTF8.GetBytes(SALT);
            _initVectorBytes = Encoding.UTF8.GetBytes(INIT_VECTOR);
        }


        /// <summary>
        /// Encrypts a string with AES
        /// </summary>
        /// <param name="plainText">Text to be encrypted</param>
        /// <param name="password">Password to encrypt with</param>   
        /// <param name="salt">Salt to encrypt with</param>    
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>    
        /// <returns>An encrypted string</returns>        
        public static string Encrypt(string plainText, string password, string salt = null, string initialVector = null) =>
            Convert.ToBase64String(EncryptToBytes(plainText, password, salt, initialVector));

        /// <summary>
        /// Encrypts a string with AES
        /// </summary>
        /// <param name="plainText">Text to be encrypted</param>
        /// <param name="password">Password to encrypt with</param>   
        /// <param name="salt">Salt to encrypt with</param>    
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>    
        /// <returns>An encrypted string</returns>        
        public static byte[] EncryptToBytes(string plainText, string password, string salt = null, string initialVector = null)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return EncryptToBytes(plainTextBytes, password, salt, initialVector);
        }

        /// <summary>
        /// Encrypts a string with AES
        /// </summary>
        /// <param name="plainTextBytes">Bytes to be encrypted</param>
        /// <param name="password">Password to encrypt with</param>   
        /// <param name="salt">Salt to encrypt with</param>    
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>    
        /// <returns>An encrypted string</returns>        
        public static byte[] EncryptToBytes(byte[] plainTextBytes, string password, string salt = null, string initialVector = null)
        {
            const int keySize = 256;

            var initialVectorBytes = string.IsNullOrEmpty(initialVector) ? _initVectorBytes : Encoding.UTF8.GetBytes(initialVector);
            var saltValueBytes = string.IsNullOrEmpty(salt) ? _saltBytes : Encoding.UTF8.GetBytes(salt);
            var keyBytes = new Rfc2898DeriveBytes(password, saltValueBytes).GetBytes(keySize / 8);

            using var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            
            using var encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes);
            using var memStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();

            return memStream.ToArray();
        }

        /// <summary>  
        /// Decrypts an AES-encrypted string. 
        /// </summary>  
        /// <param name="cipherText">Text to be decrypted</param> 
        /// <param name="password">Password to decrypt with</param> 
        /// <param name="salt">Salt to decrypt with</param> 
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param> 
        /// <returns>A decrypted string</returns>
        public static string Decrypt(string cipherText, string password, string salt = null, string initialVector = null)
        {
            var cipherTextBytes = Convert.FromBase64String(cipherText.Replace(' ','+'));
            return Decrypt(cipherTextBytes, password, salt, initialVector).TrimEnd('\0');
        }

        /// <summary>  
        /// Decrypts an AES-encrypted string. 
        /// </summary>  
        /// <param name="cipherTextBytes">Text to be decrypted</param> 
        /// <param name="password">Password to decrypt with</param> 
        /// <param name="salt">Salt to decrypt with</param> 
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param> 
        /// <returns>A decrypted string</returns>
        public static string Decrypt(byte[] cipherTextBytes, string password, string salt = null, string initialVector = null)
        {
            const int keySize = 256;

            var initialVectorBytes = string.IsNullOrEmpty(initialVector) ? _initVectorBytes : Encoding.UTF8.GetBytes(initialVector);
            var saltValueBytes = string.IsNullOrEmpty(salt) ? _saltBytes : Encoding.UTF8.GetBytes(salt);
            var keyBytes = new Rfc2898DeriveBytes(password, saltValueBytes).GetBytes(keySize / 8);
            var plainTextBytes = new byte[cipherTextBytes.Length];

            using var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;

            using var decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes);
            using var memStream = new MemoryStream(cipherTextBytes);
            using var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
            var byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }
    }
}