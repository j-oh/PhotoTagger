using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTagger
{
    struct word_weight
    {
        public string word;//string that stores the word
        public int points;//the relative importance of words as determined by a set of processes
        public word_weight(string wrd)
        {
            word = wrd;
            points = 0;//init to 0 to determine importance later
        }
    }

    class TitleChecker
    {
        public static List<word_weight> InitWordWeight(List<word_freq> freq_list)//params: freq_list-the list of words which will be transfered to a word_weight list to be scored
        {
            List<word_weight> weight_list = new List<word_weight>();
            foreach (word_freq freq in freq_list)
            {
                weight_list.Add(new word_weight(freq.word));//init word_weights based on words in the freq_list
            }
            return weight_list;//returns list with all words from freq_list ready to be scored
        }//InitWordWeight(List<word_freq> freq_list) \

        public static void CompareToTitle(bool title, List<word_weight> weight_list, string title_content)//params: title-true for title false for subtitle(these are weighted differently), weight_list-a list of words to be scored, title_content-a string containing the title's text
        {
            //prepare the title_content for processing
            title_content = title_content.ToLower();//don't compare case
            title_content = title_content.Trim();//remove leading and trailing whitespace
            for (int i = 0; i < title_content.Length; i++)
            {
                if (Char.IsPunctuation(title_content, i) && title_content.ToCharArray()[i] != '\'')//find non ' punctuation
                {
                    title_content = title_content.Replace(title_content.ToCharArray()[i], ' ');//remove all occurrences of non ' punctuation
                    i--;//don't get ob1
                }
            }
            while (title_content.IndexOf("  ") != -1)
            {
                title_content = title_content.Replace("  ", " ");//replace double space with single space
            }
            //end prepare title content for processing
            int index = -1;
            for (int i = 0; i < weight_list.Count; i++)
            {
                do
                {
                    index = title_content.IndexOf(weight_list[i].word);
                    if (index != -1)
                    {
                        if (title)
                        {
                            word_weight w = weight_list.ElementAt(i);
                            w.points++;//point values are arbitrary and may be altered
                            weight_list[i] = w;
                        }
                        else
                        {
                            word_weight w = weight_list.ElementAt(i);
                            w.points+=2;//points are arbitrary at the moment
                            weight_list[i] = w;
                        }
                    }
                } while (index != -1);
            }
        }
    }
}
