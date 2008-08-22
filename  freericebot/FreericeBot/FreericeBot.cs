using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FreericeBot
{
    class FreericeBot
    {
        public delegate void StatusUpdateHandler(string message);
        public event StatusUpdateHandler statusUpdate;

        private readonly Thread botLoopThread;

        FreericeBot()
            {
            botLoopThread = new Thread(FreericeBot.BotLoop);
            }

        public void Start()
            {
            botLoopThread.Start();
            }

        public void Stop()
            {
            botLoopThread.Abort();
            }

        private static void BotLoop()
            {
            Freerice_Site_Interaction freeRiceInteraction = new Freerice_Site_Interaction();

            while(true)
                {
                FreericeSiteData data = new FreericeSiteData();
                try
                    {
                     data = freeRiceInteraction.GetSiteData();
                    }
                catch(ErrorLoadingFreerice s)
                    {
                    continue;
                    }

                FreericeAnswerData answerData = new FreericeAnswerData();
                try
                    {
                    answerData = freeRiceInteraction.SubmitAnswer(data, 1);
                    }
                catch(ErrorLoadingFreerice s)
                    {
                    
                    }
                }  
            }
    }
}
