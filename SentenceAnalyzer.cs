using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace PhotoTagger
{
    class SentenceAnalyzer
    {
        static string punctuationToKeep = " ; : ! . ? \' ";
        static string[] punctuation = { "!", ".", "?", ";", ":" };

        public static void AnalyzeDocument(List<word_weight> weightedList, string document)//params: weightedList-the list that has scores to be updated, document-string containing the document
        {
            string to_process = document;//don't want to change the original doc
            //prepare document to be analyzed
            to_process = to_process.ToLower();//don't compare case
            to_process = to_process.Trim();//remove leading and trailing whitespace
            var s = new StringBuilder();//trying to get punctuation removal to work nicely
            foreach (char sb in to_process)
            {
                if (!char.IsPunctuation(sb) || punctuationToKeep.IndexOf(" "+ sb + " ") == 1)//remove all punctuation except end punctuation and apostrophes
                {
                    s.Append(sb);//adds the chars of the string to a string builder                 
                }
                if (sb == '.')//I couldn't make a nice way to keep end punctuation so i just brute forced it. I added spaces after punctuation to help the writers of articles out a bit
                {
                    s.Append(". ");
                }
                else if (sb == '?')
                {
                    s.Append("? ");
                }
                else if (sb == '!')
                {
                    s.Append("! ");
                }
                else if (sb == ';')
                {
                    s.Append("; ");
                }
                else if (sb == '\'')
                {
                    s.Append("'");
                }
                else if (sb == ':')
                {
                    s.Append(": ");
                }//the punctuation included are ones that may delineate thoughts of an author
            }
            to_process = s.ToString();
            while (to_process.IndexOf("  ") != -1)
            {
                to_process = to_process.Replace("  ", " ");//replace double space with single space
            }
            //end prepare title content for processing
            string sentence = "";
            int index = Int32.MaxValue;//set to max value to determine the lowest possible punctuation point in document
            while (to_process != "")
            {
                index = Int32.MaxValue;
                for (int i = 0; i < punctuation.Length; i++)
                {
                    if (to_process.IndexOf(punctuation[i]) < index)
                    {
                        if (to_process.IndexOf(punctuation[i]) != -1)
                        {
                            index = to_process.IndexOf(punctuation[i]);//finds the first point of any punctuation in the document
                        }
                    }
                }
                if (index == Int32.MaxValue)
                {
                    index = to_process.Length - 1;//if it is the last punctuation just go to the end of the doc
                }
                sentence = to_process.Substring(0, index);//get the sentence to analyze
                to_process = to_process.Substring(sentence.Length + 1);//remove sentence from process string
                try
                {
                    using (StreamReader sr = new StreamReader("importancedelineators.txt"))//importancedelineators.txt stores all the leading words that could signify location, people, objects, etc
                    {
                        do
                        {
                            String word = sr.ReadLine();//get key word from txt file
                            for (int i = 0; i < weightedList.Count; i++)
                            {
                                word_weight weight = weightedList[i];
                                int pos_tag = sentence.IndexOf(" " +weight.word+" ");//gets the index of a possible tag to compare to the words in the txt file
                                int pos_key = sentence.IndexOf(" " + word + " ");//gets any index of a word that delineates importance
                                if (pos_key != -1 && pos_tag != -1 && Math.Abs(pos_tag - pos_key) <= 15)//compare the distance from the key word and its subject. if either aren't in the sentence don't add points
                                {
                                    weight.points += 2;//add points if the delineator is close to a tag word
                                    weightedList[i] = weight;//add the newly allocated points to the List
                                }
                            }
                        } while (!sr.EndOfStream);

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);//catch file io exceptions
                }
                to_process = to_process.Trim();//remove whitespace so the while loop will terminate
            }
        }//AnalyzeDocument(List<word_weight> weightedList, string document)
    }
}
