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
    public enum FreericeSiteStatus
            {
            AtMainPage,
            AtCorrectAnswerPage,
            AtIncorrectAnswerPage,
            AtSiteToSlowPage,
            UnknownLocation
            }

    class ErrorLoadingFreerice : Exception
        {
        public readonly FreericeSiteStatus e;
        public ErrorLoadingFreerice(FreericeSiteStatus status)
            {
            e = status;
            }
        }


    /// <summary>
    /// Exposes a minimal set of functions to receive only nessary information from freerice.org 
    /// </summary>
    public class Freerice_Site_Interaction
        {

        public struct FreericeSiteData
            {

            public string Word;
            public string[] Answers;
            public int riceDonated;
            public FreericeSiteStatus siteStatus;
            public string answerPostData;
            } ;

        public struct FreericeAnswerData
            {
            public bool IsAnswerCorrect;
            public string word;
            public string Synonym;
            }

        private const string FreericeUrl = "http://www.freerice.com/index.php";
        private readonly WebRequestWrapper webRequest = new WebRequestWrapper();
        private string siteData = string.Empty;

        public FreericeSiteData GetSiteData()
            {
            if(!IsSiteLoaded())
                {
                LoadSite();
                }

            FreericeSiteStatus siteStatus = GetSiteStatus();
            if(siteStatus == FreericeSiteStatus.UnknownLocation || siteStatus == FreericeSiteStatus.AtSiteToSlowPage)
                {
                throw new ErrorLoadingFreerice(siteStatus);
                }

            FreericeSiteData data = new FreericeSiteData();
            data.Word = GetLookupWord();
            data.Answers = GetPossibleAnswers();
            data.riceDonated = GetRiceDonated();
            data.answerPostData = GetAnswerPostData();
            data.siteStatus = siteStatus;
            return data;
            }

        public FreericeAnswerData SubmitAnswer(FreericeSiteData data, int choice)
            {
            if (choice < 0 || choice > 3)
            {
                throw new ConstraintException("Invalid range of " + choice);
            }

            siteData = webRequest.GetStringRepresentationOfSite(FreericeUrl, "POST", "SELECTED=" + (choice + 1) + data.answerPostData);

            FreericeSiteStatus siteStatus = GetSiteStatus();
            if (siteStatus == FreericeSiteStatus.UnknownLocation || siteStatus == FreericeSiteStatus.AtSiteToSlowPage)
            {
                throw new ErrorLoadingFreerice(siteStatus);
            }

            FreericeAnswerData answerData = new FreericeAnswerData();
            answerData.IsAnswerCorrect = IsAnswerCorrect();
            answerData.word = data.Word;
            answerData.Synonym = GetCorrectWordSynonym();

            return answerData;
            }

        private string GetAnswerPostData()
            {
                Regex rx =
                    new Regex(
                        @"<input\ type=hidden\ name=""PAST""\ value=""(?<past>\w*)""\ />.*
                    <input\ type=hidden\ name=""INFO""\ value=(?<info>\d+)\ />.*
                    <input\ type=hidden\ name=""INFO2""\ value=(?<info2>\d+)\ />.*
                    <input\ type=hidden\ name=""INFO3""\ value=""(?<info3>[-&#8217; '\w\s]+)""\ />",
                        RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

                string results =
                    rx.Match(siteData).Result(
                              "PAST=${past}&"
                            + "INFO=${info}&"
                            + "INFO2=${info2}&"
                            + "INFO3=${info3}&");

                return results;
            }
        private string GetLookupWord()
            {
            Regex rx = new Regex(@"<li><strong>(?<word>[-&#8217; '\w\s]+)</strong>\ means:</li>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rx.Match(siteData).Result("${word}");
            }

        private string[] GetPossibleAnswers()
            {
            ArrayList answers = new ArrayList();

            Regex rx = new Regex(@"<li><noscript><input\ type=""submit""\ name=""SELECTED""\ value=""\ \d\ "">\ </noscript><a\ href=""javascript:\ submitForm\('\d'\)"">(?<answer>[-&#8217; '\w\s]+)</a></li>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            for (Match m = rx.Match(siteData); m.Success; m = m.NextMatch())
                {
                    answers.Add(m.Result("${answer}"));
                }

            return (string[]) answers.ToArray(typeof(string));
            }

        private int GetRiceDonated()
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

        private bool IsAnswerCorrect()
            {
            Regex rx = new Regex(@"<div\ id=""correct"">CORRECT!</div><div\ id=""correctDef"">.+</div>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rx.IsMatch(siteData);
            }

        private string GetCorrectWordSynonym()
            {
            Regex rx = new Regex(@"<div\ id=""(correct|incorrect)"">(CORRECT!</div><div\ id=""correctDef"">.+=\ |.+=)\ (?<match>[-&#8217; '\w\s]+[-&#8217; '\w\s]+)</div>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rx.Match(siteData).Result("${match}");
            }

        
        public void ResetSite()
            {
            siteData = webRequest.GetStringRepresentationOfSite(FreericeUrl);
            }

        private void LoadSite()
            {
            ResetSite();
            }

        private bool IsSiteLoaded()
            {
            if (siteData != string.Empty)
                {
                return false;
                }
            return true;
            }

        private FreericeSiteStatus GetSiteStatus()
            {
            const RegexOptions regexOptions =
                RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

            const string AtMainPagePattren = @"<h2\ id=""tHowToPlay"">How\ to\ play</h2>";
            const string AtCorrectAnswerPagePattren = @"<div\ id=""correct"">";
            const string AtIncorrectAnswerPagePattren = @"<div\ id=""incorrect"">";
            const string AtSiteToSlowPagePattren =
                @"<div\ id=""errorDisplay"">.*<p>Sorry,\ we\ are\ unable\ to\ process\ rice\ donations\ so\ fast\.</p>";

            if (Regex.IsMatch(siteData, AtMainPagePattren, regexOptions))
                {
                return FreericeSiteStatus.AtMainPage;
                }

            if (Regex.IsMatch(siteData, AtCorrectAnswerPagePattren, regexOptions))
                {
                return FreericeSiteStatus.AtCorrectAnswerPage;
                }

            if (Regex.IsMatch(siteData, AtIncorrectAnswerPagePattren, regexOptions))
                {
                return FreericeSiteStatus.AtIncorrectAnswerPage;
                }

            if(Regex.IsMatch(siteData, AtSiteToSlowPagePattren, regexOptions))
                {
                return FreericeSiteStatus.AtSiteToSlowPage;
                }

            return FreericeSiteStatus.UnknownLocation;
            }
        }
    }
