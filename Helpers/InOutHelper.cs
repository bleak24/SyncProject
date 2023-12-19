using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SyncProject.Helpers
{
    public static class InOutHelper
    {
        public static bool FilesAreDifferent(string sourceFilePath, string replicaFilePath)
        {
            bool result = true;
            string hashFile1;
            string hashFile2;

            using (var checksum = MD5.Create())
            {
                using (var stream = File.OpenRead(sourceFilePath))
                {
                    hashFile1 = BitConverter.ToString(checksum.ComputeHash(stream)).Replace("-", "").ToLower();
                }
                using (var stream = File.OpenRead(replicaFilePath))
                {
                    hashFile2 = BitConverter.ToString(checksum.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }

            GC.Collect();

            if (hashFile1 == hashFile2)
            {
                result = false;
            }

            return result;
        }

        public static IEnumerable<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Select(x => x.Replace(path, ""));
        }

        public static IEnumerable<string> GetFiles(string path)
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories).Select(x => x.Replace(path, ""));
        }

        public static void CreateFile(string path)
        {
            File.Create(path);
        }

        public static bool OverwriteFile(string path1, string path2)
        {
            if (FilesAreDifferent(path1, path2))
            {
                File.Delete(path2);
                File.Copy(path1, path2);
                return true; 
            }

            return false;
        }
        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public static void CreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static void DeleteFolder(string path)
        {
            Directory.Delete(path);
        }
    }
}
