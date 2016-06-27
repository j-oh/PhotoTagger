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
            /*String url;
            List<String> urlChoices = new List<String>();//list for the user menu
            urlChoices.Add("https://www.washingtonpost.com/politics/trumps-top-example-of-foreign-experience-a-scottish-golf-course-losing-millions/2016/06/22/12ae9cb0-1883-11e6-9e16-2e5a123aac62_story.html?hpid=hp_hp-top-table-main_scotland_1250pm%3Ahomepage%2Fstory");
            urlChoices.Add("http://abcnews.go.com/Politics/donald-trump-slams-hillary-clinton-world-class-liar/story?id=40040353");
            urlChoices.Add("http://www.pcmag.com/news/343547/the-growing-threat-of-ransomware");
            urlChoices.Add("http://www.techtimes.com/articles/165916/20160620/oneplus-3-vs-google-nexus-5x-which-midrange-smartphone-should-you-buy.htm");
            urlChoices.Add("http://www.forbes.com/sites/dougyoung/2016/06/20/apple-loses-but-really-wins-in-china-court-ruling/#644f8c862eb4");
            urlChoices.Add("http://www.nytimes.com/2016/06/21/us/politics/corey-lewandowski-donald-trump.html?_r=0");
            urlChoices.Add("https://www.washingtonpost.com/world/middle_east/israel-wants-someone-to-build-a-5-billion-island-off-gaza--for-a-seaport-hotels-airport/2016/06/20/e45ce6fc-7948-4a10-bef3-0f782b030739_story.html");
            //to test a different option just add desired url to urlChoices list
            Console.WriteLine("Pick a number for a url\n");
            for (int i = 0; i < urlChoices.Count; i++)
                Console.WriteLine(i + ":  " + urlChoices[i] + "\n");
            Console.Write("Choice: ");
            int choice = -1;
            while (choice < 0 || choice >= urlChoices.Count)
                choice = Convert.ToInt32(Console.ReadLine());//allow user to select a url
            url = urlChoices[choice];

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url);//load in the document

            String titleTag = CheckWebsiteTags(url, "title");//look through the document for a title tag
            HtmlNode title = document.DocumentNode.SelectNodes("//" + titleTag).First();//select nodes at previously generated title tag
            Console.WriteLine("\n\nTitle\n\n" + GetNodeText(title) + "\n\n");//print a heading to delineate the text segment in console =

            HtmlNode[] subtitles = new HtmlNode[0];//build an array of nodes in case there are multiple subtitles
            String subtitleTag = CheckWebsiteTags(url, "subtitle");//generate the tag for a subtitle
            subtitles = GetTagArray(document, "//" + subtitleTag);//call to local function: See function
            //AddToArray(ref subtitles, GetTagArray(document, "//meta[@name='Description']"));
            if (subtitles != null)
            {
                for (int i = 0; i < subtitles.Length; i++)
                    Console.WriteLine("Description " + (i + 1) + "\n\n" + GetNodeText(subtitles[i]) + "\n\n\n");//print delineations for subtitles
            }

            HtmlNode[] paragraphs = new HtmlNode[0]; //gets all document lines in the <p> tags
            AddToArray(ref paragraphs, GetTagArray(document, "//p"));//add the tag array to paragraphs
            AddToArray(ref paragraphs, GetTagArray(document, "//p[@class='story-body-text story-content']"));//add the tag array to paragraphs
            String textDocument = "";//a string that will contain a concatenation of all the inner text of the <p> tags in the document
            foreach (HtmlNode item in paragraphs)
                textDocument += GetNodeText(item);//add text to textDocument

            List<word_freq> wordList = Occurrence.freq_check(textDocument);//calls a frequency check (see Occurrences.cs)
            RemoveWords(wordList, "stopwords.txt");//removes occurrences of words from list based on a txt document of stopwords(see stopwords.txt)
            List<word_weight> weightedList = TitleChecker.InitWordWeight(wordList);//build a new weighted list based on the freq_list. the weighted list will assign points based on frequency
            TitleChecker.CompareToTitle(true, weightedList, GetNodeText(title));//the weighted list assigns points based on the title
            TitleChecker.CompareToTitle(false, weightedList, GetNodeText(subtitles[0]));//the weighted list assigns points based on the subtitle
            var new_list = weightedList.OrderBy(x => -x.points);//the list is sorted to show words with higher point values
            weightedList = new_list.ToList<word_weight>();//sets the weighted list equalto the sorting result
            int wordLimit = 15;//number of allowed tags
            if (wordLimit > weightedList.Count)
                wordLimit = weightedList.Count;//truncate tags that are not in the top 15
            for (int i = 0; i < wordLimit; i++)
            {
                word_weight word = weightedList[i];
                Console.WriteLine(word.word + ": " + word.points);//Debug test print
            }*/

            WebCrawler crawler = new WebCrawler("https://en.wikipedia.org/wiki/Main_Page", "/wiki/", 0, false, true);
            //WebCrawler crawler = new WebCrawler("https://www.reddit.com/r/random", "/r/", 30, true, true, "/comments/");

            //Console.ReadLine();//keep console open
        }

        private static String CheckWebsiteTags(String url, String type)//param: url-the url of desired website, type-a string governing if the returned tag is a subtitle or title tag
        {
            switch (type)
            {
                case "title"://case to determine title tags
                default:
                    if (url.IndexOf("washingtonpost.com") >= 0 || url.IndexOf("pcmag.com") >= 0)
                        return "h1";//title tag for washingtonpost.com and pcmag.com
                    else if (url.IndexOf("techtimes.com") >= 0)
                        return "meta[@property='og:title']";//title tag for techtimes.com
                    else
                        return "title";//if neither use default html title tag. Currently only the above websites are guaranteed to work

                case "subtitle"://case to determine subtitle tags
                    if (url.IndexOf("washingtonpost.com") >= 0)
                        return "span[@class='pb-caption']";//subtitle tag for washingtonpost.com
                    else if (url.IndexOf("abcnews") >= 0)
                        return "span[@class='caption']";//subtitle tag for abcnews
                    else if (url.IndexOf("nytimes.com") >= 0)
                        return "span[@class='caption-text']";//subtitle tag for nytimes.com
                    else
                        return "meta[@property='og:description']";//use standard html format. Again support for test version is limited further development in full version
            }
        }
    }
}
