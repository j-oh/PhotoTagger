using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTagger
{
    class TagConcatenator
    {
        public static void ConcatenateTags(List<word_weight> weightedList, string docReference)//params: weightedList-the list of words that will be concatenated so long as there are still words to be concatenated, docReference-the document to check if concatenations are reasonable
        {
            string to_process = docReference;
            //prepare docReference for processing
            to_process = to_process.ToLower();//don't compare case
            to_process = to_process.Trim();//remove leading and trailing whitespace
            to_process = new string(to_process.ToCharArray().Where(c => (!char.IsPunctuation(c) || c == '\'')).ToArray());//remove punctuation
            while (to_process.IndexOf("  ") != -1)
            {
                to_process = to_process.Replace("  ", " ");//replace double space with single space
            }
            //end prepare title content for processing

            bool changedList = false;
            string concat = "";
            for (int i = 0; i < 50; i++)
            {
                string concatTest1 = weightedList[i].word;
                for (int j = 0; j < 50; j++)
                {
                    concat = concatTest1 + " " + weightedList[j].word;
                    int index = Int32.MaxValue;
                    int concatOccur = 0;
                    do
                    {
                        index = to_process.IndexOf(concat);
                        if(index != -1)
                        {
                            concatOccur++;
                            to_process = to_process.Substring(0, index) + to_process.Substring(index + concat.Length);
                        }
                    } while (index != -1) ;
                    if (concatOccur > 3 && concat.Length < 16)
                    {
                        int points = (weightedList[i].points + weightedList[j].points) / 2;
                        word_weight w = new word_weight(concat, points);
                        weightedList.Remove(weightedList[i]);
                        weightedList.Remove(weightedList[j]);
                        i--;//don't get ob1
                        j--;//don't get ob1
                        weightedList.Add(w);
                        changedList = true;
                    }
                }
                while (to_process.IndexOf("  ") != -1)
                {
                    to_process = to_process.Replace("  ", " ");//replace double space with single space
                }
            }
            if (changedList)
            {
                ConcatenateTags(weightedList, docReference);
            }
        }
    }
}
