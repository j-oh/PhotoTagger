using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace PhotoTagger
{
    class RedditPageAnalyzer
    {
        public static bool Analyze(HtmlDocument document, ref Dictionary<String, List<Tag>> pictureIndex, ref List<String> visitedUrls) // Uses subtitles of Wikipedia article pictures to tag them
        {
            HtmlNode titleNode = document.DocumentNode.SelectSingleNode("//h1");
            String title = "";
            if (titleNode != null)
            {
                title = titleNode.InnerText;
                Console.WriteLine(title + "\n");
            }

            HtmlNode[] linkNodes = HelperFunctions.GetNodeArray(document, "//p[@class='title']");
            foreach (HtmlNode linkNode in linkNodes)
            {
                Console.WriteLine("\n" + linkNode.InnerText);
            }

            return true;
        }

        private static void AddTagPriority(List<Tag> tagList, String word, int priority)
        {
            bool added = false;
            for (int i = 0; i < tagList.Count; i++)
            {
                Tag tag = tagList[i];
                if (tag.word.Equals(word))
                {
                    tag.priority++;
                    added = true;
                    break;
                }
            }
            if (!added)
            {
                Tag newTag = new Tag();
                newTag.word = word;
                newTag.priority = priority;
                //Console.Write( " " + newTag.word);
                tagList.Add(newTag);
                //Console.WriteLine("   TAG: " + newTag.word);
            }
        }
    }
}
