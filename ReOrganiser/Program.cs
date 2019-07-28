using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReOrganiser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to organise");
            List<string> outputs = new List<string>();

            while (true)
            {
                string dirPath = Console.ReadLine();
                if (dirPath == "exit") break;
                Dictionary<string, List<DirectoryInfo>> cartists = null;
                Dictionary<string, List<DirectoryInfo>> bartists = null;

                DirectoryInfo cdir = new DirectoryInfo("Z:/music_C");
                if (cdir.Exists) cartists = cdir.EnumerateDirectories().OrderBy(d => d.Name).GroupBy(d => d.Name.Split(" - ")[0]).ToDictionary(s => s.Key, s => s.ToList());
                DirectoryInfo bdir = new DirectoryInfo("Z:/music");
                if (cdir.Exists) bartists = bdir.EnumerateDirectories().Where(f => f.LastWriteTime < new DateTime(2018,07,11)).OrderBy(d => d.Name).ToDictionary(s => s.Name, s => s.EnumerateDirectories().ToList());

                bartists.Concat(cartists).ToLookup(pair => pair.Key, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.SelectMany(list => list)
                .ToList())?.Keys.ToList().ForEach(art =>
                {
                    List<DirectoryInfo> temp;
                    if (bartists.TryGetValue(art, out temp)) { Console.ForegroundColor = ConsoleColor.Red; }
                    if (cartists.TryGetValue(art, out temp)) {Console.ForegroundColor = ConsoleColor.Cyan; }
                    if (bartists.TryGetValue(art, out temp) && cartists.TryGetValue(art, out temp)) {Console.ForegroundColor = ConsoleColor.Green; }

                    Console.WriteLine(art);
                    outputs.Add(art);

                    if (bartists.TryGetValue(art, out temp))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        foreach (DirectoryInfo d in temp)
                        {
                            Console.WriteLine("   " + d.Name);
                            outputs.Add("   Barney :   " + d.Name);
                        }
                    }

                    if (cartists.TryGetValue(art, out temp))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        foreach (DirectoryInfo d in temp)
                        {
                            Console.WriteLine("   " + d.Name.Substring(art.Count() + 3));

                            outputs.Add("   Cameron:   " + d.Name.Substring(art.Count() + 3));
                        }
                    }

                    
                });
                File.WriteAllLines("Z:\\summary.txt", outputs.ToArray());
                Console.Beep(900, 300);
                Console.WriteLine("Files detected, press y to copy new albums");
                if (Console.ReadKey().Key == ConsoleKey.Y) { Console.Beep(1000, 100); }

                string target = "Z:/Test";
                cartists.Keys.Where(k => !bartists.Keys.Contains(k)).ToList().ForEach(art =>
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    DirectoryInfo subTarget = Directory.CreateDirectory(target + "/" + art);
                    Console.WriteLine($"Creating {subTarget}");
                    cartists[art].ForEach(c =>
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write($"    Copying {c.Name}");
                        Copy(c.FullName,subTarget.FullName+ "\\"+ c.Name.Substring(art.Count() + 3));
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"\r    Copying {c.Name}\n");
                    });



                });
            }

            

        }

        private static void Copy(string source_dir, string destination_dir)
        {
            // substring is to remove destination_dir absolute path (E:\).
            Directory.CreateDirectory(destination_dir);
            // Create subdirectory structure in destination    
            foreach (string dir in System.IO.Directory.GetDirectories(source_dir, "*", System.IO.SearchOption.AllDirectories))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(destination_dir, dir.Substring(source_dir.Length)));
                // Example:
                //     > C:\sources (and not C:\E:\sources)
            }

            foreach (string file_name in System.IO.Directory.GetFiles(source_dir, "*", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Copy(file_name, destination_dir + file_name.Substring(source_dir.Length));
            }
        }
    }
}
