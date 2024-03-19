using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP
{
    public class ExportContent
    {
        private string filenameVocabulary = "trainedVocabulary.txt";
        public string ExportVocabulary(Dictionary<string, TokenData> vocabulary)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filenameVocabulary))
                {
                    foreach (var entry in vocabulary)
                    {
                        string word = entry.Key;
                        TokenData data = entry.Value;
                        writer.WriteLine($"{word} {data.Count} {data.Class0Count} {data.Class1Count}");
                    }
                }
                return $"Training vocabulary has been exported to: {filenameVocabulary}";
            }
            catch (Exception ex)
            {
                return $"Error exporting vocabulary: {ex.Message}";
            }
        }
    }
}
