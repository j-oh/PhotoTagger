using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTagger
{
    public struct word_freq
    {
        public string word;
        public int freq;
        public word_freq(string wrd)
        {
            word = wrd;
            freq = 1;
        }

    };

    public class Occurrence
    {
        public static List<word_freq> freq_check(string document)
        {
            document = document.ToLower();
            document = document.Trim();
            for(int i = 0; i < document.Length;i++)
            {
                if(Char.IsPunctuation(document, i) && document.ToCharArray()[i] != '\'')
                {
                    document = document.Replace(document.ToCharArray()[i], ' ');
                    i--;
                }
            }
            while(document.IndexOf("  ") != -1)
            {
                document = document.Replace("  ", " ");
            }
            List<word_freq> freq_list = new List<word_freq>();
            while (document != "")
            {
                string checking;
                int inde;
                if (document.IndexOf(" ") >= 0)
                    inde = document.IndexOf(" ");
                else
                    inde = document.Length;
                checking = document.Substring(0, inde);
                document = document.Substring(inde);
                word_freq to_make = new word_freq(checking);
                //Console.Write(checking + " ");
                while (document.IndexOf(" " + checking + " ") != -1)
                {
                    int index = document.IndexOf(" " + checking + " ");
                    document = document.Substring(0, index) + document.Substring(index + checking.Length + 1);
                    to_make.freq++;
                }
                
                freq_list.Add(to_make);
                document = document.Trim();
            }
            var new_list = freq_list.OrderBy(x => -x.freq);
            return new_list.ToList<word_freq>();
        }
    }
}
