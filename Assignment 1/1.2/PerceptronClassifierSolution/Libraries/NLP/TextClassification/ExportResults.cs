using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP.TextClassification
{
    public class ExportResults
    {
        private string fileNameAccuracy = "exportedResults.txt";
        private string filenameVocabulary = "exportedVocabulary.txt";
        private string filenameSentences = "incorrectlyClassified.txt";

        public string ExportAccuraciesToTxt(List<float> trainingAccuracy, List<float> validationAccuracy)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileNameAccuracy))
                {
                    for (int i = 0; i < trainingAccuracy.Count; i++)
                    {
                        writer.WriteLine($"{i} {trainingAccuracy[i]} {validationAccuracy[i]}");
                    }
                }

                return $"Accuracies exported to {fileNameAccuracy}";
            }
            catch (Exception ex)
            {
                // Handle any exceptions, e.g., file access issues
                return $"Error exporting accuracies: {ex.Message}";
            }
        }

        public string ExportVocabularyToTxt(Vocabulary vocabulary, List<double> weightList)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filenameVocabulary))
                {
                    for (int i = 0; i < weightList.Count; i++)
                    {
                        if (vocabulary.GetWordAtIndex(i) != "Index not found")
                        { writer.WriteLine($"{vocabulary.GetWordAtIndex(i)} {i} {weightList[i]} {vocabulary.GetTokenData(vocabulary.GetWordAtIndex(i)).Class0Count} {vocabulary.GetTokenData(vocabulary.GetWordAtIndex(i)).Class1Count}"); }
                    }
                    return $"Vocabulary with index and weightlist exported to {filenameVocabulary}";
                }
            }
            catch (Exception ex)
            {
                return $"Error exporting vocabulary: {ex.Message}";
            }
        }

        public string ExportIncorrectSentences(List<TextClassificationDataItem> sentences)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filenameSentences))
                {
                    foreach (TextClassificationDataItem review in sentences)
                    {
                        writer.WriteLine($"{review.ClassLabel} {review.Text}");
                        writer.WriteLine("");
                    }
                }
                return $"Succesfully exported incorrectly classified sentences to {filenameSentences}";
            }
            catch (Exception ex)
            {
                return $"Error exporting sentences: {ex.Message}";
            }
        }
    }
}
