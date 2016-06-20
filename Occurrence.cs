using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTagger
{
    public struct word_freq
    {
        public string word;//string that stores the occurrence word
        public int freq;//number of times a word occurs in document(to be translated into weight in later processing)
        public word_freq(string wrd)
        {
            word = wrd;
            freq = 1;//init to 1 as the first occurrence causes construction
        }

    };//word_freq

    public class Occurrence
    {
        public static List<word_freq> freq_check(string document)//params: document-the string of the entire document to be processed
        {
            document = document.ToLower();//don't compare case
            document = document.Trim();//remove leading and trailing whitespace
            for(int i = 0; i < document.Length;i++)
            {
                if (Char.IsPunctuation(document, i) && document.ToCharArray()[i] != '\'')//find non ' punctuation
                {
                    document = document.Replace(document.ToCharArray()[i], ' ');//remove all occurrences of non ' punctuation
                    i--;//don't get ob1
                }
            }
            while(document.IndexOf("  ") != -1)
            {
                document = document.Replace("  ", " ");//replace double space with single space
            }
            List<word_freq> freq_list = new List<word_freq>();
            while (document != "")
            {
                string checking;
                int inde;
                if (document.IndexOf(" ") >= 0)
                    inde = document.IndexOf(" ");//find the end of the first word
                else
                    inde = document.Length;//unless it is the last word
                checking = document.Substring(0, inde);//get the first word
                document = document.Substring(inde);//remove the first word from document
                word_freq to_make = new word_freq(checking);//construct a new word_freq struct
                //Console.Write(checking + " ");
                while (document.IndexOf(" " + checking + " ") != -1)//check if the word is in the string. Spaces at beginning and end so that letters are not pulled from full words
                {
                    int index = document.IndexOf(" " + checking + " ");//Get index of single word. Spaces at beginning and end so that letters are not pulled from full words
                    document = document.Substring(0, index) + document.Substring(index + checking.Length + 1);//remove word from document string
                    to_make.freq++;//increment the frequency of current word
                }
                
                freq_list.Add(to_make);//add to list
                document = document.Trim();//remove whitespace to prevent infinite looping
            }
            var new_list = freq_list.OrderBy(x => -x.freq);//lambda has a negative to sort by nonincreasing order
            return new_list.ToList<word_freq>();//return sorted list
        }
    }//freq_check(string document)
}
