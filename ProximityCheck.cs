using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTagger
{
    class ProximityCheck
    {
        public static void CheckParagraphProximity(List<word_weight> weightedList, string paragraphsNearPicture)//params: weightedList-the list of scored words to be further scaled, paragraphs near picture-a string that contains only text from close to the picture
        {
            paragraphsNearPicture = paragraphsNearPicture.ToLower();//don't compare case
            paragraphsNearPicture = paragraphsNearPicture.Trim();//remove leading and trailing whitespace
            paragraphsNearPicture = new string(paragraphsNearPicture.ToCharArray().Where(c => (!char.IsPunctuation(c) || c == '\'')).ToArray());//does the same as the above commented code

            /*while (paragraphsNearPicture.IndexOf("  ") != -1)
            {
                paragraphsNearPicture = paragraphsNearPicture.Replace("  ", " ");//replace double space with single space
            }*/
            int index = -1;
            for (int i = 0; i < weightedList.Count; i++)//loop for every word in the weighted list
            {
                do
                {
                    index = paragraphsNearPicture.IndexOf(" " + weightedList[i].word + " ");//spaces to only get words from the title
                    if (index != -1)
                    {
                        word_weight w = weightedList.ElementAt(i);
                        w.points += 3;//assign 3 points if the word is in the nearby paragraph(perhaps needs to be tuned for accuracy)
                        weightedList[i] = w;
                    }
                    if (index != -1)//if the word is not in the list leave the list alone and do not try to remove a word
                    {
                        paragraphsNearPicture = paragraphsNearPicture.Substring(0, index) + paragraphsNearPicture.Substring(index + weightedList[i].word.Length);//reset the string to have the word remove from it
                        paragraphsNearPicture = paragraphsNearPicture.Trim();
                    }
                } while (index != -1);
            }
        }//CheckParagraphProximity(List<word_weight> weightedList, string paragraphsNearPicture)
    }
}
