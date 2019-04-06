using System;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Security.Cryptography;
using DapperRepository.Core.Domain.Security;

namespace DapperRepository.Core
{
    public class CommonHelper
    {
        public static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }

            //not hosted. For example, run in unit tests
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        public static byte[] GetHash(string inputString, HashSignType hashSignType = HashSignType.SHA1)
        {
            HashAlgorithm algorithm = null;

            switch (hashSignType)
            {
                case HashSignType.SHA1:
                    algorithm = SHA1.Create();
                    break;
                case HashSignType.SHA256:
                    algorithm = SHA256.Create();
                    break;
                case HashSignType.SHA384:
                    algorithm = SHA384.Create();
                    break;
                case HashSignType.SHA512:
                    algorithm = SHA512.Create();
                    break;
            }

            if (algorithm != null)
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            return new byte[0];
        }

        public static string GetHashString(string inputString, HashSignType hashSignType = HashSignType.SHA1)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
