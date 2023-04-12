using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ZNix.SuperBLT
{
    public static class Hasher
    {
        /**
         * Hash a directory into a hashstring, as per the BLT hash system.
         *
         * @param directory The directory to hash
         * @return The string-encoded hash, which should exactly match that of BLT's hasher.
         * @throws IOException If there was an exception reading a file.
         */
        public static string HashDirectory(string directory)
        {
            StringBuilder contents = new StringBuilder();
            SubHashDirectory(directory, contents);
            return Sha256Unicode(contents.ToString());
        }

        /**
         * Hash a file into a hashstring, as per the BLT hash system.
         *
         * @param file The file to hash
         * @return The string-encoded hash, which should exactly match that of BLT's hasher.
         * @throws IOException If there was an exception reading a file.
         */
        public static string HashFile(string file)
        {
            return Sha256Unicode(Sha256(file));
        }

        /**
         * Recursively hashes all the files in a directory, putting the relative
         * paths and hashes into a map.
         *
         * @param base   The path relative to which all output keys will appear
         * @param search The directory to iterate over
         * @param output The output map
         * @throws IOException If there was a problem hashing a file
         */
        private static void hashDirectoryContents(string basef, string search, Dictionary<string, string> output)
        {
            foreach (string file in Directory.GetFiles(search))
            {
                string hash = Sha256(file);
                string relative = file.Replace(basef, "");
                output[relative] = hash;
            }

            foreach (string file in Directory.GetDirectories(search))
            {
                hashDirectoryContents(basef, file, output);
            }
        }

        /**
         * Iterate through the contents of a directory in alphabetical order,
         * concatenating the hashes of the files. This method is recursive.
         *
         * @param directory The directory to iterate over
         * @return The concatenated hashes of the contents of this directory. Note
         * that this is <b>not</b> a hash itself.
         */
        private static void SubHashDirectory(string directory, StringBuilder result)
        {
            directory = directory.Replace(Path.DirectorySeparatorChar, '/');
            string basef = directory;
            if (!basef.EndsWith("/"))
            {
                basef += "/";
            }
            
            Dictionary<string, string> output = new Dictionary<string, string>();
            hashDirectoryContents(basef, directory, output);

            List<string> names = output.Keys
                .OrderBy(a => a.ToLower())
                .ToList();

            IEnumerable<string> query = output.Keys
                .OrderBy(a => a.ToLower(), StringComparer.OrdinalIgnoreCase)
                .Select(a => output[a]);

            foreach (string s in names)
            {
                Debug.WriteLine(s);
            }

            foreach (string str in query)
            {
                result.Append(str);
            }
        }

        private static string FormatDigest(byte[] input)
        {
            return BitConverter.ToString(input).Replace("-", string.Empty).ToLower();
        }

        private static string Sha256(string filename)
        {
            using (SHA256 md5 = SHA256.Create())
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    return FormatDigest(md5.ComputeHash(stream));
                }
            }
        }

        private static string Sha256Unicode(string input)
        {
            using (SHA256 md5 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                return FormatDigest(md5.ComputeHash(bytes));
            }
        }
    }
}