using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace PhotoTagger
{
    static class WikipediaPageAnalyzer
    {
        public static void Analyze(HtmlDocument document, ref Dictionary<String, List<Tag>> pictureIndex, ref List<String> visitedUrls) // Uses subtitles of Wikipedia article pictures to tag them
        {
            int leniency = 10000;

            HtmlNode titleNode = document.DocumentNode.SelectSingleNode("//h1");
            String title = "";
            if (titleNode != null)
            {
                title = titleNode.InnerText;
                Console.WriteLine(title);
            }

            HtmlNode[] paragraphNodes = HelperFunctions.GetNodeArray(document, "//p");
            String textDocument = "";
            foreach (HtmlNode paragraphNode in paragraphNodes)
                textDocument += " " + paragraphNode.InnerText;
            List<word_freq> wordList = Occurrence.freq_check(textDocument);
            HelperFunctions.RemoveWords(wordList, "stopwords.txt");
            List<word_weight> weightedList = TitleChecker.InitWordWeight(wordList);
            if (weightedList.Count > leniency)
                weightedList.RemoveRange(leniency, weightedList.Count - leniency);
            TitleChecker.CompareToTitle(true, weightedList, title);

            HtmlNode[] pictureNodes = new HtmlNode[0];
            pictureNodes = HelperFunctions.GetNodeArray(document, "//div[@class='thumbcaption']");
            Console.WriteLine(" Pictures: " + pictureNodes.Length);
            foreach (HtmlNode pictureNode in pictureNodes)
            {
                try
                {
                    HtmlNode aNode = pictureNode.SelectSingleNode(".//a");
                    HtmlAttribute pictureHref = aNode.Attributes["href"];
                    String pictureUrl = pictureHref.Value;
                    if (pictureUrl.IndexOf("File:") >= 0)
                    {
                        if (pictureUrl.IndexOf("http") < 0)
                            pictureUrl = "https://en.wikipedia.org" + pictureUrl;
                        if (!visitedUrls.Contains(pictureUrl))
                        {
                            //Console.WriteLine("  " + pictureUrl);
                            pictureIndex.Add(pictureUrl, new List<Tag>());
                            List<Tag> tagList;
                            pictureIndex.TryGetValue(pictureUrl, out tagList);
                            String caption = pictureNode.InnerText.Trim();
                            List<word_weight> currentList = new List<word_weight>(weightedList);
                            TitleChecker.CompareToTitle(false, currentList, caption);
                            foreach (word_weight ww in currentList)
                                AddTagPriority(tagList, ww.word, ww.points);
                            /*while (caption.Length > 0)
                            {
                                int breakIndex = caption.Length;
                                if (caption.IndexOf(" ") >= 0)
                                    breakIndex = caption.IndexOf(" ");
                                String word = caption.Substring(0, breakIndex);
                                if (word.Length > 0)
                                {
                                    if (Char.IsPunctuation(word[0]))
                                        word = word.Substring(0, 1);
                                    if (Char.IsPunctuation(word[word.Length - 1]))
                                        word = word.Substring(0, word.Length - 1);
                                }
                                word = word.ToLower();
                                AddTag(tagList, word);
                                if (breakIndex >= caption.Length)
                                    breakIndex = caption.Length - 1;
                                caption = caption.Substring(breakIndex + 1);
                            }*/
                            visitedUrls.Add(pictureUrl);
                        }
                    }
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
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
                Console.Write( " " + newTag.word);
                tagList.Add(newTag);
                //Console.WriteLine("   TAG: " + newTag.word);
            }
        }

        private static void AddTag(List<Tag> tagList, String word)
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
                newTag.priority = 20;
                tagList.Add(newTag);
                //Console.WriteLine("   TAG: " + newTag.word);
            }
        }
    }
}
