using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP.TextClassification
{
    public class NaiveBayesianClassifier
    {
        public Dictionary<string, TokenData> trainedVocabulary = new Dictionary<string, TokenData>();
        private double positiveWords = 0;
        private double negativeWords = 0;

        public void InitializeClassifier(TextClassificationDataSet trainingSet)
        {
            foreach (TextClassificationDataItem review in trainingSet.ItemList)
            {
                int label = review.ClassLabel;
                foreach (Token word in review.TokenList)
                {
                    if (!trainedVocabulary.ContainsKey(word.Spelling))
                    {
                        trainedVocabulary[word.Spelling] = new TokenData(word, label);
                    }
                    else
                    {
                        trainedVocabulary[word.Spelling].AddInstance(label);
                    }
                    if (label == 0) { negativeWords++; }
                    else { positiveWords++; }
                }
            }
        }

        public int Classify(TextClassificationDataItem review)
        // Classifying a review using naive bayes classification using laplace smoothing and a log transformation.
        {
            double logProbabilityClass0 = 0.0;
            double logProbabilityClass1 = 0.0;

            foreach (Token word in review.TokenList)
            {
                if (trainedVocabulary.ContainsKey(word.Spelling))
                {
                    TokenData data = trainedVocabulary[word.Spelling];
                    logProbabilityClass0 += Math.Log((data.Class0Count + 1.0) / (negativeWords + trainedVocabulary.Count));
                    logProbabilityClass1 += Math.Log((data.Class1Count + 1.0) / (positiveWords + trainedVocabulary.Count));
                }
                else
                {
                    logProbabilityClass0 += Math.Log(1.0 / (negativeWords + trainedVocabulary.Count));
                    logProbabilityClass1 += Math.Log(1.0 / (positiveWords + trainedVocabulary.Count));
                }
            }
            if (logProbabilityClass0 > logProbabilityClass1) { return 0; }
            else { return 1; }
        }

        public float CalculateAccuracy(TextClassificationDataSet dataSet)
        {
            double correctlyAssigned = 0;

            foreach (TextClassificationDataItem review in dataSet.ItemList)
            {
                int assignedLabel = Classify(review);
                if (assignedLabel == review.ClassLabel) { correctlyAssigned++; }
            }

            return (float)correctlyAssigned / dataSet.ItemList.Count;
        }
    }
}