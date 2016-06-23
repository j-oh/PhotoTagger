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
            HtmlNode titleNode = document.DocumentNode.SelectSingleNode("//h1");
            if (titleNode != null)
                Console.WriteLine(titleNode.InnerText);

            HtmlNode[] pictureNodes = new HtmlNode[0];
            pictureNodes = GetNodeArray(document, "//div[@class='thumbcaption']");
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
                            while (caption.Length > 0)
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
                                    newTag.priority = 1;
                                    tagList.Add(newTag);
                                    //Console.WriteLine("   TAG: " + newTag.word);
                                }
                                if (breakIndex >= caption.Length)
                                    breakIndex = caption.Length - 1;
                                caption = caption.Substring(breakIndex + 1);
                            }
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

        private static HtmlNode[] GetNodeArray(HtmlDocument document, String tagName)
        {
            var x = document.DocumentNode.SelectNodes(tagName);
            if (x != null)
                return x.ToArray();
            else
                return new HtmlNode[0];
        }
    }
}
