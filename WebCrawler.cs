﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace PhotoTagger
{
    public struct Tag
    {
        public String word;
        public int priority;
    }

    public struct Picture
    {
        public String url;
        public int priority;
    }

    class WebCrawler
    {
        Dictionary<String, List<Tag>> pictureIndex;
        Dictionary<String, List<Picture>> tagIndex;
        List<String> visitedUrls;
        String startUrl, originUrl;
        String key;
        int crawlLimit;

        public WebCrawler() // Only works with Wikipedia at the moment but can be easily configured to work with other websites as well
        {
            pictureIndex = new Dictionary<String, List<Tag>>();
            tagIndex = new Dictionary<String, List<Picture>>();
            visitedUrls = new List<String>();
            startUrl = "https://en.wikipedia.org/wiki/Main_Page";
            key = "/wiki/";
            originUrl = startUrl.Substring(0, startUrl.IndexOf(key));
            crawlLimit = 200;
            Crawl();
            IndexTags();
            SearchTags();
        }

        private void Crawl() // Goes to random links in the current page's source and processes them into a jumble of words
        {
            String currentUrl = startUrl;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document;
            Random random = new Random();
            while (currentUrl != null && crawlLimit > 0)
            {
                document = web.Load(currentUrl);
                if (!currentUrl.Equals(startUrl) && !visitedUrls.Contains(currentUrl))
                {
                    Console.WriteLine(currentUrl);
                    AnalyzeWikipediaPage(document);
                    visitedUrls.Add(currentUrl);
                    crawlLimit--;
                    Console.WriteLine("\n" + crawlLimit + " remaining...\n");
                }
                HtmlNode[] nextUrls;
                var urlNodes = document.DocumentNode.SelectNodes("//a");
                if (urlNodes != null)
                    nextUrls = urlNodes.ToArray();
                else
                    nextUrls = new HtmlNode[0];
                List<String> potentialUrls = new List<String>();
                foreach (HtmlNode nextUrl in nextUrls)
                {
                    if (nextUrl.Attributes["href"] != null && nextUrl.Attributes["href"].Value.IndexOf(key) == 0 && nextUrl.Attributes["href"].Value.IndexOf(":") < 0)
                        potentialUrls.Add(nextUrl.Attributes["href"].Value);
                }
                if (potentialUrls.Count > 0)
                {
                    int choice = random.Next(potentialUrls.Count - 1);
                    currentUrl = potentialUrls[choice];
                }
                else
                    currentUrl = startUrl;
                if (currentUrl.IndexOf("http") < 0)
                    currentUrl = originUrl + currentUrl;
            }
        }

        private void IndexTags()
        {
            Console.WriteLine("Indexing tags...\n");
            foreach (KeyValuePair<String, List<Tag>> pair in pictureIndex)
            {
                List<Tag> tagList = pair.Value;
                foreach (Tag tag in tagList)
                {
                    List<Picture> pictureList;
                    if (!tagIndex.ContainsKey(tag.word))
                    {
                        pictureList = new List<Picture>();
                        tagIndex.Add(tag.word, pictureList);
                    }
                    else
                        tagIndex.TryGetValue(tag.word, out pictureList);
                    Picture picture = new Picture();
                    picture.url = pair.Key;
                    picture.priority = tag.priority;
                    pictureList.Add(picture);
                    //Console.WriteLine(tag.word);
                }
            }
        }

        private void SearchTags()
        {
            String searchTag = "";
            while (searchTag != "q")
            {
                Console.Write("\nEnter tag to search (q to exit): ");
                searchTag = Console.ReadLine();
                searchTag = searchTag.Trim().ToLower();
                Console.WriteLine();
                if (tagIndex.ContainsKey(searchTag))
                {
                    List<Picture> pictureList;
                    tagIndex.TryGetValue(searchTag, out pictureList);
                    foreach (Picture picture in pictureList)
                    {
                        Console.WriteLine(" " + picture.url);
                        Console.WriteLine("  Relevancy: " + picture.priority + "\n");
                    }
                    Console.WriteLine("\n" + pictureList.Count + " pictures found for this tag\n");
                }
                else
                    Console.WriteLine("\nNo pictures found for this tag\n");
            }
        }

        private void AnalyzeWikipediaPage(HtmlDocument document)
        {
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
                            pictureUrl = originUrl + pictureUrl;
                        if (!visitedUrls.Contains(pictureUrl))
                        {
                            Console.WriteLine("  " + pictureUrl);
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
                                if (Char.IsPunctuation(word[0]))
                                    word = word.Substring(0, 1);
                                if (Char.IsPunctuation(word[word.Length - 1]))
                                    word = word.Substring(0, word.Length - 1);
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
                                    Console.WriteLine("   TAG: " + newTag.word);
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

        private static void AddToArray(ref HtmlNode[] array1, HtmlNode[] array2)
        {
            HtmlNode[] combinedArray = new HtmlNode[array1.Length + array2.Length];
            array1.CopyTo(combinedArray, 0);
            array2.CopyTo(combinedArray, array1.Length);
            array1 = combinedArray;
        }

        private bool AnalyzePage(HtmlDocument document, int spaceIndex) // Takes the current page's information and outputs one word based on spaceIndex (unused)
        {
            var paragraphNodes = document.DocumentNode.SelectNodes("//p");
            bool added = false;
            bool firstWord = false;
            if (spaceIndex == 1)
                firstWord = true;
            //Console.WriteLine(firstWord);
            if (paragraphNodes != null)
            {
                String info = paragraphNodes.First().InnerText;
                while (info.IndexOf(" ") >= 0)
                {
                    String text = info.Substring(0, info.IndexOf(" ") + 1);
                    if (spaceIndex <= 0)
                    {
                        if (!text.Any(Char.IsPunctuation) || text.IndexOf(".") == text.Length - 2)
                        { 
                            if (!firstWord)
                                Console.Write(text);
                            else
                                Console.Write(UppercaseFirst(text));
                        }
                        if (text.IndexOf(".") == text.Length - 2)
                            Console.Write("\n\n     ");
                        else
                            added = true;
                        break;
                    }
                    info = info.Substring(info.IndexOf(" ") + 1);
                    spaceIndex--;
                }
            }
            return added;
        }

        private String UppercaseFirst(String s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
