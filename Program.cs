using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;

namespace PhotoTagger
{
    class Program
    {
        static void Main(string[] args)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("http://www.pcmag.com/news/345423/teslas-model-s-will-sort-of-swim");//Loads the webpage, change string to change webpage
            HtmlNode[] nodes = document.DocumentNode.SelectNodes("//p").ToArray();//gets all document lines in the <p> tags
            String textDocument = "";
            foreach (HtmlNode item in nodes)
            {
                textDocument += item.InnerText;//builds a string version of the text document
            }
            List<word_freq> wordList = Occurrence.freq_check(textDocument);//calls a frequency check (see Occurrences.cs)
            /*RemoveWords(wordList, "pronouns.txt");
            RemoveWords(wordList, "prepositions.txt");
            RemoveWords(wordList, "titles.txt");
            RemoveWords(wordList, "verbs.txt");
            RemoveWords(wordList, "conjunctions.txt");
            RemoveWords(wordList, "commonwords.txt");*/
            RemoveWords(wordList, "stopwords.txt");//removes occurrences of words from list based on a txt document of stopwords(see stopwords.txt)
            foreach (word_freq word in wordList)
            {
                Console.WriteLine(word.word + ": " + word.freq);//Debug test print
            }
            Console.ReadLine();//keep console open
        }

        private static void RemoveWords(List<word_freq> wordList, String filename)//params: wordList-List of occurrences to have removed, filename-file with words to be removed
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    do
                    {
                        String word = sr.ReadLine();//get word to remove from txt file
                        for (int i = 0; i < wordList.Count; i++)
                        {
                            word_freq wordCombo = wordList[i];
                            if (wordCombo.word.Equals(word.ToLower()))//don't compare case
                                wordList.RemoveAt(i);//remove word
                        }
                    } while (!sr.EndOfStream);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }//RemoveWords(List<word_freq> wordList, String filename)
    }
}
