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
            Random r = new Random();

            for (int i = 0; i < 2; i++ )
                {
                string word = Freerice_Site_Interaction.GetLookupWord();
                Console.AppendText("Word: " + word + "\r\n");

                Console.AppendText("\tAnswers: ");
                string[] answers = Freerice_Site_Interaction.GetPossibleAnswers();
                foreach (string s in answers)
                    {
                    Console.AppendText("\""+ s + "\" ");
                    }
                Console.AppendText("\r\n");

                Console.AppendText("\tSynonyms: ");
                string[] synonyms = Thesaurus_Site_Interaction.GetSynonymsOfWord(word);
                foreach (string s in synonyms)
                {
                    Console.AppendText("\"" + s + "\" ");
                }
                
                Freerice_Site_Interaction.SubmitAnswer(r.Next(0,3));

                Console.AppendText(
                    Freerice_Site_Interaction.IsAnswerCorrect() ? "Correct answer" : "Incorrect answer");

                Console.AppendText("\r\n");

                Console.AppendText(Freerice_Site_Interaction.GetCorrectWordSynonym() + "\r\n");

                Console.AppendText("Rice donated: " + Freerice_Site_Interaction.GetRiceDonated() + "\r\n");

                Console.AppendText("\r\n");
                Console.AppendText("\r\n");
                Console.AppendText("\r\n");
                Console.AppendText("\r\n");
                //Freerice_Site_Interaction.ReloadSite();
                }
            
        }
    }
}
