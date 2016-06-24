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
            string to_process = document;
            //prepare document to be analyzed
            to_process = to_process.ToLower();//don't compare case
            to_process = to_process.Trim();//remove leading and trailing whitespace
            var s = new StringBuilder();
            foreach (char sb in to_process)
            {
                if (!char.IsPunctuation(sb) || punctuationToKeep.IndexOf(" "+ sb + " ") == 1)//remove all punctuation except end punctuation and apostrophes
                {
                    s.Append(sb);//remove all occurrences of non ' punctuation                   
                }
                if (sb == '.')
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
                }
            }
            to_process = s.ToString();
            while (to_process.IndexOf("  ") != -1)
            {
                to_process = to_process.Replace("  ", " ");//replace double space with single space
            }
            //end prepare title content for processing
            string sentence = "";
            int index = Int32.MaxValue;
            while (to_process != "")
            {
                index = Int32.MaxValue;
                for (int i = 0; i < punctuation.Length; i++)
                {
                    if (to_process.IndexOf(punctuation[i]) < index)
                    {
                        if (to_process.IndexOf(punctuation[i]) != -1)
                        {
                            index = to_process.IndexOf(punctuation[i]);
                        }
                    }
                }
                if (index == Int32.MaxValue)
                {
                    index = to_process.Length - 1;
                }
                sentence = to_process.Substring(0, index);
                to_process = to_process.Substring(sentence.Length + 1);
                try
                {
                    using (StreamReader sr = new StreamReader("importancedelineators.txt"))
                    {
                        do
                        {
                            String word = sr.ReadLine();//get word to remove from txt file
                            for (int i = 0; i < weightedList.Count; i++)
                            {
                                word_weight weight = weightedList[i];
                                int pos_tag = sentence.IndexOf(" " +weight.word+" ");
                                int pos_key = sentence.IndexOf(" " + word + " ");
                                if (pos_key != -1 && pos_tag != -1 && Math.Abs(pos_tag - pos_key) <= 15)//compare the distance from the key word and its subject
                                {
                                    weight.points += 2;//add points
                                    weightedList[i] = weight;
                                }
                            }
                        } while (!sr.EndOfStream);

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                to_process = to_process.Trim();
            }
        }//AnalyzeDocument(List<word_weight> weightedList, string document)
    }
}
