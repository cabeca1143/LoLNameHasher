using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Hasher
{
    internal class Hasher
    {
        static void Main(string[] args)
        {
            bool isInputValid = false;

            while (!isInputValid)
            {
                Console.WriteLine("Would you like to Hash a single string or a Text File?\n1 - Single String\n2 - Text File");

                string optionInput = Console.ReadLine();
                int option = 0;

                try
                {
                    option = int.Parse(optionInput);
                }
                catch
                {
                    PrintError("Invalid Input, please use numbers!\n");
                    continue;
                }

                switch (option)
                {
                    case 1:
                        HashSingleString();
                        isInputValid = true;
                        break;
                    case 2:
                        HashFile();
                        isInputValid = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option! Please select a valid option!\n");
                        break;
                }

                Console.WriteLine("Press Enter to close this program.");
                Console.ReadLine();
            }
        }

        static void PrintError(string error)
        {
            Console.WriteLine(error);
        }

        static void HashSingleString()
        {
            Console.Write("\nInsert the string to hash: ");

            string input = Console.ReadLine();

            Console.WriteLine($"Your Hash is: {Hash(input)}\n");
        }

        static void HashFile()
        {
            Console.Write("\nInsert the path for the file to be read: ");
            string path = Console.ReadLine();
            string fileName = Path.GetFileNameWithoutExtension(path);
            string outputPath = $"{Path.GetDirectoryName(path)}\\";
            string[] pathFileStrings = File.ReadAllLines(path);

            var JsonSettings = new JsonSerializerOptions { WriteIndented = true };
            List<HashObject> stringsToSerialize = new List<HashObject>();

            foreach (string fileString in pathFileStrings)
            {
                stringsToSerialize.Add(new HashObject(fileString, Hash(fileString)));
            }

            var output = JsonSerializer.Serialize(stringsToSerialize, JsonSettings);

            File.WriteAllText($"{outputPath + fileName}Hashed.json", output);
            Console.WriteLine($"Done! Your file was generatied in: {outputPath + fileName}Hashed.json\n");
        }

        static uint Hash(string str)
        {
            uint hash = 0;
            var mask = 0xF0000000;
            for (var i = 0; i < str.Length; i++)
            {
                hash = char.ToLower(str[i]) + 0x10 * hash;
                if ((hash & mask) > 0)
                {
                    hash ^= hash & mask ^ (hash & mask) >> 24;
                }
            }
            return hash;
        }
    }

    public class HashObject
    {
        public string Name { get; set; }
        public uint Hash { get; set; }
        public HashObject(string name, uint hash)
        {
            Name = name;
            Hash = hash;
        }

    }
}
