///
/// Author: Aron J
/// Date: 08/05/08

using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace FreericeBot
    {
    class SiteNotFastEnough : Exception
        {
        }
    /// <summary>
    /// Exposes a minimal set of functions to receive only nessary information from freerice.org 
    /// </summary>
    static class Freerice_Site_Interaction
        {
        private const string FreericeUrl = "http://www.freerice.com/index.php";
        private static readonly WebRequestWrapper webRequest = new WebRequestWrapper();
        private static string siteData = string.Empty;

        static Freerice_Site_Interaction()
            {
            //gets the inital site data
            ReloadSite();
            }

        public static string GetLookupWord()
            {
            Regex rx = new Regex(@"<li><strong>(?<word>[-&#8217; '\w\s]+)</strong>\ means:</li>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rx.Match(siteData).Result("${word}");
            }

        public static string[] GetPossibleAnswers() //TODO: change string array to a class for readablity
            {
            ArrayList answers = new ArrayList();

            Regex rx = new Regex(@"<li><noscript><input\ type=""submit""\ name=""SELECTED""\ value=""\ \d\ "">\ </noscript><a\ href=""javascript:\ submitForm\('\d'\)"">(?<answer>[-&#8217; '\w\s]+)</a></li>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            for (Match m = rx.Match(siteData); m.Success; m = m.NextMatch())
                {
                    answers.Add(m.Result("${answer}"));
                }

            return (string[]) answers.ToArray(typeof(string));
            }

        public static int GetRiceDonated()
            {
            Regex rx = new Regex(@"(?<riceDonated>\d+)\ grains\ of\ rice\.</p>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            try
                {
                return int.Parse(rx.Match(siteData).Result("${riceDonated}"));
                }
            catch(NotSupportedException)
                {
                return 0;
                }
            }

        public static void SubmitAnswer(int choice)
            {
            if(choice < 0 || choice > 3)
                {
                throw new ConstraintException("Invalid range of " + choice);
                }

            Regex rx =
                new Regex(
                    @"<input\ type=hidden\ name=""PAST""\ value=""(?<past>\w*)""\ />.*
                    <input\ type=hidden\ name=""INFO""\ value=(?<info>\d+)\ />.*
                    <input\ type=hidden\ name=""INFO2""\ value=(?<info2>\d+)\ />.*
                    <input\ type=hidden\ name=""INFO3""\ value=""(?<info3>[-&#8217; '\w\s]+)""\ />", 
		            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace| RegexOptions.Compiled);

            string results =
                rx.Match(siteData).Result(
                        "SELECTED=" + (choice + 1)
                        +"PAST=${past}&"
                        +"INFO=${info}&"
                        +"INFO2=${info2}&"
                        +"INFO3=${info3}&");

            siteData = webRequest.GetStringRepresentationOfSite(FreericeUrl, "POST", results);
            }

        public static bool IsAnswerCorrect()
            {
            Regex rx = new Regex(@"<div\ id=""correct"">CORRECT!</div><div\ id=""correctDef"">.+</div>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rx.IsMatch(siteData);
            }

        public static string GetCorrectWordSynonym()
            {
            Regex rx = new Regex(@"<div\ id=""(correct|incorrect)"">(CORRECT!</div><div\ id=""correctDef"">.+=\ |.+=)\ (?<match>[-&#8217; '\w\s]+[-&#8217; '\w\s]+)</div>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rx.Match(siteData).Result("${match}");
            }

        public static void ReloadSite()
            {
            siteData = webRequest.GetStringRepresentationOfSite(FreericeUrl);

            Regex rx =
                new Regex(
                    @"<div\ id=""errorDisplay"">.*
                    <p>Sorry,\ we\ are\ unable\ to\ process\ rice\ donations\ so\ fast\.</p>", 
		            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace| RegexOptions.Compiled);

            if(rx.IsMatch(siteData))
                {
                throw new SiteNotFastEnough();
                }
            
            }

        }
    }
