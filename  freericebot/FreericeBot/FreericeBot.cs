using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FreericeBot
{
    public class FreericeBot
    {
        public delegate void StatusUpdateHandler(string message);
        public event StatusUpdateHandler statusUpdate;

        private readonly Thread botLoopThread;

        public FreericeBot()
            {
            botLoopThread = new Thread(FreericeBot.BotLoop);
            }

        public void Start()
            {
            botLoopThread.Start(statusUpdate);
            }

        public void Stop()
            {
            botLoopThread.Abort();
            }

        private static void BotLoop(object _statusUpdate)
            {
            StatusUpdateHandler statusUpdate = (StatusUpdateHandler) _statusUpdate;
            Freerice_Site_Interaction freeRiceInteraction = new Freerice_Site_Interaction();

            while(true)
                {
                FreericeSiteData data = new FreericeSiteData();
                try
                    {
                    data = freeRiceInteraction.GetSiteData();
                    statusUpdate(data.Word);
                    }
                catch(ErrorLoadingFreerice s)
                    {
                    freeRiceInteraction.ResetSite();
                    statusUpdate("Error loading site " + s);
                    Thread.Sleep(5000);
                    continue;
                    }

                int answer = SearchThesaurusForAnswer(data.Word, data.Answers);
                if(answer == -1)
                    {
                    statusUpdate("Unable to find correct word so we guess");
                    Random rand = new Random();
                    answer = rand.Next(0, 3);
                    }
                else
                    {
                        statusUpdate("Found answer: " + data.Answers[answer]);
                    }
                

                FreericeAnswerData answerData = new FreericeAnswerData();
                while (true)
                    {
                    try
                        {
                        answerData = freeRiceInteraction.SubmitAnswer(data, answer);
                        statusUpdate(answerData.IsAnswerCorrect + " " + data.riceDonated);
                        break;
                        }
                    catch (ErrorLoadingFreerice s)
                        {
                        statusUpdate("Error sending answer " + s);
                        }
                    }
                }  
            }
        private static int SearchThesaurusForAnswer(string word, string[] answers)
            {
            string[] thesaurusWords = Thesaurus_Site_Interaction.GetSynonymsOfWord(word);

            for (int i = 0; i < answers.Length; i++)
                {
                if (Array.BinarySearch(thesaurusWords, answers[i]) >= 0)// broken
                    {
                    //we found it
                    return i;
                    }
                }
            return -1;
            }
    }
}
