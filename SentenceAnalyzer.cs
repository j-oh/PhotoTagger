using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTagger
{
    class SentenceAnalyzer
    {
        public static void AnalyzeDocument(List<word_weight> weightedList, string document)//params: weightedList-the list that has scores to be updated, document-string containing the document
        {
            //prepare document to be analyzed
            document = document.ToLower();//don't compare case
            document = document.Trim();//remove leading and trailing whitespace
            for (int i = 0; i < document.Length; i++)
            {
                if (document.ToCharArray[i] == ' ')//remove all punctuation except end punctuation and apostrophes
                {

                }
            }
        }//AnalyzeDocument(List<word_weight> weightedList, string document)
    }
}
