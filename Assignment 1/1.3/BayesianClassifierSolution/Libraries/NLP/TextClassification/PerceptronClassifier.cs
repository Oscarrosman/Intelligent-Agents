using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP.TextClassification
{
    public class PerceptronClassifier: NaiveBayesianClassifier
    {
        private double bias;
        private List<double> weightList = null;
        /*
        public override int Classify(List<int> tokenIndexList)
        {
            // ToDo: Write this method

            // The input should be the indices (in the vocabulary) of the
            // words in the text that is being classified.


            // Remove the line below - needed for compilation, since the method must return an integer.
            // The returned integer should be the class ID (in this case, either 0 or 1).
            return 0;

        }
        */
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
