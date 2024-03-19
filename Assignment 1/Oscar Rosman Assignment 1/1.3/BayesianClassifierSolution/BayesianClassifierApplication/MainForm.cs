using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO; // For the Path class; see LoadDataSet()
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLP;
using NLP.TextClassification;

namespace BayesianClassifierApplication
{
    public partial class MainForm : Form
    {
        private const string TEXT_FILE_FILTER = "Text files (*.txt)|*.txt";

        private TextClassificationDataSet trainingSet = null;
        private TextClassificationDataSet testSet = null;
        private NaiveBayesianClassifier classifier = null;
        private Boolean exportContent = false;

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
            if ((trainingSet != null) && (testSet != null)) { tokenizeButton.Enabled = true; }
            loadTrainingSetToolStripMenuItem.Enabled = false; // To avoid accidentally reloading the training set instead of the validation set...
        }

        private void loadTestSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testSet = LoadDataSet();
            if ((trainingSet != null) && (testSet != null)) { tokenizeButton.Enabled = true; }
            loadTestSetToolStripMenuItem.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tokenizeButton_Click(object sender, EventArgs e)
        {
            trainingSet.TokenizeDataSet();
            testSet.TokenizeDataSet();
            progressListBox.Items.Add("");
            progressListBox.Items.Add("Both datasets have been tokenized");

            initialiseClassifier.Enabled = true;
        }

       private void initialiseClassifier_Click(object sender, EventArgs e)
       {
            classifier = new NaiveBayesianClassifier();
            classifier.InitializeClassifier(trainingSet); // Internal dictionary created based of trainingset and the number of negative/positive instances of each word
            progressListBox.Items.Add($"Naive Bayesian classifier initialized with {classifier.trainedVocabulary.Count} tokens.");

            classifyDataSets.Enabled = true;

            if (exportContent)
            {
                ExportContent exporter = new ExportContent();
                string outcome = exporter.ExportVocabulary(classifier.trainedVocabulary);
                progressListBox.Items.Add(outcome);
            }

       }

        private void classifyDataSets_Click(object sender, EventArgs e)
        {
            progressListBox.Items.Clear();

            float testAccuracy = classifier.CalculateAccuracy(testSet);
            float trainingAccuracy = classifier.CalculateAccuracy(trainingSet);

            progressListBox.Items.Add("");
            progressListBox.Items.Add("Datasets classified: Accuracy [%]");
            progressListBox.Items.Add("");
            progressListBox.Items.Add($"Trainingset: {100*trainingAccuracy} %");
            progressListBox.Items.Add("");
            progressListBox.Items.Add($"Testset: {100*testAccuracy} %");
            generatePerfromanceMeasures.Enabled = true;
        }

        private void generatePerfromanceMeasures_Click(object sender, EventArgs e)
        // Very ugly last minute implementation to find performance measures
        {
            progressListBox.Items.Add("");
            progressListBox.Items.Add("Performance measures: ");
            progressListBox.Items.Add("Training set: ");
            progressListBox.Items.Add(" > Class 0:");
            Evaluator evaluatorTrainingClass0 = new Evaluator();
            Evaluator evaluatorTrainingClass1 = new Evaluator();
            evaluatorTrainingClass0.InitializeEvaluator(trainingSet, classifier, 0);
            evaluatorTrainingClass1.InitializeEvaluator(trainingSet, classifier, 1);
            progressListBox.Items.Add($"   - Precision: {evaluatorTrainingClass0.PrecisionMetric():P2}");
            progressListBox.Items.Add($"   - Recall: {evaluatorTrainingClass0.RecallMetric():P2}");
            progressListBox.Items.Add($"   - F1: {evaluatorTrainingClass0.F1Metric():P2}");
            progressListBox.Items.Add(" > Class 1:");
            progressListBox.Items.Add($"   - Precision: {evaluatorTrainingClass1.PrecisionMetric():P2}");
            progressListBox.Items.Add($"   - Recall: {evaluatorTrainingClass1.RecallMetric():P2}");
            progressListBox.Items.Add($"   - F1: {evaluatorTrainingClass1.F1Metric():P2}");
            progressListBox.Items.Add("");
            progressListBox.Items.Add("Test set: ");
            progressListBox.Items.Add(" > Class 0:");
            Evaluator evaluatorTestClass0 = new Evaluator();
            Evaluator evaluatorTestClass1 = new Evaluator();
            evaluatorTestClass0.InitializeEvaluator(testSet, classifier, 0);
            evaluatorTestClass1.InitializeEvaluator(testSet, classifier, 1);
            progressListBox.Items.Add($"   - Precision: {evaluatorTestClass0.PrecisionMetric():P2}");
            progressListBox.Items.Add($"   - Recall: {evaluatorTestClass0.RecallMetric():P2}");
            progressListBox.Items.Add($"   - F1: {evaluatorTestClass0.F1Metric():P2}");
            progressListBox.Items.Add(" > Class 1:");
            progressListBox.Items.Add($"   - Precision: {evaluatorTestClass1.PrecisionMetric():P2}");
            progressListBox.Items.Add($"   - Recall: {evaluatorTestClass1.RecallMetric():P2}");
            progressListBox.Items.Add($"   - F1: {evaluatorTestClass1.F1Metric():P2}");

        }
    }
}
