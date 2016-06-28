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
        public static bool Analyze(HtmlDocument document, Dictionary<Picture, List<Tag>> pictureIndex, String currentUrl, List<String> visitedUrls) // Uses subtitles of Wikipedia article pictures to tag them
        {
            int leniency = 20;
            int lengthLimit = 50000;

            HtmlNode titleNode = document.DocumentNode.SelectSingleNode("//h1");
            String title = "";
            if (titleNode != null)
            {
                title = titleNode.InnerText;
                Console.WriteLine(title);
            }

            HtmlNode[] pictureNodes = new HtmlNode[0];
            pictureNodes = HelperFunctions.GetNodeArray(document, "//div[@class='thumbcaption']");
            Console.WriteLine(" Pictures: " + pictureNodes.Length);

            HtmlNode[] paragraphNodes = HelperFunctions.GetNodeArray(document, "//p");
            String textDocument = "";
            foreach (HtmlNode paragraphNode in paragraphNodes)
                textDocument += " " + paragraphNode.InnerText;
            if (pictureNodes.Length > 0 && textDocument.Length < lengthLimit)
            {
                List<word_freq> wordList = Occurrence.freq_check(textDocument);
                HelperFunctions.RemoveWords(wordList, "stopwords.txt");
                HelperFunctions.RemoveWords(wordList, "adverbs.txt");
                List<word_weight> weightedList = TitleChecker.InitWordWeight(wordList);
                if (weightedList.Count > leniency)
                    weightedList.RemoveRange(leniency, weightedList.Count - leniency);
                TitleChecker.CompareToTitle(true, weightedList, title);

                foreach (HtmlNode pictureNode in pictureNodes)
                {
                    try
                    {
                        HtmlNode aNode = pictureNode.SelectSingleNode(".//a");
                        HtmlAttribute pictureHref = aNode.Attributes["href"];
                        String pictureUrl = pictureHref.Value;
                        if (pictureUrl.IndexOf("File:") >= 0)
                        {
                            String cutoff = pictureUrl.Substring(pictureUrl.IndexOf("File:") + 5);
                            if (pictureUrl.IndexOf("http") < 0)
                                pictureUrl = "https://en.wikipedia.org" + pictureUrl;
                            if (!visitedUrls.Contains(pictureUrl))
                            {
                                HtmlWeb web = new HtmlWeb();
                                HtmlDocument pictureDocument = web.Load(pictureUrl);
                                HtmlNode[] nextPictureNodes = HelperFunctions.GetNodeArray(pictureDocument, "//img");

                                foreach (HtmlNode nextPictureNode in nextPictureNodes)
                                {
                                    String pictureSource = nextPictureNode.Attributes["src"].Value;
                                    if (pictureSource.IndexOf(cutoff) >= 0)
                                    {
                                        Console.WriteLine("  " + cutoff);
                                        Picture picture = new Picture();
                                        picture.url = "https:" + pictureSource;
                                        picture.source = currentUrl;
                                        if (!pictureIndex.ContainsKey(picture))
                                        {
                                            pictureIndex.Add(picture, new List<Tag>());
                                            List<Tag> tagList;
                                            pictureIndex.TryGetValue(picture, out tagList);
                                            String caption = pictureNode.InnerText.Trim();

                                            List<word_weight> currentList = new List<word_weight>(weightedList); //algorithm stuff
                                            var cList = currentList.OrderBy(x => -x.points);
                                            currentList = cList.ToList<word_weight>();
                                            currentList = FilterWords.LimitList(50, currentList);
                                            TitleChecker.CompareToTitle(true, currentList, title);
                                            TitleChecker.CompareToTitle(false, currentList, caption);
                                            SentenceAnalyzer.AnalyzeDocument(currentList, textDocument);
                                            TagConcatenator.ConcatenateTags(currentList, textDocument);
                                            FilterWords.filterLetters(currentList);
                                            FilterWords.RemoveNumbers(currentList);
                                            cList = currentList.OrderBy(x => -x.points);
                                            currentList = cList.ToList<word_weight>();
                                            currentList = FilterWords.LimitList(20, currentList);
                                            foreach (word_weight ww in currentList)
                                                AddTagPriority(tagList, ww.word, ww.points);
                                            visitedUrls.Add(pictureUrl);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                return true;
            }
            else
            {
                if (pictureNodes.Length <= 0)
                    Console.WriteLine("  Skipped (no pictures)");
                else
                    Console.WriteLine("  Skipped (longer than " + lengthLimit + " characters)");
            }
            return false;
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
