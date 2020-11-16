using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using SansyHuman.UDE.Util.Math;

using UnityEngine;
using UnityEngine.Assertions;

/*
 * **Story mode score save binary format**
 * Header(12b)
 * | key1(1b) | key2(1b) | key3(1b) | key4(1b) |
 * |                  key5(4b)                 |
 * | entry count(1b) |        dummy(3b)        |
 * 
 * key1 - 4 are used in XOR cipher used on the entry.
 * Entry byte array is encrypted with key1, key2, key3, key4, key1,... and so on.
 * key5 is the multiple of two primes less than 1000.
 * The smaller prime is used as the seed of the Mersenne twister.
 * entry count is the number of entries in the file.
 * 
 * Entry(24b)
 * |     date(4b)    | time(2b)    | character(1b) | stage(1b) |
 * |                         score(8b)                         |
 * | bomb1(1b) | bomb2(1b) | bomb3(1b) | bomb4(1b) | dummy(4b) |
 * 
 * date is an 8-digit integer that represents the date of the score.
 * ex) 2020-10-26: 20201026
 * time is an 4-digit integer that represents the time.
 * ex) 14:35: 1435
 * character is the character's code
 * stage is that stage's code
 * score is a score of the run.
 * bomb1 - 4 is the codes of skills used in the run
 * Entry is encrypted using key1 - 4 with XOR cipher
 * 
 * Binary structure
 * Binary uses little endian.
 * Header is at the start of the binary.
 * There will be at most 20 entries.
 * The start address of the entry is determined by the list of random numbers
 * obtained by Mersenne twister with the seed obtained from key5. All random numbers
 * are greater than or equals 0 and less than 500.
 * If there are duplicated random numbers, then the later ones are skipped and uses
 * next random numbers.
 * The total size of the binary except the header is 50 KiB.
 * The n-th entry's start address is the n-th random number times 100.
 * The rest of the binaries are assigned with random values.
 */

namespace SansyHuman.Save
{
    public struct StoryHeader
    {
        public byte key1;
        public byte key2;
        public byte key3;
        public byte key4;
        public int key5;
        public byte entryCount;
    }

    [Serializable]
    public struct StoryEntry
    {
        public uint date;
        public ushort time;
        public byte character;
        public byte stage;
        public long score;
        public byte bomb1;
        public byte bomb2;
        public byte bomb3;
        public byte bomb4;

        public override string ToString()
        {
            uint year = date / 10000;
            uint month = (date - year * 10000) / 100;
            uint day = date - year * 10000 - month * 100;
            int hour = time / 100;
            int minute = time - hour * 100;

            return $"{year}-{month:00}-{day:00} {hour:00}:{minute:00} {character} {stage} {score} {bomb1} {bomb2} {bomb3} {bomb4}";
        }
    }

    public static class SaveBinaryCreator
    {
        public static readonly int[] Primes = {
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61,
        67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137,
        139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211,
        223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283,
        293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379,
        383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461,
        463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 563,
        569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643,
        647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739,
        743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829,
        839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929, 937,
        941, 947, 953, 967, 971, 977, 983, 991, 997 };

        public static readonly string SaveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.companyName, Application.productName);

        private static byte[] GetBytesLittleEndian(int value)
        {
            var res = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                var tmp = res[0];
                res[0] = res[3];
                res[3] = tmp;
                tmp = res[1];
                res[1] = res[2];
                res[2] = tmp;
            }

