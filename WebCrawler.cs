using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace PhotoTagger
{
    class WebCrawler
    {
        String startUrl, originUrl;
        String key;

        public WebCrawler() // Only works with Wikipedia at the moment but can be easily configured to work with other websites as well
        {
            startUrl = "https://en.wikipedia.org/wiki/Main_Page";
            key = "/wiki/";
            originUrl = startUrl.Substring(0, startUrl.IndexOf(key));
            Crawl();
        }

        private void Crawl() // Goes to random links in the current page's source and processes them into a jumble of words
        {
            String currentUrl = startUrl;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document;
            Random random = new Random();
            while (currentUrl != null)
            {
                document = web.Load(currentUrl);
                if (!currentUrl.Equals(startUrl))
                    AnalyzePage(document);
                Console.WriteLine(currentUrl);
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

        private void AnalyzePage(HtmlDocument document)
        {

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
