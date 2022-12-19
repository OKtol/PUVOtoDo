using System.Security.Cryptography;

namespace PUVOtoDo.Models
{
    public static class Encryptor
    {
        public static void EncryptFile(string path)
        {
            string tmpPath = Path.GetTempFileName();
            using (FileStream fsSrc = File.OpenRead(path))
            using (var aes = Aes.Create())
            using (FileStream fsDst = File.Create(tmpPath))
            {
                aes.Key = Secrets.KEY;
                fsDst.Write(aes.IV);
                using (CryptoStream cs = new CryptoStream(fsDst, aes.CreateEncryptor(), CryptoStreamMode.Write, true))
                {
                    fsSrc.CopyTo(cs);
                }
            }
            File.Delete(path);
            File.Move(tmpPath, path);
        }

        public static void DecryptFile(string path)
        {
            string tmpPath = Path.GetTempFileName();
            using (FileStream fsSrc = File.OpenRead(path))
            {
                byte[] iv = new byte[16];
                fsSrc.Read(iv);
                using (var aes = Aes.Create())
                {
                    aes.Key = Secrets.KEY;
                    aes.IV = iv;
                    using (CryptoStream cs = new CryptoStream(fsSrc, aes.CreateDecryptor(), CryptoStreamMode.Read, true))
                    using (FileStream fsDst = File.Create(tmpPath))
                    {
                        cs.CopyTo(fsDst);
                    }
                }
            }
            File.Delete(path);
            File.Move(tmpPath, path);
        }
    }
}
