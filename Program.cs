﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;

namespace PhotoTagger
{
    class Program
    {
        static void Main(string[] args)
        {
            String url;
            List<String> urlChoices = new List<String>();
            urlChoices.Add("https://www.washingtonpost.com/politics/trumps-top-example-of-foreign-experience-a-scottish-golf-course-losing-millions/2016/06/22/12ae9cb0-1883-11e6-9e16-2e5a123aac62_story.html?hpid=hp_hp-top-table-main_scotland_1250pm%3Ahomepage%2Fstory");
            urlChoices.Add("http://abcnews.go.com/Politics/donald-trump-slams-hillary-clinton-world-class-liar/story?id=40040353");
            urlChoices.Add("http://www.pcmag.com/news/343547/the-growing-threat-of-ransomware");
            urlChoices.Add("http://www.techtimes.com/articles/165916/20160620/oneplus-3-vs-google-nexus-5x-which-midrange-smartphone-should-you-buy.htm");
            urlChoices.Add("http://www.forbes.com/sites/dougyoung/2016/06/20/apple-loses-but-really-wins-in-china-court-ruling/#644f8c862eb4");
            urlChoices.Add("http://www.nytimes.com/2016/06/21/us/politics/corey-lewandowski-donald-trump.html?_r=0");
            Console.WriteLine("Pick a number for a url\n");
            for (int i = 0; i < urlChoices.Count; i++)
                Console.WriteLine(i + ":  " + urlChoices[i] + "\n");
            Console.Write("Choice: ");
            int choice = -1;
            while (choice < 0 || choice >= urlChoices.Count)
                choice = Convert.ToInt32(Console.ReadLine());
            url = urlChoices[choice];

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url);

            String titleTag = CheckWebsiteTags(url, "title");
            HtmlNode title = document.DocumentNode.SelectNodes("//" + titleTag).First();
            Console.WriteLine("\n\nTitle\n\n" + GetNodeText(title) + "\n\n");

            HtmlNode[] subtitles = new HtmlNode[0];
            String subtitleTag = CheckWebsiteTags(url, "subtitle");
            subtitles = GetTagArray(document, "//" + subtitleTag);
            //AddToArray(ref subtitles, GetTagArray(document, "//meta[@name='Description']"));
            if (subtitles != null)
            {
                for (int i = 0; i < subtitles.Length; i++)
                    Console.WriteLine("Description " + (i + 1) + "\n\n" + GetNodeText(subtitles[i]) + "\n\n\n");
            }

            HtmlNode[] paragraphs = new HtmlNode[0]; //gets all document lines in the <p> tags
            AddToArray(ref paragraphs, GetTagArray(document, "//p"));
            AddToArray(ref paragraphs, GetTagArray(document, "//p[@class='story-body-text story-content']"));
            String textDocument = "";
            foreach (HtmlNode item in paragraphs)
                textDocument += GetNodeText(item);

            List<word_freq> wordList = Occurrence.freq_check(textDocument);//calls a frequency check (see Occurrences.cs)
            RemoveWords(wordList, "stopwords.txt");//removes occurrences of words from list based on a txt document of stopwords(see stopwords.txt)
            List<word_weight> weightedList = TitleChecker.InitWordWeight(wordList);
            TitleChecker.CompareToTitle(true, weightedList, GetNodeText(title));
            TitleChecker.CompareToTitle(false, weightedList, GetNodeText(subtitles[0]));
            var new_list = weightedList.OrderBy(x => -x.points);
            weightedList = new_list.ToList<word_weight>();
            int wordLimit = 15;
            if (wordLimit > weightedList.Count)
                wordLimit = weightedList.Count;
            for (int i = 0; i < wordLimit; i++)
            {
                word_weight word = weightedList[i];
                Console.WriteLine(word.word + ": " + word.points);//Debug test print
            }

            Console.ReadLine();//keep console open
        }

        private static String GetNodeText(HtmlNode node)
        {
            HtmlAttribute desc = node.Attributes["content"];
            if (desc != null)
                return desc.Value;
            else
                return node.InnerText;
        }

        private static String CheckWebsiteTags(String url, String type)
        {
            switch (type)
            {
                case "title":
                default:
                    if (url.IndexOf("washingtonpost.com") >= 0 || url.IndexOf("pcmag.com") >= 0)
                        return "h1";
                    else if (url.IndexOf("techtimes.com") >= 0)
                        return "meta[@property='og:title']";
                    else
                        return "title";
                    break;

                case "subtitle":
                    if (url.IndexOf("washingtonpost.com") >= 0)
                        return "span[@class='pb-caption']";
                    else if (url.IndexOf("abcnews") >= 0)
                        return "span[@class='caption']";
                    else if (url.IndexOf("nytimes.com") >= 0)
                        return "span[@class='caption-text']";
                    else
                        return "meta[@property='og:description']";
                    break;
            }
        }

        private static void AddToArray(ref HtmlNode[] array1, HtmlNode[] array2)
        {
            HtmlNode[] combinedArray = new HtmlNode[array1.Length + array2.Length];
            array1.CopyTo(combinedArray, 0);
            array2.CopyTo(combinedArray, array1.Length);
            array1 = combinedArray;
        }

        private static HtmlNode[] GetTagArray(HtmlDocument document, String tagName)
        {
            var x = document.DocumentNode.SelectNodes(tagName);
            if (x != null)
                return x.ToArray();
            else
                return new HtmlNode[0];
        }
        
        private static void RemoveWords(List<word_freq> wordList, String filename)//params: wordList-List of occurrences to have removed, filename-file with words to be removed
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    do
                    {
                        String word = sr.ReadLine();//get word to remove from txt file
                        for (int i = 0; i < wordList.Count; i++)
                        {
                            word_freq wordCombo = wordList[i];
                            if (wordCombo.word.Equals(word.ToLower()))//don't compare case
                                wordList.RemoveAt(i);//remove word
                        }
                    } while (!sr.EndOfStream);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }//RemoveWords(List<word_freq> wordList, String filename)
    }
}
