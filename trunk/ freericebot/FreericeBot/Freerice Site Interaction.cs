///
/// Author: Aron J
/// Date: 08/05/08

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace FreericeBot
    {
    /// <summary>
    /// Exposes a minimal set of functions to receive only nessary information from freerice.org 
    /// </summary>
    static class Freerice_Site_Interaction
        {

        string GetLookupWord()
            {
            throw new NotImplementedException();
            }

        string[] GetPossibleAnswers() //TODO: change string array to a class for readablity
            {
            throw new NotImplementedException();
            }
        
        int GetRiceDonated()
            {
            throw new NotImplementedException();
            }

        void SubmitAnswer(Int16 choice)
            {
            if(choice < 0 || choice > 3)
                {
                throw new ConstraintException("Invalid range of " + choice);
                }

            throw new NotImplementedException();
            }

        bool IsAnswerCorrect()
            {
            throw new NotImplementedException();
            }

        string GetCorrectWordSynonym()
            {
            throw new NotImplementedException();
            }

        }
    }
