using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;

namespace PhotoTagger
{
    static class HelperFunctions
    {
        public static void RemoveWords(List<word_freq> wordList, String filename)//params: wordList-List of occurrences to have removed, filename-file with words to be removed
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

        public static HtmlNode[] GetNodeArray(HtmlDocument document, String tagName)
        {
            var x = document.DocumentNode.SelectNodes(tagName);
            if (x != null)
                return x.ToArray();
            else
                return new HtmlNode[0];
        }

        public static String GetNodeText(HtmlNode node)//param: node-the node to retrieve inner text from
        {
            HtmlAttribute desc = node.Attributes["content"];
            if (desc != null)
                return desc.Value;
            else
                return node.InnerText;
        }//GetNodeText(HtmlNode node)

        public static void AddToArray(ref HtmlNode[] array1, HtmlNode[] array2)//param: array1-a reference to the array that needs an array added to it, array2-the array to be added to array1 
        {
            HtmlNode[] combinedArray = new HtmlNode[array1.Length + array2.Length];
            array1.CopyTo(combinedArray, 0);
            array2.CopyTo(combinedArray, array1.Length);
            array1 = combinedArray;
        }//AddToArray(ref HtmlNode[] array1, HtmlNode[] array2)
    }
}
