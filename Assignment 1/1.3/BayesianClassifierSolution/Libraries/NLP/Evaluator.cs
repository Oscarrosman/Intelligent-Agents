using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLP.TextClassification;

namespace NLP
{
    public class Evaluator
    // Should really be a set functions but inexperience in C# made it easier to just make a class...
    {
        private int truePositive = 0;
        private int falsePositive = 0;
        private int falseNegative = 0;

        public void InitializeEvaluator(TextClassificationDataSet dataSet, NaiveBayesianClassifier classifier, int investigatedClass)
        {
            foreach (TextClassificationDataItem review in dataSet.ItemList)
            {
                int assignedLabel = classifier.Classify(review);
                int trueLabel = review.ClassLabel;

                if (assignedLabel == investigatedClass && trueLabel==investigatedClass)
                {
                    truePositive++;
                }
                else if (assignedLabel == investigatedClass && trueLabel != investigatedClass)
                {
                    falsePositive++;
                }
                else if (assignedLabel != investigatedClass && trueLabel == investigatedClass)
                {
                    falseNegative++;
                }
            }
        }

        public float PrecisionMetric()
        {
            return (float)truePositive / (truePositive + falsePositive);
        }

        public float RecallMetric()
        {
            return (float)truePositive / (truePositive + falseNegative);
        }

        public float F1Metric()
        {
            return (float)(2 * PrecisionMetric() * RecallMetric()) / (PrecisionMetric() + RecallMetric());
        }
    }
}