            return res;
        }

        private static int ToInt32(byte[] value, int startIndex)
        {
            if (!BitConverter.IsLittleEndian)
            {
                var tmp = value[startIndex];
                value[startIndex] = value[startIndex + 3];
                value[startIndex + 3] = tmp;
                tmp = value[startIndex + 1];
                value[startIndex + 1] = value[startIndex + 2];
                value[startIndex + 2] = tmp;
            }

            return BitConverter.ToInt32(value, startIndex);
        }

        private static byte[] GetBytesLittleEndian(uint value)
        {
            var res = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                var tmp = res[0];
                res[0] = res[3];
                res[3] = tmp;
                tmp = res[1];
                res[1] = res[2];
                res[2] = tmp;
            }

            return res;
        }

        private static uint ToUInt32(byte[] value, int startIndex)
        {
            if (!BitConverter.IsLittleEndian)
            {
                var tmp = value[startIndex];
                value[startIndex] = value[startIndex + 3];
                value[startIndex + 3] = tmp;
                tmp = value[startIndex + 1];
                value[startIndex + 1] = value[startIndex + 2];
                value[startIndex + 2] = tmp;
            }

            return BitConverter.ToUInt32(value, startIndex);
        }

        private static byte[] GetBytesLittleEndian(ushort value)
        {
            var res = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                var tmp = res[0];
                res[0] = res[1];
                res[1] = tmp;
            }

            return res;
        }

        private static ushort ToUInt16(byte[] value, int startIndex)
        {
            if (!BitConverter.IsLittleEndian)
            {
                var tmp = value[startIndex];
                value[startIndex] = value[startIndex + 1];
                value[startIndex + 1] = tmp;
            }

            return BitConverter.ToUInt16(value, startIndex);
        }

        private static byte[] GetBytesLittleEndian(long value)
        {
            var res = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                var tmp = res[0];
                res[0] = res[7];
                res[7] = tmp;
                tmp = res[1];
                res[1] = res[6];
                res[6] = tmp;
                tmp = res[2];
                res[2] = res[5];
                res[5] = tmp;
                tmp = res[3];
                res[3] = res[4];
                res[4] = tmp;
            }

            return res;
        }

        private static long ToInt64(byte[] value, int startIndex)
        {
            if (!BitConverter.IsLittleEndian)
            {
                var tmp = value[startIndex];
                value[startIndex] = value[startIndex + 7];
                value[startIndex + 7] = tmp;
                tmp = value[startIndex + 1];
                value[startIndex + 1] = value[startIndex + 6];
                value[startIndex + 6] = tmp;
                tmp = value[startIndex + 2];
                value[startIndex + 2] = value[startIndex + 5];
                value[startIndex + 5] = tmp;
                tmp = value[startIndex + 3];
                value[startIndex + 3] = value[startIndex + 4];
                value[startIndex + 4] = tmp;
            }

            return BitConverter.ToInt64(value, startIndex);
        }

        private static void PrimeFactorization(int key, out int small, out int big)
        {
            small = -1;
            big = -1;

            for (int i = 0; i < Primes.Length; i++)
            {
                if (key % Primes[i] == 0)
                {
                    int tmp1 = Primes[i];
                    int tmp2 = key / tmp1;
                    Assert.IsTrue(Array.BinarySearch(Primes, tmp2) >= 0);

                    if (tmp1 <= tmp2)
                    {
                        small = tmp1;
                        big = tmp2;
                    }
                    else
                    {
                        small = tmp2;
                        big = tmp1;
                    }
                }
            }
        }

        private static byte[] CreateStorySaveBinary(in StoryHeader header, StoryEntry[] entries)
        {
            byte[] bin = new byte[500012];
            unsafe
            {
                fixed (byte* pBin = &bin[0])
                {
                    int* pBin4 = (int*)pBin;

                    System.Random rand = new System.Random();
                    for (int i = 0; i < 500012 / 4; i++)
                        pBin4[i] = rand.Next();
                }
            }

            bin[0] = header.key1;
            bin[1] = header.key2;
            bin[2] = header.key3;
            bin[3] = header.key4;
            byte[] tmp = GetBytesLittleEndian(header.key5);
            tmp.CopyTo(bin, 4);
            bin[8] = header.entryCount;

            PrimeFactorization(header.key5, out int small, out int _);
            UDEMersenneRandom random = new UDEMersenneRandom((uint)small);
            int[] randList = new int[header.entryCount];
            for (int i = 0; i < randList.Length; i++)
            {
                int tmp2 = random.NextInt(0, 500);
                if (Array.FindIndex(randList, 0, i, t => t == tmp2) >= 0)
                {
                    i--;
                    continue;
                }
                randList[i] = tmp2;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                ref StoryEntry entry = ref entries[i];
                int baseInd = randList[i] * 100 + 12;

                tmp = GetBytesLittleEndian(entry.date);
                tmp[0] ^= header.key1;
                tmp[1] ^= header.key2;
                tmp[2] ^= header.key3;
                tmp[3] ^= header.key4;
                tmp.CopyTo(bin, baseInd);

                tmp = GetBytesLittleEndian(entry.time);
                tmp[0] ^= header.key1;
                tmp[1] ^= header.key2;
                bin[baseInd + 4] = tmp[0];
                bin[baseInd + 5] = tmp[1];

                bin[baseInd + 6] = (byte)(entry.character ^ header.key3);
                bin[baseInd + 7] = (byte)(entry.stage ^ header.key4);

                tmp = GetBytesLittleEndian(entry.score);
                tmp[0] ^= header.key1;
                tmp[1] ^= header.key2;
                tmp[2] ^= header.key3;
                tmp[3] ^= header.key4;
                tmp[4] ^= header.key1;
                tmp[5] ^= header.key2;
                tmp[6] ^= header.key3;
                tmp[7] ^= header.key4;
                tmp.CopyTo(bin, baseInd + 8);

                bin[baseInd + 16] = (byte)(entry.bomb1 ^ header.key1);
                bin[baseInd + 17] = (byte)(entry.bomb2 ^ header.key2);
                bin[baseInd + 18] = (byte)(entry.bomb3 ^ header.key3);
                bin[baseInd + 19] = (byte)(entry.bomb4 ^ header.key4);
            }

            return bin;
        }

        public static void SaveStoryEntries(params StoryEntry[] entries)
        {
            UDEXORRandom random = new UDEXORRandom();
            StoryHeader header = new StoryHeader()
            {
                key1 = (byte)random.NextInt(0, 256),
                key2 = (byte)random.NextInt(0, 256),
                key3 = (byte)random.NextInt(0, 256),
                key4 = (byte)random.NextInt(0, 256),
                key5 = Primes[random.NextInt(0, Primes.Length)] * Primes[random.NextInt(0, Primes.Length)],
                entryCount = (byte)entries.Length
            };

            var bin = CreateStorySaveBinary(header, entries);

            DirectoryInfo info = new DirectoryInfo(SaveDir);
            if (!info.Exists)
                info.Create();

            using (FileStream stream = new FileStream(Path.Combine(SaveDir, "sav0.sav"), FileMode.Create, FileAccess.Write))
            {
                stream.Write(bin, 0, bin.Length);
            }
        }

        public static StoryEntry[] LoadStoryEntries()
        {
            try
            {
                using (FileStream stream = new FileStream(Path.Combine(SaveDir, "sav0.sav"), FileMode.Open, FileAccess.Read))
                {
                    byte[] bin = new byte[500012];
                    stream.Read(bin, 0, 500012);

                    StoryHeader header = new StoryHeader();
                    header.key1 = bin[0];
                    header.key2 = bin[1];
                    header.key3 = bin[2];
                    header.key4 = bin[3];
                    header.key5 = ToInt32(bin, 4);
                    header.entryCount = bin[8];

                    StoryEntry[] entries = new StoryEntry[header.entryCount];

                    PrimeFactorization(header.key5, out int small, out int _);
                    UDEMersenneRandom random = new UDEMersenneRandom((uint)small);
                    int[] randList = new int[header.entryCount];
                    for (int i = 0; i < randList.Length; i++)
                    {
                        int tmp2 = random.NextInt(0, 500);
                        if (Array.FindIndex(randList, 0, i, t => t == tmp2) >= 0)
                        {
                            i--;
                            continue;
                        }
                        randList[i] = tmp2;
                    }

                    for (int i = 0; i < header.entryCount; i++)
                    {
                        ref StoryEntry entry = ref entries[i];
                        int baseInd = randList[i] * 100 + 12;

                        for (int j = 0; j < 24; j += 4)
                        {
                            bin[baseInd + j] ^= header.key1;
                            bin[baseInd + j + 1] ^= header.key2;
                            bin[baseInd + j + 2] ^= header.key3;
                            bin[baseInd + j + 3] ^= header.key4;
                        }

                        entry.date = ToUInt32(bin, baseInd);
                        entry.time = ToUInt16(bin, baseInd + 4);
                        entry.character = bin[baseInd + 6];
                        entry.stage = bin[baseInd + 7];
                        entry.score = ToInt64(bin, baseInd + 8);
                        entry.bomb1 = bin[baseInd + 16];
                        entry.bomb2 = bin[baseInd + 17];
                        entry.bomb3 = bin[baseInd + 18];
                        entry.bomb4 = bin[baseInd + 19];
                    }

                    return entries;
                }
            }
            catch (FileNotFoundException)
            {
                return new StoryEntry[0];
            }
        }
    }
}