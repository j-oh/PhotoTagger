using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;

namespace PhotoTagger
{
    static class HelperFunctions
    {
        public static void RemoveWords(List<word_freq> wordList, String filename)//params: wordList-List of occurrences to have removed, filename-file with words to be removed
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

        public static HtmlNode[] GetNodeArray(HtmlDocument document, String tagName)
        {
            var x = document.DocumentNode.SelectNodes(tagName);
            if (x != null)
                return x.ToArray();
            else
                return new HtmlNode[0];
        }

        public static String GetNodeText(HtmlNode node)//param: node-the node to retrieve inner text from
        {
            HtmlAttribute desc = node.Attributes["content"];
            if (desc != null)
                return desc.Value;
            else
                return node.InnerText;
        }//GetNodeText(HtmlNode node)

        public static void AddToArray(ref HtmlNode[] array1, HtmlNode[] array2)//param: array1-a reference to the array that needs an array added to it, array2-the array to be added to array1 
        {
            HtmlNode[] combinedArray = new HtmlNode[array1.Length + array2.Length];
            array1.CopyTo(combinedArray, 0);
            array2.CopyTo(combinedArray, array1.Length);
            array1 = combinedArray;
        }//AddToArray(ref HtmlNode[] array1, HtmlNode[] array2)

        public static void DBInsert(MySqlConnection connection, String table, String key, List<String> values)
        {
            String primaryKey = "Word";
            if (table.Equals("Photos"))
                primaryKey = "URL";
            String stringCommand = "DELETE FROM " + table + " WHERE " + primaryKey + "=" + "\"" + key + "\"";
            MySqlCommand command = new MySqlCommand(stringCommand, connection);
            command.ExecuteNonQuery();

            stringCommand = "INSERT INTO " + table + " VALUES(\"" + key + "\", ";
            for (int i = 0; i < 20; i++)
            {
                String value = "";
                if (values.Count > i)
                    value = values[i];
                if (i < 19)
                    stringCommand += "\"" + value + "\", ";
                else
                    stringCommand += "\"" + value + "\");";
            }
            command = new MySqlCommand(stringCommand, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Inserted '{0}' row into database", key);
        }

        public static List<String> DBSearch(MySqlConnection connection, String table, String key)
        {
            MySqlDataReader reader = null;
            String stringCommand = "SELECT Pic1,Pic2,Pic3,Pic4,Pic5,Pic6,Pic7,Pic8,Pic9,Pic10,Pic11,Pic12,Pic13,Pic14,Pic15,Pic16,Pic17,Pic18,Pic19,Pic20 FROM " + table + " WHERE Word=\"" + key + "\"";
            MySqlCommand command = new MySqlCommand(stringCommand, connection);
            reader = command.ExecuteReader();
            List<String> urlList = new List<String>();
            if (reader.Read())
            {
                for (int i = 0; i <= 19; i++)
                {
                    String url = reader.GetString(i);
                    url = url.Trim();
                    if (url != "")
                        urlList.Add(url);
                }
                Console.WriteLine("Found '{0}' row in database", key);
                reader.Close();
                return urlList;
            }
            else
            {
                Console.WriteLine("Couldn't find '{0}' row in database\n", key);
                reader.Close();
                return null;
            }
        }
    }
}
