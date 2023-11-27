/******************************
* Madison Pardy's WordCounter *
******************************/

namespace WordCount
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            // Read & Prepare Input File
            StringBuilder rawInput = new StringBuilder(File.ReadAllText(@"../../NOTES/input.txt"));
            rawInput.Replace("\n", " ");
            string[] input = rawInput.ToString().Split(' ');
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = input[i].Trim();
            }

            // Read & Prepare StopWord File
            string rawStopWords = File.ReadAllText(@"../../NOTES/stopwords.txt");
            string[] stopWords = rawStopWords.Split('\n');
            for (var i = 0; i < stopWords.Length; i++)
            {
                stopWords[i] = stopWords[i].Trim();
            }

            // Read & Prepare Contraction CSV
            IDictionary<string, string> contractionary = new Dictionary<string, string>();
            using(var reader = new StreamReader(@"../../NOTES/contractions.csv"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().ToLower();
                    string[] values = line.Split(',');

                    contractionary.Add(values[0], values[1]);
                }
            }

            // Initialize an instance WordCounter Class with a list of stopwords and contractions
            WordCounter wCounter = new WordCounter(stopWords, contractionary);

            // Feed Words into WordCounter
            foreach (string word in input)
            {
                wCounter.FeedWord(word);
            }

            // Return top 10 values
            var freqs = (Dictionary<string, int>) wCounter.GetFreqs();
            Console.WriteLine("Word : Frequency");
            foreach (KeyValuePair<string, int> pair in freqs.OrderByDescending(p => p.Value).Take(10))
            {
                Console.WriteLine($"{pair.Key} : {pair.Value}");
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
