using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLP.Tokenization;

namespace NLP.TextClassification
{
    public class TextClassificationDataSet
    {
        private List<TextClassificationDataItem> itemList;

        public TextClassificationDataSet()
        {
            itemList = new List<TextClassificationDataItem>();
        }

        public List<TextClassificationDataItem> ItemList
        {
            get { return itemList; }
            set { itemList = value; }
        }

        public void TokenizeDataSet()
        {
            Tokenizer tokenizer = new Tokenizer();
            foreach (TextClassificationDataItem review in ItemList)
            {
                string line = review.Text;
                var label = review.ClassLabel;
                TextClassificationDataItem temporaryItem = new TextClassificationDataItem();
                if (line != "")
                {
                    List<Token> output = tokenizer.Tokenize(line);
                    review.TokenList = output;
                }
            }
        }

        public void IndexDataSet(Vocabulary vocabulary)
        {

            foreach (TextClassificationDataItem review in ItemList)
            {
                List<int> temporaryList = new List<int>();
                foreach (Token word in review.TokenList)
                {
                    int index = vocabulary.GetIndex(word.Spelling);
                    temporaryList.Add(index);
                }
                review.IndexedText = temporaryList;
            }
        }

        public List<TextClassificationDataItem> Shuffle()
        {
            List<TextClassificationDataItem> shuffledList = new List<TextClassificationDataItem>(ItemList);
            Random rand = new Random();

            int n = shuffledList.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                TextClassificationDataItem value = shuffledList[k];
                shuffledList[k] = shuffledList[n];
                shuffledList[n] = value;
            }

            return shuffledList;
        }
    }
}
