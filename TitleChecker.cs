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
        public word_weight(string wrd, int point)
        {
            word = wrd;
            points = point;//init to 0 to determine importance later
        }
    }

    class TitleChecker
    {
        public static List<word_weight> InitWordWeight(List<word_freq> freq_list)//params: freq_list-the list of words which will be transfered to a word_weight list to be scored
        {
            List<word_weight> weight_list = new List<word_weight>();
            foreach (word_freq freq in freq_list)
            {
                weight_list.Add(new word_weight(freq.word, (freq.freq/6)));//init word_weights based on words in the freq_list
            }
            return weight_list;//returns list with all words from freq_list ready to be scored
        }//InitWordWeight(List<word_freq> freq_list) \

        public static void CompareToTitle(bool title, List<word_weight> weight_list, string title_content)//params: title-true for title false for subtitle(these are weighted differently), weight_list-a list of words to be scored, title_content-a string containing the title's text
        {
            //prepare the title_content for processing
            string to_process = new String(title_content.ToArray());
            to_process = to_process.ToLower();//don't compare case
            to_process = to_process.Trim();//remove leading and trailing whitespace
            for (int i = 0; i < to_process.Length; i++)
            {
                if (Char.IsPunctuation(to_process, i) && (to_process.ToCharArray()[i] != '\''))//find non ' punctuation(39 is the dec equivalent of ')
                {
                    to_process = to_process.Replace(to_process.ToCharArray()[i], '\0');//remove all occurrences of non ' punctuation
                    i--;//don't get ob1
                }
            }
            while (to_process.IndexOf("  ") != -1)
            {
                to_process = to_process.Replace("  ", " ");//replace double space with single space
            }
            //end prepare title content for processing
            int index = -1;
            for (int i = 0; i < weight_list.Count; i++)
            {
                do
                {
                    index = to_process.IndexOf(" " + weight_list[i].word+ " ");

                    if (index != -1)
                    {
                        if (title)
                        {
                            word_weight w = weight_list.ElementAt(i);
                            w.points += 3;
                            weight_list[i] = w;
                        }
                        else
                        {
                            word_weight w = weight_list.ElementAt(i);
                            w.points += 4;
                            weight_list[i] = w;
                        }
                    }
                    if (index != -1)
                    {
                        to_process = to_process.Substring(0, index) + to_process.Substring(index + weight_list[i].word.Length);
                        to_process = to_process.Trim();
                    }
                } while (index != -1);
            }
        }
    }
}
