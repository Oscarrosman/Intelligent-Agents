using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP.TextClassification
{
    public class PerceptronEvaluator
    {
        public float highestValidationAccuracy;
        private double bias;
        public List<double> weightList;

        public PerceptronEvaluator()
        {
            highestValidationAccuracy = 0;
            bias = 0;
        }

        public float EvaluateClassifier(PerceptronClassifier classifier, float trainingAccuracy, float validationAccuracy)
        {
            if (validationAccuracy > highestValidationAccuracy)
            {
                highestValidationAccuracy = validationAccuracy;
                weightList = new List<double>(classifier.WeightList);
                bias = classifier.Bias;
            }
            return highestValidationAccuracy;
        }

        public PerceptronClassifier RecreateClassifier()
        {
            PerceptronClassifier classifier = new PerceptronClassifier();
            classifier.Initialize(weightList.Count);
            classifier.WeightList = weightList;
            classifier.Bias = bias;
            return classifier;
        }
    }
}
