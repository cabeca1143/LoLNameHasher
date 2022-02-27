using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Hasher
{
    internal class Hasher
    {
        static JsonSerializerOptions JsonSettings = new JsonSerializerOptions { WriteIndented = true };

        static void Main(string[] args)
        {
            bool isInputValid = false;

            while (!isInputValid)
            {
                Console.WriteLine("Would you like to Hash a single string, a text file, or an entire directory?\n1 - Single String\n2 - Text File\n3 - Directory");

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
                    case 3:
                        HashDirectory();
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
            Console.Write("\nInsert the path of the file to be read: ");

            string path = Console.ReadLine();
            string fileName = Path.GetFileNameWithoutExtension(path);
            string outputPath = $"{Path.GetDirectoryName(path)}\\";
            string[] pathFileStrings = File.ReadAllLines(path);

            List<HashObject> stringsToSerialize = new List<HashObject>();

            foreach (string fileString in pathFileStrings)
            {
                stringsToSerialize.Add(new HashObject(fileString, Hash(fileString)));
            }

            var output = JsonSerializer.Serialize(stringsToSerialize, JsonSettings);

            File.WriteAllText($"{outputPath + fileName}Hashed.json", output);
            Console.WriteLine($"Done! Path to the generated file: {outputPath + fileName}Hashed.json\n");
        }

        static void HashDirectory()
        {
            Console.Write("\nInsert the path of the directory to be read: ");
            string path = Console.ReadLine();

            string directions = "\nSpecify the method of hashing to use:\n1 - Filename Only\n2 - Character Packages\n";
            Console.Write(directions);
            string optionInput = Console.ReadLine();
            int option = StringIntConvert(optionInput);

            while(option != 1 && option != 2)
            {
                PrintError("Invalid Input, please use a number from the options [1, 2].\n");
                Console.Write(directions);
                option = StringIntConvert(Console.ReadLine());
            }

            string[] files = Directory.GetFiles(path);
            List<HashObject> stringsToSerialize = new List<HashObject>();

            List<string> tempNames = new List<string>();

            if (option == 1)
            {
                directions = "\nSpecify any extension filters:\n";
                Console.Write(directions);
                string filter = Console.ReadLine();

                while (filter != string.Empty && !filter.Contains("."))
                {
                    PrintError("Invalid Input, extension filters must start with a period (.)\n");
                    Console.Write(directions);
                    filter = Console.ReadLine();
                }

                foreach (string file in files)
                {
                    if (!file.Contains(filter))
                    {
                        continue;
                    }

                    string nameCutoff = ".";

                    // Particles need to keep .troy part of their file name.
                    if (filter == ".troybin")
                    {
                        nameCutoff = "bin";
                    }

                    string name = Path.GetFileName(file.Substring(0, file.IndexOf(nameCutoff)));
                    if (!tempNames.Contains(name))
                    {
                        tempNames.Add(name);
                        stringsToSerialize.Add(new HashObject(name, Hash(name)));
                    }
                }
            }
            else
            {
                files = Directory.GetDirectories(path);

                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    if (!tempNames.Contains(name))
                    {
                        tempNames.Add(name);
                        stringsToSerialize.Add(new HashObject(name, HashNorm("[Character]" + name + "00")));
                    }
                }
            }

            string output = JsonSerializer.Serialize(stringsToSerialize, JsonSettings);

            File.WriteAllText($"{path}Hashed.json", output);
            Console.WriteLine($"Done! Path to the generated file: {path}Hashed.json\n");
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

        static uint HashNorm(string str)
        {
            uint hash = 0;

            for (var i = 0; i < str.Length; i++)
            {
                hash = char.ToLower(str[i]) + 65599 * hash;
            }

            return hash;
        }

        static int StringIntConvert(string input)
        {
            int result = -1;

            try
            {
                return Convert.ToInt32(input);
            }
            catch
            {
                return result;
            }
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
