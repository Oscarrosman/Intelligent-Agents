using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP.POS.Taggers
{
    public partial class UnigramTagger : POSTagger
    {
        public Dictionary<string, string> MostCommonTag { get; set; } = new Dictionary<string, string>();
        public void GenerateUnigramTagger(POSDataSet trainingDataSet)
        {
            Dictionary<string, Dictionary<string, float>> associatedTags = trainingDataSet.AssociatedTags;

            foreach (var kvp in associatedTags)
            {
                string word = kvp.Key;
                Dictionary<string, float> innerDictionary = kvp.Value;

                // Find the key with the maximum float value in the inner dictionary
                var maxEntry = innerDictionary.Aggregate((x, y) => x.Value > y.Value ? x : y);
                MostCommonTag[word] = maxEntry.Key;
            }
        }
        public override List<string> Tag(Sentence sentence)
        {
            List<string> assignedTags = new List<string>();

            foreach (TokenData tokenData in sentence.TokenDataList)
            {
                Token currentToken = tokenData.Token;
                string word = currentToken.Spelling;
                if (!MostCommonTag.ContainsKey(word))
                {
                    assignedTags.Add("X");
                }
                else
                {
                    assignedTags.Add(MostCommonTag[word]);
                }
            }
            return assignedTags;
        }

        public float RunUnigramTagger(POSDataSet dataSet)
        {
            int numberOfWords = 0;
            int correctAssignments = 0;

            foreach (Sentence sentence in dataSet.SentenceList)
            {
                List<string> assignedTags = Tag(sentence);
                for (int i = 0; i < sentence.TokenDataList.Count; i++)
                {
                    Token currentToken = sentence.TokenDataList[i].Token;
                    string correctTag = currentToken.POSTag;
                    string assigned = assignedTags[i];
                    numberOfWords += 1;

                    if (assigned == correctTag)
                    {
                        correctAssignments += 1;
                    }
                }
            }
            float accuracy = (float) correctAssignments / numberOfWords;
            return accuracy;
        }
    }
}
