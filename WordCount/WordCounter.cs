using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WordCount
{
    /// <summary>
    /// Class <c>WordCounter</c> counts the words you feed it and keeps track of how many times a word has been seen in a dictionary. 
    /// It takes a set of stopwords to ignore and (optionally) a dictionary of contractions to treat as seperate words.
    /// 
    /// NOTE: WordCounter automatically filters words to non-possesive when 'is' is a stopword
    /// </summary>
    class WordCounter
    {
        private readonly HashSet<string> _stopWordSet;
        private readonly IDictionary _contractionary;
        private IDictionary _wordFreq;

        /// <summary>
        /// Initializes an instance of WordCounter with a set of stop words to exclude
        /// </summary>
        /// <param name="stopWords">Array of words to exclude</param>
        public WordCounter(string[] stopWords)
        {
            _stopWordSet = new HashSet<string>(stopWords);
            _contractionary = new Dictionary<string, string>();
            _wordFreq = new Dictionary<string, int>();
        }

        /// <summary>
        /// Initializes an instance of WordCounter with a set of stop words to exclude and dictionary of contractions and their constituent words
        /// </summary>
        /// <param name="stopWords">Array of words to exclude</param>
        /// <param name="contractions">A dictionary of contractions (keys) and their space-delimited constituent words (values)</param>
        public WordCounter(string[] stopWords, IDictionary<string, string> contractions)
        {
            _stopWordSet = new HashSet<string>(stopWords);
            _contractionary = (Dictionary<string,string>) contractions;
            _wordFreq = new Dictionary<string, int>();
        }

        /// <summary>
        /// Feed a word into frequency counting logic (which in turn increments the appropriate word's frequency count(s))
        /// </summary>
        /// <param name="word">Word used to update frequency list</param>
        public void FeedWord(string word)
        {
            ParseWord(word);
        }

        /// <summary>
        /// Get the Frequency Dictionary
        /// </summary>
        /// <returns>Frequency Dictionary&lt;string, int&gt; with words (keys) and frequencies (values)</returns>
        public IDictionary GetFreqs()
        {
            return _wordFreq;
        }

        /// <summary>
        /// Get the frequency count for a specific word
        /// </summary>
        /// <param name="word">Word to check</param>
        /// <returns>Frequency of word or -1 if word has no entry</returns>
        public int GetWordFreq(string word)
        {
            word = CleanWord(word);
            int freq = -1;
            if (_wordFreq.Contains(word) && !IsStopWord(word))
                freq = (int)_wordFreq[word];

            return freq;
        }

        /// <summary>
        /// Increments the input word's frequency count if it is not blank or a stopword, treats contractions as seperate words if a contractionary is specified
        /// </summary>
        /// <param name="word">Word used to update frequency list</param>
        private void ParseWord(string inWord)
        {
            inWord = CleanWord(inWord);
            string[] wordArr;

            // If the word is a known contraction, use its constituent words
            if (_contractionary.Contains(inWord))
                wordArr = Decontract(inWord);
            else
                wordArr = new string[] { inWord };

            // for each word found, increment its frequency if it is not blank or a stopword
            foreach(string word in wordArr)
            {
                int freq = -1;
                if (!_stopWordSet.Contains(word) && !word.Equals(""))
                {
                    if (_wordFreq.Contains(word))
                    {
                        freq = (int)_wordFreq[word] + 1;
                        _wordFreq[word] = freq;
                    }
                    else
                    {
                        freq = 1;
                        _wordFreq.Add(word, freq);
                    }
                }
            }
        }

        /// <summary>
        /// Sets word to all lowercase and removes all chars that are not lowercase alphabetical chars and not apostrophe. Possesive words are filtered
        /// to their non-possesive form, but only if 'is' is a stopWord because this also filters contractions ending in 's (only a problem if counting 'is').
        /// </summary>
        /// <param name="word">Word to clean</param>
        /// <returns>Cleaned word</returns>
        private string CleanWord(string word)
        {
            word = word.ToLower();
            word = Regex.Replace(word, @"[^a-z\']", "");

            if (word.EndsWith("\'s") && _stopWordSet.Contains("is"))
                word = word.Replace("\'s", "");

            return word;
        }

        /// <summary>
        /// Returns the words that make up a given contraction according to the contractionary
        /// </summary>
        /// <param name="word">A contraction that exists in the contractionary</param>
        /// <returns>The words that make up the input contraction</returns>
        private string[] Decontract(string word)
        {
            return ((string)_contractionary[word]).Split(' ');
        }

        private bool IsStopWord(string word)
        {
            return _stopWordSet.Contains(word);
        }
    }
}
