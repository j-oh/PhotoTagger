using System;
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
        String key, antikey;
        int crawlLimit;
        bool beginAtStart, forceData;

        public WebCrawler(String startUrl, String key, int crawlLimit, bool beginAtStart, bool forceData, String antikey = null) // Only works with Wikipedia at the moment but can be easily configured to work with other websites as well
        {
            pictureIndex = new Dictionary<String, List<Tag>>();
            tagIndex = new Dictionary<String, List<Picture>>();
            visitedUrls = new List<String>();
            this.startUrl = startUrl;
            this.key = key;
            this.antikey = antikey;
            originUrl = startUrl.Substring(0, startUrl.IndexOf(key));
            this.crawlLimit = crawlLimit;
            this.beginAtStart = beginAtStart;
            this.forceData = forceData;
            Crawl();
            IndexTags();
            SearchTags();
        }

        private void Crawl() // Goes to random links in the current page's source and adds pictures + tags to pictureIndex until crawlLimit reaches 0
        {
            String currentUrl = startUrl;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document;
            Random random = new Random();
            while (currentUrl != null && crawlLimit > 0)
            {
                document = web.Load(currentUrl);
                bool extractedData = false;
                if ((beginAtStart || !currentUrl.Equals(startUrl)) && !visitedUrls.Contains(currentUrl))
                {
                    //Console.WriteLine(currentUrl);
                    if (startUrl.IndexOf("en.wikipedia.org") >= 0)
                        extractedData = WikipediaPageAnalyzer.Analyze(document, ref pictureIndex, ref visitedUrls);
                    else
                        extractedData = RedditPageAnalyzer.Analyze(document, ref pictureIndex, ref visitedUrls);
                    visitedUrls.Add(currentUrl);
                    if (!forceData || (extractedData && forceData))
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
                    if (nextUrl.Attributes["href"] != null)
                    {
                        String nextUrlString = nextUrl.Attributes["href"].Value;
                        if (!nextUrlString.Equals(currentUrl) && nextUrlString.IndexOf(key) == 0 && nextUrlString.IndexOf(":") < 0 && (antikey == null || nextUrlString.IndexOf(antikey) < 0))
                            potentialUrls.Add(nextUrl.Attributes["href"].Value);
                    }
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

        private void IndexTags() // Transfers information from pictureIndex to tagIndex to make it easy to search
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

        private void SearchTags() // Search for pictures using tags
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
                    pictureList = pictureList.OrderBy(x => x.priority).ToList<Picture>();
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

        private static void AddToArray(ref HtmlNode[] array1, HtmlNode[] array2)
        {
            HtmlNode[] combinedArray = new HtmlNode[array1.Length + array2.Length];
            array1.CopyTo(combinedArray, 0);
            array2.CopyTo(combinedArray, array1.Length);
            array1 = combinedArray;
        }

        private String UppercaseFirst(String s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
