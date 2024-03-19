using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP
{
    public class Vocabulary
    {
        // Write this class - it should contain a data structure
        // that identifies all the words in the vocabulary via
        // an index. You can either use an alphabetically sorted
        // list or an instance of the Dictionary class (or
        // something else...)

        private Dictionary<string, TokenData> vocabulary;
        private Dictionary<string, int> indexedVocabulary;

        public Vocabulary()
        {
            vocabulary = new Dictionary<string, TokenData>();
            indexedVocabulary = new Dictionary<string, int>();
        }



        public void AddWord(Token word, int label)
        {
            if (vocabulary.ContainsKey(word.Spelling))
            // update class count
            {
                if (label == 0)
                {
                    vocabulary[word.Spelling].Class0Count += 1;
                }
                else if (label == 1)
                {
                    vocabulary[word.Spelling].Class1Count += 1;
                }
                vocabulary[word.Spelling].Count += 1;
            }
            else
            // Add the word to dictionary
            {
                TokenData entry = new TokenData(word);
                if (label == 0)
                {
                    entry.Class0Count = 1;
          
                }
                else if (label == 1)
                {
                    entry.Class1Count = 1;
                }
                vocabulary[word.Spelling] = entry;
                indexedVocabulary[word.Spelling] = vocabulary.Count-1;
            }
        }

        public int GetVocabularySize()
        {
            return vocabulary.Count;
        }

        public List<string> GetAllWordsAsString()
        {
            return vocabulary.Keys.ToList();
        }

        public List<TokenData> GetAllWordsAsTokens()
        {
            return vocabulary.Values.ToList();
        }

        public TokenData GetTokenData(string word)
        {
            return vocabulary[word];
        }

        public int GetIndex(string word)
        {
            if (vocabulary.ContainsKey(word))
            {
                return indexedVocabulary[word];
            }
            else
            {
                int output = -1;
                return output;
            }
        }

        public string GetWordAtIndex(int index)
        {
            var wordEntry = indexedVocabulary.FirstOrDefault(entry => entry.Value == index);

            if (!string.IsNullOrEmpty(wordEntry.Key))
            {
                return wordEntry.Key;
            }
            else
            {
                // Handle the case where the index is not found in the vocabulary
                return "Index not found";
            }
        }
    }
}
