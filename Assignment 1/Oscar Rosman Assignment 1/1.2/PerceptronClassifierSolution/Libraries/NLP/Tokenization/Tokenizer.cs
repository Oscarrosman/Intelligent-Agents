using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP.Tokenization
{
    public class Tokenizer
    {
        public List<Token> Tokenize(string text)
        // Implement your tokenizer here (to handle abbreviations, numbers, special characters, and so on). 
        // You may wish to add more methods to keep the code well-structured
        {
            List<string> review = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<Token> generatedTokens = new List<Token>();

            foreach (string word in review)
            {
                Token currentWord = new Token();
                StringBuilder formattedWord = new StringBuilder();
                int index = 0;
                List<Token> queue = new List<Token>();
                if (word !="")
                {
                    foreach (char character in word)
                    {
                        if (!Char.IsLetterOrDigit(character))
                        {
                            if (index == 0 || index == word.Length-1)
                            // Creates token for punctuation if before or after word
                            {
                                StringBuilder specialCharacter = new StringBuilder();
                                specialCharacter.Append(character);
                                Token specialToken = new Token
                                {
                                    Spelling = specialCharacter.ToString()
                                };
                                if (index==0)
                                {
                                    generatedTokens.Add(specialToken);
                                }
                                else
                                {
                                    queue.Add(specialToken);
                                }
                            }
                            else if (!Char.IsLetterOrDigit(word[index + 1]))
                            {
                                StringBuilder specialCharacter = new StringBuilder();
                                specialCharacter.Append(character);
                                Token specialToken = new Token
                                {
                                    Spelling = specialCharacter.ToString()
                                };
                                queue.Add(specialToken);

                            }
                            else
                            {
                                formattedWord.Append(character);
                            }
                        }
                        else
                        {
                            formattedWord.Append(character);
                        }
                        index+=1;

                    }

                    currentWord.Spelling = formattedWord.ToString();
                    generatedTokens.Add(currentWord);
                    if (queue.Count != 0)
                    {
                        foreach (Token entry in queue)
                        {
                            generatedTokens.Add(entry);
                        }
                        queue.Clear();
                    }
                        

                }
            }

            return generatedTokens;
            
        }
    }
}