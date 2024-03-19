using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLP;
using NLP.TextClassification;
using NLP.Tokenization;

namespace PerceptronClassifierApplication
{
    public partial class MainForm : Form
    {
        private const string TEXT_FILE_FILTER = "Text files (*.txt)|*.txt";

        private PerceptronClassifier classifier = null;
        private PerceptronEvaluator evaluator = null;
        private Thread TrainingThread;
        private Boolean optimizerPermission = false;
        private Boolean exportData = false; // Set to true to export accuracy and some vocabulary data as .txt files
        private Vocabulary vocabulary = null;
        private TextClassificationDataSet trainingSet = null;
        private TextClassificationDataSet validationSet = null;
        private TextClassificationDataSet testSet = null;
        private List<float> trainingAccuracyList = new List<float>();
        private List<float> validationAccuracyList = new List<float>();

        public MainForm()
        {
            InitializeComponent();
        }

        private TextClassificationDataSet LoadDataSet()
        {
            TextClassificationDataSet dataSet = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = TEXT_FILE_FILTER;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataSet = new TextClassificationDataSet();
                    StreamReader dataReader = new StreamReader(openFileDialog.FileName);    
                    while (!dataReader.EndOfStream)
                    {
                        string line = dataReader.ReadLine();
                        List<string> lineSplit = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        TextClassificationDataItem item = new TextClassificationDataItem();
                        item.Text = lineSplit[0].ToLower();
                        item.ClassLabel = int.Parse(lineSplit[1]);  
                        dataSet.ItemList.Add(item); 
                    }
                    dataReader.Close();
                    int count0 = dataSet.ItemList.Count(i => i.ClassLabel == 0);
                    int count1 = dataSet.ItemList.Count(i => i.ClassLabel == 1);
                    string fileName = Path.GetFileName(openFileDialog.FileName); // File name without the file path.
                    progressListBox.Items.Add("Loaded data file \"" + fileName + "\" with " + count0.ToString() +
                        " negative reviews and " + count1.ToString() + " positive reviews.");
                }
            }
            return dataSet; 
        }

        private void loadTrainingSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trainingSet = LoadDataSet();
            if ((trainingSet != null) && (validationSet != null) && (testSet != null)) { tokenizeButton.Enabled = true; }
            loadTrainingSetToolStripMenuItem.Enabled = false; // To avoid accidentally reloading the training set instead of the validation set...
        }

        private void loadValidationSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            validationSet = LoadDataSet();
            if ((trainingSet != null) && (validationSet != null) && (testSet != null)) { tokenizeButton.Enabled = true; }
            loadValidationSetToolStripMenuItem.Enabled = false;
        }

        private void loadTestSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testSet = LoadDataSet();
            if ((trainingSet != null) && (validationSet != null) && (testSet != null)) { tokenizeButton.Enabled = true; }
            loadTestSetToolStripMenuItem.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void GenerateVocabulary(TextClassificationDataSet dataSet)
        {
            vocabulary = new Vocabulary();

            foreach (TextClassificationDataItem review in dataSet.ItemList)
            {
                int groundTruth = review.ClassLabel;
                foreach (Token word in review.TokenList)
                {
                    vocabulary.AddWord(word, groundTruth);
                }
            }
        }

    private void tokenizeButton_Click(object sender, EventArgs e)
        {
            // First tokenize the training set:

            // Add code here...
            trainingSet.TokenizeDataSet();

            // Then build the vocabulary from the training set:
            GenerateVocabulary(trainingSet);

            // Next, tokenize the validation set:

            validationSet.TokenizeDataSet();

            // Finally, tokenize the test set:

            testSet.TokenizeDataSet();
            progressListBox.Items.Add("All datasets tokenized and a vocabulary has been created from the training set.");
            indexButton.Enabled = true;
        }

        private void indexButton_Click(object sender, EventArgs e)
        {
            trainingSet.IndexDataSet(vocabulary);
            validationSet.IndexDataSet(vocabulary);
            testSet.IndexDataSet(vocabulary);
            progressListBox.Items.Add("All datasets has been indexed");

            initializeOptimizerButton.Enabled = true;
        }

        private void initializeOptimizerButton_Click(object sender, EventArgs e)
        {
            // Write code here for initializing a perceptron optimizer, which
            // you must also write (i.e. a class called PerceptronOptimizer).
            // Moreover, as mentioned in the assignment text,
            // it might be a good idea to define an evaluator class (e.g. PerceptronEvaluator)
            // You should place both classes in the TextClassification folder in the NLP library.
            int numerOfFeatures = vocabulary.GetVocabularySize();
            classifier = new PerceptronClassifier();
            classifier.Initialize(numerOfFeatures);

            evaluator = new PerceptronEvaluator();

            progressListBox.Items.Clear();
            progressListBox.Items.Add("Initialized peceptron classifyer and evaluator.");

            TrainingThread = new Thread(new ThreadStart(() => TrainPerceptron()));
            startOptimizerButton.Enabled = true;
        }

        private void startOptimizerButton_Click(object sender, EventArgs e)
        {
            startOptimizerButton.Enabled = false;

            // Start the optimizer here.

            // For every epoch, the optimizer should (after the epoch has been completed)
            // trigger an event that prints the current accuracy (over the training set
            // and the validation set) of the perceptron classifier (in a thread-safe
            // manner, and with proper (clear) formatting). Do *not* involve
            // the test set here.
            //optimizerThread = new Thread(new ThreadStart(() => ))
            progressListBox.Items.Add("");
            progressListBox.Items.Add("Training of perceptron started");
            optimizerPermission = true;
            TrainingThread.Start();

            stopOptimizerButton.Enabled = true;
        }

        private void stopOptimizerButton_Click(object sender, EventArgs e)
        {
            stopOptimizerButton.Enabled = false;

            // Stop the optimizer here.

            // For simplicity (even though one may perhaps resume the optimizer), at this
            // point, evaluate the best classifier (= best validation performance) over
            // the *test* set, and print the accuracy to the screen (in a thread-safe
            // manner, and with proper (clear) formatting).
            optimizerPermission = false;
            SafetyCheck("");
            SafetyCheck("Training stopped.");

            PerceptronClassifier newClassifier = evaluator.RecreateClassifier();
            float testAccuracy = newClassifier.CalculateAccuracy(testSet);
            string formattedResult = $"Test accuracy: {testAccuracy:P1}";
            SafetyCheck(formattedResult);

            if (exportData)
            {
                ExportResults exporter = new ExportResults();
                progressListBox.Items.Add($"");
                string outcome1 = exporter.ExportAccuraciesToTxt(trainingAccuracyList, validationAccuracyList);
                progressListBox.Items.Add($"{outcome1}");
                string outcome2 = exporter.ExportVocabularyToTxt(vocabulary, evaluator.weightList);
                progressListBox.Items.Add($"{outcome2}");
                List<TextClassificationDataItem> incorrectlyClassified = newClassifier.SampleIncorrectReviews(testSet);
                string outcome3 = exporter.ExportIncorrectSentences(incorrectlyClassified);
                progressListBox.Items.Add(outcome3);
                foreach (TextClassificationDataItem review in incorrectlyClassified)
                {
                    progressListBox.Items.Add(review.Text);
                }
            }




            stopOptimizerButton.Enabled = true; // A bit ugly, should wait for the
            // optimizer to actually stop, but that's OK, it will stop quickly.
        }

        private void TrainPerceptron()
        {
            int epoch = 0;
            string header = " |  Epoch |  Training accuracy | Validation accuracy |";
            SafetyCheck(header);

            while (optimizerPermission)
            {
                (float trainingAccuracy, float validationAccuracy) = classifier.Optimizer(trainingSet, validationSet);
                trainingAccuracyList.Add(trainingAccuracy);
                validationAccuracyList.Add(evaluator.highestValidationAccuracy);
                float bestValidationAccuracy = evaluator.EvaluateClassifier(classifier, trainingAccuracy, validationAccuracy);
                if (optimizerPermission)
                {
                    string progress = $" | {epoch,6} | {trainingAccuracy,18:P2} | {bestValidationAccuracy,19:P2} |";
                    SafetyCheck(progress);
                }
                    
                epoch++; 
            }
        }

        private void SafetyCheck(string progress)
        {
            if (InvokeRequired) { BeginInvoke(new MethodInvoker(() => PrintProgress(progress))); }
            else { PrintProgress(progress); }
        }

        private void PrintProgress(string progress)
        {
            progressListBox.Items.Add(progress);
        }
    }
}
