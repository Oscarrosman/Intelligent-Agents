using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP.TextClassification
{
    public class PerceptronClassifier: TextClassifier
    {
        private double bias;
        private List<double> weightList = null;
        private float learningRate = (float)0.1; // Added

        public override void Initialize(int numberOfFeatures)
        {
            // Write this method, setting up the vector of (initially random) weights.
            // Here you can use the Random class, with a suitable (integer) random 
            // number seed.

            int seed = 10;
            Random random = new Random(seed);

            // Initialize the weightList with random values
            weightList = new List<double>();
            for (int i = 0; i < numberOfFeatures; i++)
            {
                // Set each weight to a random value between -1 and 1
                double randomWeight = (random.NextDouble() * 2) - 1;
                weightList.Add(randomWeight);
            }
            bias = (double)1 / numberOfFeatures;

        }

        public (float, float) Optimizer(TextClassificationDataSet trainingSet, TextClassificationDataSet validationSet)
        // Runs one training epoch
        {
            List<TextClassificationDataItem> permutatedTrainingSet = trainingSet.Shuffle();
            List<int> assignedLabels = new List<int>();
            List<int> groundTruth = new List<int>();

            // Classify training set
            foreach (TextClassificationDataItem review in permutatedTrainingSet)
            {
                int assignedLabel = Classify(review.IndexedText);
                assignedLabels.Add(assignedLabel);
                groundTruth.Add(review.ClassLabel);
            }

            // Update weights based on Eq. 4.10
            for (int i = 0; i < permutatedTrainingSet.Count; i++)
            {
                if (assignedLabels[i] != groundTruth[i])
                {
                    // Only updates the weights used to classify the review (selects only feature values != 0)
                    List<int> featureValues = permutatedTrainingSet[i].IndexedText;
                    foreach (int weightIndex in featureValues)
                    {
                        weightList[weightIndex] += learningRate * (groundTruth[i] - assignedLabels[i]);
                    }
                    // Update bias
                    bias += learningRate * (groundTruth[i] - assignedLabels[i]);
                }
            }

            // Get accuracy after finished epoch
            float trainingAccuracy = CalculateAccuracy(trainingSet);
            float validationAccuracy = CalculateAccuracy(validationSet);


            return (trainingAccuracy, validationAccuracy);
        }

        public override int Classify(List<int> tokenIndexList)
        {
            double sum = bias;

            foreach (int indexedWord in tokenIndexList)
            {
                if (indexedWord == -1) { continue; }
                sum += weightList[indexedWord];
            }
            
            // Assign label
            if (sum<=0) { return 0; }

            return 1; 
        }

        public float CalculateAccuracy(TextClassificationDataSet dataSet)
        {
            int corretlyClassified = 0;
            // Classify each review
            foreach (TextClassificationDataItem review in dataSet.ItemList)
            {
                int assignedLabel = Classify(review.IndexedText);
                if (assignedLabel == review.ClassLabel) { corretlyClassified++; }
            }
            // Return the accuracy correct/nReviews
            return (float)corretlyClassified / dataSet.ItemList.Count;
        }

        public List<TextClassificationDataItem> SampleIncorrectReviews(TextClassificationDataSet dataSet)
        {
            List<TextClassificationDataItem> incorrectlyClassified = new List<TextClassificationDataItem>();

            foreach (TextClassificationDataItem review in dataSet.ItemList)
            {
                int assignedLabel = Classify(review.IndexedText);
                if (assignedLabel != review.ClassLabel)
                {
                    incorrectlyClassified.Add(review);
                }
            }
            return incorrectlyClassified;
        }

        public double Bias
        {
            get { return bias; }
            set { bias = value; }
        }

        public List<double> WeightList
        {
            get { return weightList; }
            set { weightList = value; }
        }
    }
}
