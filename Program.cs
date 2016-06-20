using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace PhotoTagger
{
    class Program
    {
        static void Main(string[] args)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("http://www.techtimes.com/articles/165916/20160620/oneplus-3-vs-google-nexus-5x-which-midrange-smartphone-should-you-buy.htm");
            HtmlNode[] nodes = document.DocumentNode.SelectNodes("//p").ToArray();
            String textDocument = "";
            foreach (HtmlNode item in nodes)
            {
                textDocument += item.InnerText;
            }
            List<word_freq> wordList = Occurrence.freq_check(textDocument);
            foreach(word_freq word in wordList)
            {
                Console.WriteLine(word.word + ": " + word.freq);
            }
            Console.ReadLine();
        }
    }
}
