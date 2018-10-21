using System;
using System.Collections.Generic;

namespace rokhan.inc
{
    //----------------------------------------------------------------------
    // Persian Log2Vis version 2
    //----------------------------------------------------------------------
    // Copyright (c) 2012 Oxygen Web Solutions <http://oxygenws.com>
    //----------------------------------------------------------------------
    // This program is under the terms of the GENERAL PUBLIC LICENSE (GPL)
    // as published by the FREE SOFTWARE FOUNDATION. The GPL is available
    // through the world-wide-web at http://www.gnu.org/copyleft/gpl.html
    //----------------------------------------------------------------------
    // Authors: Omid Mottaghi Rad <webmaster@oxygenws.com>
    // Thanks to TCPDF project @ http://www.tecnick.com/
    //----------------------------------------------------------------------

    /**
     * A function to change persian or arabic text from its logical condition to visual
     *
     * @author        Omid Mottaghi Rad
     * @param        string    Main text you want to change it
     * @param        boolean    Apply e'raab characters or not? default is true
     * @param        boolean    Which encoding? default it "utf8"
     * @param        boolean    Do you want to change special characters like "allah" or "lam+alef" or "lam+hamza", default is true
     */
    /**
    * converted to c# by peyman farahmand(pfndesigen@gmail.com)
    * 
    * 2018/10/12
    */
    public struct logvis
    {
        public string word;
        public string chars;
        public string flag;
        public logvis(string word, string chars, string flag = "none")
        {
            this.word = word;
            this.chars = chars;
            this.flag = flag;
        }
    }
    class persian_log2vis
    {
        public Dictionary<int, logvis> setup(string str)
        {
            bidi bidi = new bidi();

            string[] lines = str.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            // modified for awr
            int[] chars;
            Dictionary<int, logvis> str_ar = new Dictionary<int, logvis>();
            foreach (string text in lines)
            {
                chars = bidi.Utf8Bidi(bidi.UTF8StringToArray(str), "AL");

                if (bidi.ContainsArabicCharacter(str))
                    Array.Reverse(chars);

                int key = 0;
                foreach (int cha in chars)
                {
                    logvis lg = new logvis(bidi.unichr(cha), bidi.unichr(cha));
                    str_ar.Add(key, lg);
                    key++;
                }

            }
            return str_ar;
        }
    }
}
