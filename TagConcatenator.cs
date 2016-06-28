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
            try
            {
                string to_process = docReference;
                //prepare docReference for processing
                to_process = to_process.ToLower();//don't compare case
                to_process = to_process.Trim();//remove leading and trailing whitespace
                to_process = new string(to_process.ToCharArray().Where(c => (!char.IsPunctuation(c))).ToArray());//remove punctuation
                /*while (to_process.IndexOf("  ") != -1)
                {
                    to_process = to_process.Replace("  ", " ");//replace double space with single space
                }*/
                //end prepare title content for processing

                bool changedList = false;
                string concat = "";
                for (int i = 0; i < weightedList.Count; i++)//only goes to 50 words so that program won't take hours to resolve
                {
                    string concatTest1 = weightedList[i].word;
                    for (int j = 0; j < weightedList.Count; j++)//due to both going to 50 and looping 8 times for each concatenation the loop can run up to 20,000 times without being called back
                    {
                        concat = (concatTest1 + " " + weightedList[j].word).ToLower();//create a concatenation of the words in the list
                        int index = Int32.MaxValue;
                        int concatOccur = 0;
                        do
                        {
                            index = to_process.IndexOf(concat);//get the index of a concatenated string
                            if (index != -1)//if the string exists increment occurance by one
                            {
                                concatOccur++;
                                to_process = to_process.Substring(0, index) + to_process.Substring(index + concat.Length);//remove the concatenated string from the document text
                            }
                        } while (index != -1);
                        if (concatOccur >= 2 && concat.Length < 32)//add a concatenation so long as it occurs twice in the article and is shorter than 32 characters
                        {
                            int points = (weightedList[i].points + weightedList[j].points) / 6;
                            word_weight w = new word_weight(concat, points + (concatOccur * 5));//make a new word weight with the average of the concat objects two point values
                            if (weightedList[i].points < 25 && weightedList[j].points < 25)
                            {
                                weightedList.Remove(weightedList[i]);//remove objects that were concatenated so they arent tagged more than needed
                                weightedList.Remove(weightedList[j]);//remove objects that were concatenated so they arent tagged more than needed
                                if (i != 0)
                                {
                                    i--;
                                }
                                if (j != 0)
                                {
                                    j--;
                                }
                            }
                            //i--;//don't get ob1
                            //j--;//don't get ob1
                            weightedList.Add(w);
                            changedList = false;//set a flag to trigger a recursive call to this function
                        }
                    }
                    /*while (to_process.IndexOf("  ") != -1)
                    {
                        to_process = to_process.Replace("  ", " ");//replace double space with single space
                    }*/
                }
                if (changedList)
                {
                    ConcatenateTags(weightedList, docReference);//call yourself back to attempt to concatenate strings if a string successfully concatenates
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
        }//ConcatenateTags(List<word_weight> weightedList, string docReference)
    }
}
