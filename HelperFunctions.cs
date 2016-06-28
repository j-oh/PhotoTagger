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

        public static void DBInsert(MySqlConnection connection, String pictureUrl, String sourceUrl, List<Tag> tagList)
        {
            //String stringCommand = "DELETE FROM " + table + " WHERE " + primaryKey + "=" + "\"" + key + "\"";
            String stringCommand = "INSERT INTO images(url,source) VALUES(\"" + pictureUrl + "\", \"" + sourceUrl + "\");";
            MySqlCommand command = new MySqlCommand(stringCommand, connection);
            command.ExecuteNonQuery();
            stringCommand = "SELECT id FROM images WHERE url=\"" + pictureUrl + "\"";
            command = new MySqlCommand(stringCommand, connection);
            MySqlDataReader idReader = command.ExecuteReader();
            idReader.Read();
            int id = idReader.GetInt32(0);
            idReader.Close();

            int count = 0;
            foreach (Tag tag in tagList)
            {
                stringCommand = "INSERT INTO tag VALUES(" + id + ", \"" + tag.word + "\", " + tag.priority + ")";
                command = new MySqlCommand(stringCommand, connection);
                command.ExecuteNonQuery();
                count++;
            }
            Console.WriteLine("Inserted '{0}' row into database with {1} tag rows", pictureUrl, tagList.Count);
        }

        public static List<String> DBSearch(MySqlConnection connection, String tag)
        {
            MySqlDataReader idReader = null;
            String stringCommand = "SELECT id FROM tag WHERE img_tag=\"" + tag + "\"";
            MySqlCommand command = new MySqlCommand(stringCommand, connection);
            idReader = command.ExecuteReader();
            List<int> idList = new List<int>();
            while (idReader.Read())
                idList.Add(idReader.GetInt32(0));
            idReader.Close();

            List<String> urlList = new List<String>();
            foreach (int id in idList)
            {
                stringCommand = "SELECT url FROM images WHERE id=" + id.ToString();
                command = new MySqlCommand(stringCommand, connection);
                MySqlDataReader urlReader = command.ExecuteReader();
                if (urlReader.Read())
                {
                    String url = urlReader.GetString(0);
                    url = url.Trim();
                    if (url != "")
                        urlList.Add(url);
                }
                urlReader.Close();
            }
            if (urlList.Count > 0)
            {
                Console.WriteLine("Found '{0}' row in database", tag);
                return urlList;
            }
            else
            {
                Console.WriteLine("Couldn't find '{0}' row in database\n", tag);
                return null;
            }
        }

        public static void DBRemember(List<String> visitedUrls)
        {
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(@"server=db4free.net;userid=jo35;password=kkkKKK12;database=phototag");
                connection.Open();
                Console.WriteLine("Connected to database\n");

                Console.WriteLine("Remembering visited URLs...");
                MySqlDataReader sourceReader = null;
                String stringCommand = "SELECT source FROM images";
                MySqlCommand command = new MySqlCommand(stringCommand, connection);
                sourceReader = command.ExecuteReader();
                while (sourceReader.Read())
                {
                    String url = sourceReader.GetString(0);
                    if (!visitedUrls.Contains(url))
                    {
                        visitedUrls.Add(url);
                        Console.WriteLine("Already visited " + url);
                    }
                }
                Console.WriteLine("Remembered visited URLs\n");
                sourceReader.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        public static void DBDeleteAll()
        {
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(@"server=db4free.net;userid=jo35;password=kkkKKK12;database=phototag");
                connection.Open();
                Console.WriteLine("Connected to database\n");

                MySqlCommand command = new MySqlCommand("DELETE FROM images", connection);
                command.ExecuteNonQuery();
                command = new MySqlCommand("DELETE FROM tag", connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Deleted all rows from all tables\n");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
    }
}
