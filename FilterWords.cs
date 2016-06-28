using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhotoTagger
{
    class FilterWords
    {
        public static void filterLetters(List<word_weight> weightedList)
        {
            for (int i = 0; i < weightedList.Count; i++)
            {
                if (weightedList[i].word.Length < 2)
                {
                    weightedList.Remove(weightedList[i]);
                    i--;
                }
            }
        }

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

        public static void RemoveNumbers(List<word_weight> weightedList)
        {
            for (int i = 0; i < weightedList.Count; i++)
            {
                int num = 0;
                int let = 0;
                string word = weightedList[i].word;
                foreach (char letter in word)
                {
                    if (Char.IsNumber(letter))
                    {
                        num++;
                    }
                    else if (Char.IsLetter(letter))
                    {
                        let++;
                    }
                }
                if (let <= num)
                {
                    weightedList.Remove(weightedList[i]);
                    i--;
                }
            }
        }

        public static List<word_weight> LimitList(int listLimit, List<word_weight> weightedList)
        {
            List<word_weight> new_list = new List<word_weight>();
            if (listLimit > weightedList.Count)
            {
                listLimit = weightedList.Count;//truncate tags that are not in the top limitList
            }
            for (int i = 0; i < listLimit; i++)
            {
                new_list.Add(weightedList[i]);
            }
            return new_list;
        }
    }
}
