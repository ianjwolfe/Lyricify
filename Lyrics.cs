using HtmlAgilityPack;
using Lyricify;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml;

namespace Lyricify
{
    internal class Lyrics
    {
        private static string url = "";
        private static string lyrics = "";
        public static string artistName = "";

        public static string Get(string songName)
        {
            return AddLyrics(DownloadAndConvert(songName));
        }

        private static string DownloadAndConvert(string song)
        {
            WebClient downloader = new WebClient();

            downloader.DownloadFile("https://genius.com/api/search?q=" + song, "test.json");

            var path = (Directory.GetCurrentDirectory().ToString() + "\\test.json");
            string file = File.ReadAllText(path);

            XmlDocument xmlDoc = (XmlDocument)JsonConvert.DeserializeXmlNode(file, "response");

            XmlNodeList url = xmlDoc.GetElementsByTagName("url");

            string lyricurl = url[0].InnerText.ToString();

            XmlNodeList artistname = xmlDoc.GetElementsByTagName("full_title");

            artistName = artistname[0].InnerText.ToString();

            File.Delete(path);

            return lyricurl;
        }


        public static string AddLyrics(string site)
        {
            lyrics = "";

            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument document = web.Load(site);

            document.OptionWriteEmptyNodes = true;

            var lyriccontainer = document.DocumentNode.SelectNodes("//div[contains(@class, 'Lyrics__Container-sc-1ynbvzw-6 YYrds')]");

            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//br"))
                node.ParentNode.ReplaceChild(document.CreateTextNode("\r\n"), node);

            foreach (HtmlNode n in lyriccontainer)
            {

                lyrics += System.Net.WebUtility.HtmlDecode(n.InnerText);
            }

            return lyrics;
        }

    }
}
