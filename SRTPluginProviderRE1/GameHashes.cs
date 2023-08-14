using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SRTPluginProviderRE1
{
    /// <summary>
    /// SHA256 hashes for the REmake game executables.
    /// </summary>
    public static class GameHashes
    {
        private static readonly byte[] bhdWW_20230801_1 = new byte[32] { 0xD1, 0x8C, 0xDF, 0x47, 0x3C, 0x01, 0xAF, 0xBD, 0x90, 0xCE, 0x1D, 0xF0, 0xC3, 0xF3, 0xBB, 0x4A, 0x3D, 0x6C, 0x4E, 0xBF, 0xE4, 0xD7, 0x92, 0xA0, 0x35, 0x89, 0x4C, 0x63, 0xA2, 0x79, 0x0F, 0x8F };
        private static readonly byte[] bhdWW_20181019_1 = new byte[32] { 0x0A, 0xB6, 0x1B, 0xAD, 0xA3, 0x47, 0x83, 0xA6, 0x84, 0x49, 0x08, 0x58, 0xE2, 0x00, 0x5B, 0xBD, 0x2E, 0x9A, 0x1B, 0x13, 0x53, 0xEA, 0xAA, 0xD4, 0x43, 0x37, 0xBF, 0x7A, 0xBB, 0x77, 0x3B, 0x72 };
        
        public static GameVersion DetectVersion(string filePath)
        {
            byte[] checksum;
            using (SHA256 hashFunc = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                checksum = hashFunc.ComputeHash(fs);

            if (checksum.SequenceEqual(bhdWW_20230801_1))
                return GameVersion.WW_20230801_1;
            else if (checksum.SequenceEqual(bhdWW_20181019_1))
                return GameVersion.WW_20181019_1;
            else
                return GameVersion.Unknown;
        }
    }
}