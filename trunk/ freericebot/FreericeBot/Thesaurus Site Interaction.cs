using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace FreericeBot
    {
    internal static class Thesaurus_Site_Interaction
        {
        private const string siteUrl = "http://thesaurus.publicdomaindb.org/{0}/{1}.html";
        private static readonly WebRequestWrapper webWrapper = new WebRequestWrapper();

        public static string[] GetSynonymsOfWord(string word)
            {
            string url = string.Format(siteUrl, word.ToUpper()[0], word.ToLower());
            string siteData = webWrapper.GetStringRepresentationOfSite(url);
            Regex rx = new Regex(@"<div\ class=""entry"">.+<b>.*</b>:<i>\ (?<words>.*)</i><br\ />", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return rx.Match(siteData).Result("${words}").Split(new Char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);//catch error
            }
        }
    }