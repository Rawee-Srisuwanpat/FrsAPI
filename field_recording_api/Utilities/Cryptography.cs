using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace field_recording_api.Utilities
{
    public static class Cryptography
    {
        public static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                //rijAlg.FeedbackSize = 128;
                rijAlg.BlockSize = 128;
                rijAlg.KeySize = 256;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                //byte[] plainText2 = null;
                try
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            //csDecrypt.Write(cipherText, 0, cipherText.Length);
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                        //plainText2 = msDecrypt.ToArray();
                    }
                    //string s = System.Text.Encoding.Unicode.GetString(plainText2);
                }
                catch
                {
                    plaintext = "keyError";
                }
            }
            return plaintext;
        }
        public static string DecryptStringAES(string cipherText)
        {
            var salt = HexStringToByteArray(cipherText.Substring(0, 32));
            var keybytes = GenerateKey(cipherText, salt);//Encoding.UTF8.GetBytes("8080808080808080");
            var iv = HexStringToByteArray(cipherText.Substring(32, 32));//Encoding.UTF8.GetBytes("8080808080808080");
            var encrypted = Convert.FromBase64String(cipherText.Substring(64));//Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return decriptedFromJavascript; // return Jsonstring
            //return string.Format(decriptedFromJavascript); return string value
        }
        public static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        public static string Encrypt(string strPlainText)
        {
            //var myRijndael = getCryptoInstance(strPlainText, true);
            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.BlockSize = 128;
            myRijndael.KeySize = 256;
            var salt = generatframent();
            myRijndael.Key = GenerateKey(strPlainText, HexStringToByteArray(salt)); //Encoding.ASCII.GetBytes("b40747c552017bf96360bc498df4be45f26253eea6b06d7f1fb7bb6bf4177044")
            var iv = generatframent();
            myRijndael.IV = HexStringToByteArray(iv);//generatframent(); //Encoding.ASCII.GetBytes("e1f03fdea0e632fbf102ec8f065a3d1e");
            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.Mode = CipherMode.CBC;
            byte[] strText = new System.Text.UTF8Encoding().GetBytes(strPlainText);
            var transform = myRijndael.CreateEncryptor(myRijndael.Key, myRijndael.IV);
            byte[] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);
            var tt = Convert.ToBase64String(cipherText);
            return salt + iv + Convert.ToBase64String(cipherText);// Convert.ToBase64String(cipherText);// BitConverter.ToString(cipherText).Replace("-", "");//Convert.ToBase64String(cipherText);
        }
        public static string Decrypt(string encryptedText)
        {
            var myRijndael = getCryptoInstance(encryptedText);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText.Substring(64));
            var decryptor = myRijndael.CreateDecryptor(myRijndael.Key, myRijndael.IV);
            byte[] originalBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(originalBytes);
        }
        public static RijndaelManaged getCryptoInstance(string strCrypto, bool isencrypt = false)
        {
            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.BlockSize = 128;
            myRijndael.KeySize = 256;
            //var salt = isencrypt ? HexStringToByteArray(generatframent()) : HexStringToByteArray(strCrypto.Substring(0, 32));
            var salt = HexStringToByteArray(strCrypto.Substring(0, 32));
            myRijndael.Key = GenerateKey(strCrypto, salt); //Encoding.ASCII.GetBytes("b40747c552017bf96360bc498df4be45f26253eea6b06d7f1fb7bb6bf4177044")
            myRijndael.IV = HexStringToByteArray(strCrypto.Substring(32, 32));// isencrypt ? HexStringToByteArray(generatframent()) : HexStringToByteArray(strCrypto.Substring(32, 32)); //Encoding.ASCII.GetBytes("e1f03fdea0e632fbf102ec8f065a3d1e");
            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.Mode = CipherMode.CBC;
            return myRijndael;
        }
        public static string generatframent()
        {
            var frament = new byte[16];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(frament);
            }
            return string.Join("", frament.Select(b => b.ToString("x2")));// HexStringToByteArray(string.Join("", frament.Select(b => b.ToString("x2"))));//HexStringToByteArray(string.Join("", frament.Select(b => b.ToString("x2"))));
        }
        public static byte[] HexStringToByteArray(string strHex)
        {
            dynamic r = new byte[strHex.Length / 2];
            for (int i = 0; i <= strHex.Length - 1; i += 2)
            {
                r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
            }
            return r;
        }
        public static byte[] GenerateKey(string strCrypto, byte[] salt)
        {
            var strCry = Regex.Replace(((char)strCrypto.Length).ToString(), @"^[^;](\p{L}\p{Lm})*", ((char)0x00).ToString().TrimEnd('\u0000'));
            var strHash = MD5Hash("" + strCry).ToString(); //Regex.Replace(((char)strCrypto.Length).ToString(), @"[^\p{L}\p{N}]+", ((char)0x00).ToString().TrimEnd('\u0000'))
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes("g_" + strHash), salt, 100);
            return rfc2898.GetBytes(256 / 8);
        }
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            //get hash result after compute it
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
        public static bool IsJson(string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }
    }
}
