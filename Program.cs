using System;
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
            urlChoices.Add("http://abcnews.go.com/Entertainment/ben-affleck-happy-batman-script/story?id=40040351");
            urlChoices.Add("http://www.pcmag.com/news/345423/teslas-model-s-will-sort-of-swim");
            urlChoices.Add("http://www.techtimes.com/articles/165916/20160620/oneplus-3-vs-google-nexus-5x-which-midrange-smartphone-should-you-buy.htm");
            urlChoices.Add("http://www.forbes.com/sites/dougyoung/2016/06/20/apple-loses-but-really-wins-in-china-court-ruling/#644f8c862eb4");
            urlChoices.Add("http://www.techtimes.com/articles/165916/20160620/oneplus-3-vs-google-nexus-5x-which-midrange-smartphone-should-you-buy.htm");
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
            Console.WriteLine("\n\nTitle\n\n" + title.InnerText + "\n\n");

            HtmlNode[] subtitles = new HtmlNode[0];
            String subtitleTag = CheckWebsiteTags(url, "subtitle");
            subtitles = GetTagArray(document, "//" + subtitleTag);
            //AddToArray(ref subtitles, GetTagArray(document, "//meta[@name='Description']"));
            if (subtitles != null)
            {
                for (int i = 0; i < subtitles.Length; i++)
                {
                    HtmlNode subtitle = subtitles[i];
                    HtmlAttribute desc = subtitle.Attributes["content"];
                    if (desc != null)
                        Console.Write("Description " + (i + 1) + "\n\n" + desc.Value + "\n\n\n");
                    else
                        Console.Write("Description " + (i + 1) + "\n\n" + subtitle.InnerText + "\n\n\n");
                }
            }

            HtmlNode[] paragraphs = new HtmlNode[0]; //gets all document lines in the <p> tags
            AddToArray(ref paragraphs, GetTagArray(document, "//p"));
            AddToArray(ref paragraphs, GetTagArray(document, "//p[@class='story-body-text story-content']"));
            String textDocument = "";
            foreach (HtmlNode item in paragraphs)
                textDocument += item.InnerText;//builds a string version of the text document
            List<word_freq> wordList = Occurrence.freq_check(textDocument);//calls a frequency check (see Occurrences.cs)
            RemoveWords(wordList, "stopwords.txt");//removes occurrences of words from list based on a txt document of stopwords(see stopwords.txt)
            int wordLimit = 15;
            if (wordLimit > wordList.Count)
                wordLimit = wordList.Count;
            for (int i = 0; i < wordLimit; i++)
            {
                word_freq word = wordList[i];
                Console.WriteLine(word.word + ": " + word.freq);//Debug test print
            }

            Console.ReadLine();//keep console open
        }

        private static String CheckWebsiteTags(String url, String type)
        {
            switch (type)
            {
                case "title":
                default:
                    if (url.IndexOf("washingtonpost.com") >= 0 || url.IndexOf("pcmag.com") >= 0)
                        return "h1";
                    else if (url.IndexOf("abcnews") >= 0)
                        return "meta[@name='description']";
                    else
                        return "title";
                    break;

                case "subtitle":
                    if (url.IndexOf("washingtonpost.com") >= 0)
                        return "span[@class='pb-caption']";
                    else if (url.IndexOf("abcnews") >= 0)
                        return "meta[@name='description']";
                        //return "span[@class='caption']";
                    else if (url.IndexOf("pcmag.com") >= 0)
                        return "meta[@property='og:description']";
                    else
                        return "meta";
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
