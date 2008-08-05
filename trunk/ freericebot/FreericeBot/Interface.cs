using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FreericeBot
{
    public partial class Interface : Form
    {
        public Interface()
        {
            InitializeComponent();

            for (int i = 0; i < 2; i++ )
                {
                Console.AppendText(Freerice_Site_Interaction.GetLookupWord() + "\r\n\t");

                foreach (string s in Freerice_Site_Interaction.GetPossibleAnswers())
                    {
                    Console.AppendText("\"" + s + "\" ");
                    }
                Console.AppendText("\r\n");
                Console.AppendText("\r\n");
                Console.AppendText("\r\n");

                Random r = new Random();
                Freerice_Site_Interaction.SubmitAnswer(r.Next(0,3));

                Console.AppendText(
                    Freerice_Site_Interaction.IsAnswerCorrect() ? "Correct answer" : "Incorrect answer");

                Console.AppendText("\r\n");

                Console.AppendText(Freerice_Site_Interaction.GetCorrectWordSynonym() + "\r\n");

                Console.AppendText("Rice donated: " + Freerice_Site_Interaction.GetRiceDonated() + "\r\n");
                //Freerice_Site_Interaction.ReloadSite();
                }
            
        }
    }
}
