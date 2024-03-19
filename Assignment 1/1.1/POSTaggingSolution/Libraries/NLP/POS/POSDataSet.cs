using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP.POS
{
    public class POSDataSet
    {
        private List<Sentence> sentenceList;

        public POSDataSet()
        {
            sentenceList = new List<Sentence>();
        }

        public Dictionary<string, Dictionary<string, float>> AssociatedTags { get; private set; }

        public List<Sentence> SentenceList
        {
            get { return sentenceList; }
            set { sentenceList = value; }
        }

        public void ConvertPOSTags(Dictionary<string, string> translation)
        {
            foreach (Sentence sentence in sentenceList)
            {
                foreach (TokenData tokenData in sentence.TokenDataList)
                {
                    Token currentToken = tokenData.Token;

                    if (translation.TryGetValue(currentToken.POSTag, out string tagAfter))
                    {
                        currentToken.POSTag = tagAfter;
                    }
                    else
                    {
                        // Handle the case where a translation is not found.
                        Console.WriteLine($"Translation not found for tag: {currentToken.POSTag}");
                    }
                }
            }
        }

        // My static method
        public static Tuple<POSDataSet, POSDataSet> SplitDataSet(POSDataSet dataSet, double fraction)
        {
            int splitIndex = (int)Math.Ceiling(dataSet.sentenceList.Count * fraction);

            List<Sentence> trainingSentences = dataSet.sentenceList.GetRange(0, splitIndex);
            List<Sentence> testSentences = dataSet.sentenceList.GetRange(splitIndex, dataSet.sentenceList.Count - splitIndex);

            // Create new instances of POSDataSet for training and testing sets
            POSDataSet trainingSet = new POSDataSet { SentenceList = trainingSentences };
            POSDataSet testSet = new POSDataSet { SentenceList = testSentences };

            return Tuple.Create(trainingSet, testSet);
        }

        public Dictionary<string, Tuple<int, float>> TagInstances()
            // Method to count instances and fractions of each tag in the data set
        {
            Dictionary<string, int> resultDictionary = new Dictionary<string, int>();

            foreach (Sentence sentence in sentenceList)
            {
                foreach (TokenData tokenData in sentence.TokenDataList)
                {
                    Token currentToken = tokenData.Token;
                    string currentTag = currentToken.POSTag;

                    if (!resultDictionary.ContainsKey(currentTag))
                    // If tag not already in dictionary, add and add +1 instance
                    {
                        resultDictionary.Add(currentTag, 1);
                    }
                    else
                    // If tag already in dictionary, +1 instance
                    {
                        resultDictionary[currentTag] += 1;
                    }

                }
            }

            // Finally for each tag calculate the fraction and reformat the dictionary to: Dictionary<posTag, Tuple<instances, fraction>>
            int totalInstances = resultDictionary.Values.Sum();
            Dictionary<string, Tuple<int, float>> formattedResultDictionary = new Dictionary<string, Tuple<int, float>>();
            foreach (var posTag in resultDictionary)
            {
                float fraction = (float)posTag.Value / totalInstances;
                formattedResultDictionary[posTag.Key] = new Tuple<int, float>(posTag.Value, fraction);
            }
            return formattedResultDictionary;
        }

        //public Dictionary<string, Dictionary<string, float>> associatedTags { get; set; }

        public Dictionary<int, int> wordsWithNTags { get; set; }

        public Dictionary<int, int> WordVariations()
        {
            // Initialize associatedTags
            AssociatedTags = new Dictionary<string, Dictionary<string, float>>();

            foreach (Sentence sentence in sentenceList)
            {
                foreach (TokenData tokenData in sentence.TokenDataList)
                {
                    Token currentToken = tokenData.Token;
                    string word = currentToken.Spelling;
                    string posTag = currentToken.POSTag;

                    if (!AssociatedTags.ContainsKey(word))
                    {
                        AssociatedTags[word] = new Dictionary<string, float> { { posTag, 1 } };
                    }
                    else
                    {
                        if (AssociatedTags[word].ContainsKey(posTag))
                        {
                            AssociatedTags[word][posTag] += 1;
                        }
                        else
                        {
                            AssociatedTags[word][posTag] = 1;
                        }
                    }
                }
            }

            Dictionary<int, int> wordsWithNTags = new Dictionary<int, int>();
            foreach (var entry in AssociatedTags)
            {
                string word = entry.Key;
                Dictionary<string, float> posTags = entry.Value;
                //float totalInstances = posTags.Values.Sum();
                int numberOfTags = AssociatedTags[word].Count;

                // Calculate number of words associated with N tags:
                if (!wordsWithNTags.ContainsKey(numberOfTags))
                {
                    wordsWithNTags[numberOfTags] = 1;
                }
                else
                {
                    wordsWithNTags[numberOfTags] += 1;
                }
            }
            return wordsWithNTags;
        }
    }
}
