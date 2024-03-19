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
    }
}
