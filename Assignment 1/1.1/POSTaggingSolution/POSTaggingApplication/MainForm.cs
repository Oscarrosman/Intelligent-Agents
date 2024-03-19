using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLP;
using NLP.POS;
using NLP.POS.Taggers;

namespace POSTaggingApplication
{
    public partial class MainForm : Form
    {
        private const string TEXT_FILE_FILTER = "Text files (*.txt)|*.txt";
        private POSDataSet completeDataSet = null;
        private POSDataSet trainingDataSet = null;
        private POSDataSet testDataSet = null;
        private List<TokenData> vocabulary = null;
        private Dictionary<string, string> brownToUniversalMapping;
        private UnigramTagger unigramTagger;

        public MainForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Note: 
        // The Brown corpus is available on the Canvas web page.
        // It can also be obtained at http://www.sls.hawaii.edu/bley-vroman/brown_corpus.html
        // in the file "browntag_nolines.txt: Corpus in one file, tagged, no line numbers, each sentence is one line"
        private void loadPOSCorpusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = TEXT_FILE_FILTER;
                int tokenCount = 0;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    StreamReader fileReader = new StreamReader(openFileDialog.FileName);
                    completeDataSet = new POSDataSet();
                    while (!fileReader.EndOfStream)
                    {
                        string line = fileReader.ReadLine();
                        if (line != "")
                        {
                            List<string> lineSplit = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            List<TokenData> tokenDataList = new List<TokenData>();
                            Sentence sentence = new Sentence();
                            foreach (string lineSplitItem in lineSplit)
                            {
                                List<string> spellingAndTag = lineSplitItem.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                Token token = new Token();
                                if (spellingAndTag.Count == 2) // Needed in order to ignore the very last line that just contains "_.".
                                {
                                    token.Spelling = spellingAndTag[0].ToLower().Trim(); // Convert all words to lowercase.
                                    token.POSTag = spellingAndTag[1].Trim();
                                }
                                TokenData tokenData = new TokenData(token);
                                if (token.POSTag.Length == 1 || token.POSTag[1] != '|') // A somewhat ugly fix, needed to remove some junk from the data ...
                                {
                                    tokenDataList.Add(tokenData);
                                    tokenCount++;
                                }
                            }
                            sentence.TokenDataList = tokenDataList;
                            completeDataSet.SentenceList.Add(sentence);
                        }
                    }
                    fileReader.Close();
                    resultsListBox.Items.Add("Loaded the Brown corpus with " + completeDataSet.SentenceList.Count.ToString()
                        + " sentences and " + tokenCount.ToString() + " tokens.");
                }
            }
        }

        private List<TokenData> GenerateVocabulary(POSDataSet dataSet)
        {
            List<TokenData> tmpTokenDataList = new List<TokenData>();
            foreach (Sentence sentence in dataSet.SentenceList)
            {
                foreach (TokenData tokenData in sentence.TokenDataList)
                {
                    tmpTokenDataList.Add(tokenData);
                }
            }
            // Sort in alphabetical order, then by tag (also in alphabetical order)...
            // This takes a few seconds to run: It would have been more elegant (and easy) to put the
            // computation in a separate thread, but I didn't bother to do that here, as it would make
            // the code slightly more complex. Here, it is OK that the application freezes for a few
            // seconds while it is sorting the data.
            tmpTokenDataList = tmpTokenDataList.OrderBy(t => t.Token.Spelling).ThenBy(t => t.Token.POSTag).ToList();
            // ... then merge
            List<TokenData> tokenDataList = MergeTokens(tmpTokenDataList);
            return tokenDataList;
        }

        private List<TokenData> MergeTokens(List<TokenData> unmergedDataSet)
        {
            List<TokenData> mergedDataSet = new List<TokenData>();
            if (unmergedDataSet.Count > 0)
            {
                int index = 0;
                Token currentToken = new Token();
                currentToken.Spelling = unmergedDataSet[index].Token.Spelling;
                currentToken.POSTag = unmergedDataSet[index].Token.POSTag;
                TokenData currentTokenData = new TokenData(currentToken);
                index++;
                while (index < unmergedDataSet.Count)
                {
                    Token nextToken = unmergedDataSet[index].Token;
                    if ((nextToken.Spelling == currentToken.Spelling) && (nextToken.POSTag == currentToken.POSTag))
                    {
                        currentTokenData.Count += 1;
                    }
                    else
                    {
                        mergedDataSet.Add(currentTokenData);
                        currentToken = new Token();
                        currentToken.Spelling = unmergedDataSet[index].Token.Spelling;
                        currentToken.POSTag = unmergedDataSet[index].Token.POSTag;
                        currentTokenData = new TokenData(currentToken);
                    }
                    index++;
                }
                mergedDataSet.Add(currentTokenData); // Add the final element as well ...
            }
            return mergedDataSet;
        }


        private void loadTagConversionDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

            brownToUniversalMapping = new Dictionary<string, string> { };

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = TEXT_FILE_FILTER;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    StreamReader fileReader = new StreamReader(openFileDialog.FileName);
                    while (!fileReader.EndOfStream)
                    {
                        string line = fileReader.ReadLine();
                        // Same method load POS corpus
                        if (line != "")
                        {
                            List<string> lineSplit = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            brownToUniversalMapping.Add(lineSplit[0], lineSplit[1]);

                        }
                    }
                    fileReader.Close();
                    resultsListBox.Items.Add("Loaded the Brown to universal mapping with " + brownToUniversalMapping.Count.ToString() + " mappings.");

                }    
            }

            // Keep these lines: They will activate the conversion button, provided that the
            // Brown data set has been loaded first:
            if (completeDataSet != null)
            {
                if (completeDataSet.SentenceList.Count > 0)
                {
                    convertPOSTagsButton.Enabled = true;
                }
            }
        }

        private void convertPOSTagsButton_Click(object sender, EventArgs e)
        {
            // Method call:
            completeDataSet.ConvertPOSTags(brownToUniversalMapping);
            resultsListBox.Items.Add("Converted tags to universal ");

            vocabulary = GenerateVocabulary(completeDataSet);

            // Keep this line: It will activate the split button.
            splitDataSetButton.Enabled = true;
        }

        private void splitDataSetButton_Click(object sender, EventArgs e)
        {
            trainingDataSet = new POSDataSet();
            testDataSet = new POSDataSet();
            double splitFraction;
            Boolean splitFractionOK = double.TryParse(splitFractionTextBox.Text, out splitFraction);
            if (splitFractionOK && splitFraction > 0 && splitFraction <= 1)
            {
                var result = POSDataSet.SplitDataSet(completeDataSet, splitFraction);
                trainingDataSet.SentenceList = result.Item1.SentenceList;
                testDataSet.SentenceList = result.Item2.SentenceList;
                resultsListBox.Items.Add("Split data completed. Sentences in train: " + trainingDataSet.SentenceList.Count() + ". Sentences in test: " + testDataSet.SentenceList.Count());

                // Keep these lines: It will activate the statistics generation button and the unigram tagger generation button,
                // once the data set has been split.
                generateStatisticsButton.Enabled = true;
                generateUnigramTaggerButton.Enabled = true;
            }
            else
            {
                MessageBox.Show("Incorrectly specified split fraction", "Error", MessageBoxButtons.OK);
            }
        }

        private void generateStatisticsButton_Click(object sender, EventArgs e)
        {
            resultsListBox.Items.Clear(); // Keep this line.

            if (trainingDataSet != null && trainingDataSet.SentenceList.Count > 0)
            {
                // Use method to get statistics
                Dictionary<string, Tuple<int, float>> tagStatistics = trainingDataSet.TagInstances();

                // Display the results
                resultsListBox.Items.Add("Tag Instances and Fractions:");

                foreach (var kvp in tagStatistics)
                {
                    string tag = kvp.Key.PadRight(10);  
                    string instances = kvp.Value.Item1.ToString().PadRight(10);  
                    string fraction = $"{kvp.Value.Item2:P}".PadRight(10);  

                    resultsListBox.Items.Add($"{tag}  {instances}  {fraction}");
                }
            }
            else
            {
                resultsListBox.Items.Add("Training set is empty. Load data before generating statistics.");
            }

            // Use method to get word variations
            var wordsWithNTags = trainingDataSet.WordVariations();

            // Display words with N tags
            resultsListBox.Items.Add("\n");
            resultsListBox.Items.Add("\nWords with N Tags:");
            float totalSumOfValues = wordsWithNTags.Values.Sum();

            foreach (var entry in wordsWithNTags)
            {
                int numberOfTags = entry.Key;
                int count = entry.Value;

                float fraction = (float)count / totalSumOfValues;

                string tag = numberOfTags.ToString().PadRight(10);
                string instances = count.ToString().PadRight(10);
                string fractionString = $"{fraction:P}".PadRight(10);

                resultsListBox.Items.Add($"{tag}  {instances}  {fractionString}");
            }

        }

        private void generateUnigramTaggerButton_Click(object sender, EventArgs e)
        {
            // Keep this line: It will activate the evaluation button for the unigram tagger
            unigramTagger = new UnigramTagger();
            unigramTagger.GenerateUnigramTagger(trainingDataSet);
            runUnigramTaggerButton.Enabled = true;
        }

        private void runUnigramTaggerButton_Click(object sender, EventArgs e)
        {
            resultsListBox.Items.Clear(); // Keep this line.
            float testAccuracy = unigramTagger.RunUnigramTagger(testDataSet);
            float trainingAccuracy = unigramTagger.RunUnigramTagger(trainingDataSet);

            float testAccuracyPercentage = testAccuracy * 100.0f;
            float trainingAccuracyPercentage = trainingAccuracy * 100.0f;

            resultsListBox.Items.Add("Unigram tagger accuracy");
            resultsListBox.Items.Add("");
            resultsListBox.Items.Add(" > Test accuracy: " + testAccuracyPercentage.ToString("F1") + "%");
            resultsListBox.Items.Add("");
            resultsListBox.Items.Add(" > Training accuracy: " + trainingAccuracyPercentage.ToString("F1") + "%");
        }
    }
}
