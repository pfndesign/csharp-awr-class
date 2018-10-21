using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

/**
 * [awr_process description]
 * ver 2.0
 * awr_process is a advanced word rule processor especially for arabic/quran
 * author  : peyman farahmand
 * email : pfndesigen@gmail.com
 * date : 14/10/2018
 */

/**
 * [testing description]
 * fully checked surah list :
 * an-nas
 * al-falaq
 * al-ikhlas
 * al-masadd
 * al-nasr
 * al-kafiroon
 * al-kauther
 * al-maun
 * al-quraish
 * al-fil
 * al-humaza
 * al-asr
 * al-takathur
 * al-qaria
 * al-adiyat
 * al-zalzala
 * al-bayyina
 * al-qadr
 * al-alaq
 * at-tin
 * al-inshirah
 * at this point, I"m certain that everything is working
 * reminder: for tajvid rules to work correctly all words must have a correct Arabic erab.
 */

namespace rokhan.inc
{
    public delegate void processHandler(object source, ProcessEventArgs e);
    class awr_process
    {
        public string text;

        private Dictionary<int, Dictionary<int, logvis>> text_ready;

        public Dictionary<String, SolidColorBrush> colors = new Dictionary<string, SolidColorBrush>{
                {"none" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000")) },
                {"chunna" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6600")) },
                {"ikhfaa" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#CC0000")) },
                {"qalqala" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#00CC00")) },
                {"lqlab" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#6699FF")) },
                {"idghamwg" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#BBBBBB")) },
                {"idgham" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#9900CC")) },
                {"maddah" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#34495e")) },
            };
        public awr_process(string text)
        {
            this.text = text;
            Ready();
        }
        /**
         * [ready description]
         * splitting text to words and words to characters
         * converting each character to Unicode character to keeping there original form in the word
         * @return [type] array [description] unicode characters from the word
         */
        private void Ready()
        {
            string[] verses = this.text.Split(new char[0]);
            verses = verses.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            Dictionary<int, Dictionary<int, logvis>> verses_ar = new Dictionary<int, Dictionary<int, logvis>>();
            int key = 0;
            persian_log2vis persian_log2vis = new persian_log2vis();

            foreach (string verse in verses)
            {
                Dictionary<int, logvis> list = persian_log2vis.setup(verse);
                verses_ar.Add(key, list);
                key++;
            }
            text_ready = verses_ar;
        }
        /**
         * [register_filter description]
         * register filters/rules to the process function
         * default rule functions :
         * filter_qalqala
         * filter_ghunna
         * filter_lqlab
         * filter_ikhfaa
         * filter_idgham
         * filter_idgham_without_ghunna
         * filter_maddah
         * @param  [type] function_name [description] name of the rule functions
         */
        public event processHandler Process_event;
        public virtual void OnProcess(ProcessEventArgs e)
        {
            if (Process_event != null)
                Process_event(this, e);
        }
        /**
         * [process description]
         * calls each registered filter/rule function to process each character
         */
        public void Process()
        {
            ProcessEventArgs data = new ProcessEventArgs();
            foreach (KeyValuePair<int, Dictionary<int, logvis>> verse in text_ready)
            {
                foreach (KeyValuePair<int, logvis> chars in verse.Value.ToList())
                {
                    data.Key1 = verse.Key;
                    data.Key2 = chars.Key;
                    OnProcess(data);
                }
            }
        }
        /**
         * [set_word_flag description]
         * set the flag for the charecters
         * @param [type] key1 [description] the position of the character in array / row
         * @param [type] key2 [description] the position of the character in array / column
         * @param [type] flag [description] rule flag name
         */
        private void Set_word_flag(int key1, int key2, string flag)
        {
            text_ready[key1][key2] = new logvis(text_ready[key1][key2].chars, text_ready[key1][key2].word, flag);
        }
        /**
         * [wordrule_applyer description]
         * applying/checking the word rules for characters
         * @param  [type]  wordkey          [description] the position of the character in array / row
         * @param  [type]  key              [description] the position of the character in array / column
         * @param  [type]  tag              [description] rule flag name
         * @param  [type]  words            [description] array of charecters
         * @param  boolean attachedby       [description] false or array of characters that must be attached to words
         * @param  boolean followedby       [description] false or array of characters that must be followed by words / attachedby if not false
         * @param  boolean followedbyattach [description] false or array of characters that must be attached to followedby if not false
         * @param  boolean lastlettercheck  [description] false or check if the previous rules are for a character at end of the text
         * @param  boolean erab_flag        [description] true if you don"t want to use alef as erab
         * @return [type]                    [description] if the rule applies for character
         */
        private bool Wordrule_applyer(int wordkey, int key, string tag, String[] words, String[] attachedby = null, String[] followedby = null, String[] followedbyattach = null, bool lastlettercheck = false, bool erab_flag = false)
        {

            List<int[]> path = new List<int[]>();
            int wordkey1 = wordkey;
            int wordkey2 = wordkey;
            int wordkey3 = wordkey;
            int wordkey4 = wordkey;
            int key1 = key + 1;
            int key2 = key + 2;
            int key3 = key + 3;
            int key4 = key + 4;
            int index = 0;
            string sourcetext_plus = "";
            string sourcetext_plustwo = "";
            string sourcetext_plusthree = "";
            string sourcetext_plusfour = "";
            
            string sourcetext = text_ready[wordkey][key].chars;
            if (text_ready.ContainsKey(wordkey1) && text_ready[wordkey1].ContainsKey(key1))
            {
                sourcetext_plus = text_ready[wordkey1][key1].chars;
            }
            else if (text_ready.ContainsKey((wordkey1 + 1)) && text_ready[(wordkey1 + 1)].ContainsKey(index))
            {
                wordkey1 = wordkey1 + 1;
                key1 = index;
                index++;
                sourcetext_plus = text_ready[wordkey1][key1].chars;
            }

            if (text_ready.ContainsKey(wordkey2) && text_ready[wordkey2].ContainsKey(key2))
            {
                sourcetext_plustwo = text_ready[wordkey2][key2].chars;
            }
            else if (text_ready.ContainsKey((wordkey2 + 1)) && text_ready[(wordkey2 + 1)].ContainsKey(index))
            {
                wordkey2 = wordkey2 + 1;
                key2 = index;
                index++;
                sourcetext_plustwo = text_ready[wordkey2][key2].chars;
            }

            if (text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3))
            {
                sourcetext_plusthree = text_ready[wordkey3][key3].chars;
            }
            else if (text_ready.ContainsKey((wordkey3 + 1)) && text_ready[(wordkey3 + 1)].ContainsKey(index))
            {
                wordkey3 = wordkey3 + 1;
                key3 = index;
                index++;
                sourcetext_plusthree = text_ready[wordkey3][key3].chars;
            }
            if (text_ready.ContainsKey(wordkey4) && text_ready[wordkey4].ContainsKey(key4))
            {
                sourcetext_plusfour = text_ready[wordkey4][key4].chars;
            }
            else if (text_ready.ContainsKey((wordkey4 + 1)) && text_ready[(wordkey4 + 1)].ContainsKey(index))
            {
                wordkey4 = wordkey4 + 1;
                key4 = index;
                index++;
                sourcetext_plusfour = text_ready[wordkey4][key4].chars;
            }

            if (!words.Contains(sourcetext))
            {
                return false;
            }
            /**
             * [if description]
             * check attachedby
             * attachedby can have a array of characters or especially "erab" or "!erab" to check for earb!! duh
             */
            if (attachedby != null && text_ready.ContainsKey(wordkey1) && text_ready[wordkey1].ContainsKey(key1))
            {
                if (attachedby.Contains("erab") && !Erab(text_ready[wordkey1][key1].chars, erab_flag))
                {
                    return false;
                }
                else if (attachedby.Contains("erab") && Erab(text_ready[wordkey1][key1].chars, erab_flag))
                {
                    path.Add(new int[] { wordkey1, key1 });
                }
                else if (attachedby.Contains("!erab") && Erab(text_ready[wordkey1][key1].chars, erab_flag))
                {
                    return false;
                }
                else if (attachedby.Contains("!erab") && !Erab(text_ready[wordkey1][key1].chars, erab_flag))
                {
                    path.Add(new int[] { wordkey1, key1 });
                }
                if (((!attachedby.Contains("erab") && !attachedby.Contains("!erab")) || attachedby.Length > 1) && !attachedby.Contains(sourcetext_plus))
                {
                    return false;
                }
                else if (((!attachedby.Contains("erab") && !attachedby.Contains("!erab")) || attachedby.Length > 1) && attachedby.Contains(sourcetext_plus))
                {
                    path.Add(new int[] { wordkey1, key1 });
                }
            }
            else if (attachedby != null && (!text_ready.ContainsKey(wordkey1) || !text_ready[wordkey1].ContainsKey(key1)))
            {
                return false;
            }
            /**
             * [if description]
             * check followedby
             * followedby can have a array of characters
             */
            if (followedby != null && ((text_ready.ContainsKey(wordkey2) && text_ready[wordkey2].ContainsKey(key2)) || (text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3))))
            {
                if ((text_ready.ContainsKey(wordkey2) && text_ready[wordkey2].ContainsKey(key2)) && (!text_ready.ContainsKey(wordkey3) || !text_ready[wordkey3].ContainsKey(key3)) && !followedby.Contains(sourcetext_plustwo)
                    ||
                    ((text_ready.ContainsKey(wordkey2) && text_ready[wordkey2].ContainsKey(key2)) && (text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) && !Erab(text_ready[wordkey2][key2].chars, erab_flag) && !followedby.Contains(sourcetext_plustwo))
                    ||
                    ((text_ready.ContainsKey(wordkey2) && text_ready[wordkey2].ContainsKey(key2)) && (text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) && Erab(text_ready[wordkey2][key2].chars, erab_flag) && !followedby.Contains(sourcetext_plusthree))
                    )
                {
                    return false;
                }
                else if ((text_ready.ContainsKey(wordkey2) && text_ready[wordkey2].ContainsKey(key2)) && (text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) && Erab(text_ready[wordkey2][key2].chars, erab_flag) && followedby.Contains(sourcetext_plusthree))
                {
                    path.Add(new int[] { wordkey1, key1 });
                    path.Add(new int[] { wordkey2, key2 });
                    path.Add(new int[] { wordkey3, key3 });
                }
                else if ((text_ready.ContainsKey(wordkey2) && text_ready[wordkey2].ContainsKey(key2)) && followedby.Contains(sourcetext_plustwo))
                {
                    path.Add(new int[] { wordkey1, key1 });
                    path.Add(new int[] { wordkey2, key2 });
                }
            }
            else if (followedby != null && (!text_ready.ContainsKey(wordkey2) || !text_ready[wordkey2].ContainsKey(key2)))
            {
                return false;
            }
            /**
             * [if description]
             * check followedbyattach if followedby is true
             * followedbyattach can have a array of characters
             */
            if (followedby != null && followedbyattach != null && ((text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) || (text_ready.ContainsKey(wordkey4) && text_ready[wordkey4].ContainsKey(key4))))
            {
                if ((text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) && (!text_ready.ContainsKey(wordkey4) || !text_ready[wordkey4].ContainsKey(key4)) && !followedbyattach.Contains(sourcetext_plusthree)
                    ||
                    ((text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) && (text_ready.ContainsKey(wordkey4) && text_ready[wordkey4].ContainsKey(key4)) && !Erab(text_ready[wordkey3][key3].chars, erab_flag) && !followedbyattach.Contains(sourcetext_plusthree))
                    ||
                    ((text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) && (text_ready.ContainsKey(wordkey4) && text_ready[wordkey4].ContainsKey(key4)) && Erab(text_ready[wordkey3][key3].chars, erab_flag) && !followedbyattach.Contains(sourcetext_plusfour))
                    )
                {
                    return false;
                }
                else if ((text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) && (text_ready.ContainsKey(wordkey4) && text_ready[wordkey4].ContainsKey(key4)) && Erab(text_ready[wordkey3][key3].chars, erab_flag) && followedbyattach.Contains(sourcetext_plusfour))
                {
                    path.Add(new int[] { wordkey3, key3 });
                    path.Add(new int[] { wordkey4, key4 });
                }
                else if ((text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) && followedbyattach.Contains(sourcetext_plusthree))
                {
                    path.Add(new int[] { wordkey3, key3 });
                }
            }
            else if (followedby != null && followedbyattach != null && (!text_ready.ContainsKey(wordkey3) || !text_ready[wordkey3].ContainsKey(key3)))
            {
                return false;
            }

            /**
             * [if description]
             * check if the character is at end of the text
             */
            if (lastlettercheck && (text_ready.ContainsKey((wordkey + 1)) ||
                    ((text_ready.ContainsKey(wordkey1) && text_ready[wordkey1].ContainsKey(key1)) && (!text_ready.ContainsKey(wordkey2) || !text_ready[wordkey2].ContainsKey(key2)) && Erab(text_ready[wordkey1][key1].chars, erab_flag) && (text_ready[wordkey1].Count - 1) != key1)
                    ||
                    ((text_ready.ContainsKey(wordkey1) && text_ready[wordkey1].ContainsKey(key1)) && (text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) && (text_ready.ContainsKey(wordkey2) && text_ready[wordkey2].ContainsKey(key2)) && Erab(text_ready[wordkey1][key1].chars, erab_flag) && Erab(text_ready[wordkey2][key2].chars, erab_flag) && (text_ready[wordkey1].Count - 1) != key2)
                    ||
                    ((text_ready.ContainsKey(wordkey1) && text_ready[wordkey1].ContainsKey(key1)) && (text_ready.ContainsKey(wordkey3) && text_ready[wordkey3].ContainsKey(key3)) && (text_ready.ContainsKey(wordkey2) && text_ready[wordkey2].ContainsKey(key2)) && Erab(text_ready[wordkey1][key1].chars, erab_flag) && !Erab(text_ready[wordkey2][key2].chars, erab_flag))
                    ))
            {
                return false;
            }

            path.Add(new int[] { wordkey, key });

            /**
             * [foreach description]
             * applying the flag to the characters in path
             */
            foreach (int[] wkey in path)
            {
                Set_word_flag(wkey[0], wkey[1], tag);
            }

            return true;
        }
        /**
         * [erab description]
         * check if the character is erab
         * @param  [type] text      [description] character
         * @param  [type] erab_flag [description] use without alef if true
         * @return [type] bool
         */
        public static string StringToHex(string hexstring)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char t in hexstring)
            {
                //Note: X for upper, x for lower case letters
                sb.Append(Convert.ToInt32(t).ToString("x"));
            }
            return sb.ToString();
        }
       
        public bool Erab(string text, bool erab_flag = false)
        {
            string[] erablist = new string[] {
        "64b",
        "64c",
        "64e",
        "64f",
        "650",
        "651",
        "652",
        "653", //madhe
        "654", //zameh
        "656", //alefkochik
        "657", //dammah
        "658", //noon ghunna
        "659", //zwarakay
        "65a", //vowel
        "65b", //vowel
        "65d", //reversed damma
        "670",
        "64d",
        "fe8e", //alef
        "fe8d",
        "fefb", // la
        };
            if (erab_flag)
            {
                string[] alef = new string[] { "fe8e", "fe8d" };
                erablist = erablist.Except(alef).ToArray();
            }

            text = StringToHex(text);

            if (erablist.Contains(text))
            {
                return true;
            }

            return false;
        }
        /**
         * [filter_qalqala description]
         * check tajvid qalqala rules
         * @param  [type] key1 [description] the position of the character in array / row
         * @param  [type] key2 [description] the position of the character in array / column
         * @return [type] bool
         */
        public void Filter_qalqala(object source, ProcessEventArgs e)
        {
            string[] charecters = new string[] {
      "\u0642",
      "\ufed5",
      "\ufed6", //ق
      "\ufed7",
      "\ufed8",
      "\u0637",
      "\ufec1",
      "\ufec2", //ط
      "\ufec3",
      "\ufec4",
      "\u0628",
      "\ufe8f",
      "\ufe90", //ب
      "\ufe91",
      "\ufe92",
      "\u062c",
      "\ufe9d",
      "\ufe9e", //ج
      "\ufea0",
      "\ufe9f",
      "\u062f",
      "\ufea9", //د
      "\ufeaa"
      };
            string[] sukun = new string[1] { "\u0652" };
            bool rule1 = Wordrule_applyer(e.Key1, e.Key2, "qalqala", charecters, sukun);
            bool rule2 = Wordrule_applyer(e.Key1, e.Key2, "qalqala", charecters, null, null, null, true, true);
            /*
            some of Quran surah that I checked manually for this filter :
            falaq,ikhlas,masadd,nasr,kafiroon,kauther
             */
            //return (rule1 || rule2);
        }
        /**
         * [filter_ghunna description]
         * check tajvid ghunna rules
         * @param  [type] key1 [description] the position of the character in array / row
         * @param  [type] key2 [description] the position of the character in array / column
         * @return [type] bool
         */
        public void Filter_ghunna(object source, ProcessEventArgs e)
        {
            string[] m = new string[] {
          "\u0645",
          "\ufee1",
          "\ufee2", //م
          "\ufee4",
          "\ufee3"
        };
            string[] n = new string[] {
          "\u0646",
          "\ufee5",
          "\ufee6", //ن
          "\ufee8",
          "\ufee7"
            };
            string[] tasdid = new string[1] { "\u0651" };
            bool rule1 = Wordrule_applyer(e.Key1, e.Key2, "chunna", m, tasdid, null, null, false, true);
            bool rule2 = Wordrule_applyer(e.Key1, e.Key2, "chunna", n, tasdid, null, null, false, true);
            /*
            some of Quran surah that I checked manually for this filter :
            nas,falaq,masadd,nasr,kauther
             */
            //return (rule1 || rule2);
        }
        /**
         * [filter_lqlab description]
         * check tajvid lqlab rules
         * @param  [type] key1 [description] the position of the character in array / row
         * @param  [type] key2 [description] the position of the character in array / column
         * @return [type] bool
         */
        public void Filter_lqlab(object source, ProcessEventArgs e)
        {
            string[] erabha = new string[] {
        "\u064b",
        "\u064d",
        "\u064c"
            };

            string[] n = new string[] {
        "\u0646",
        "\ufee5",
        "\ufee6", //ن
        "\ufee8",
        "\ufee7"
            };

            string[] b = new string[] {
        "\u0628",
        "\ufe8f",
        "\ufe90", //ب
        "\ufe91",
        "\ufe92"
            };
            string[] sukon = new string[1] { "\u0652" };

            bool rule1 = Wordrule_applyer(e.Key1, e.Key2, "lqlab", erabha, b);
            bool rule2 = Wordrule_applyer(e.Key1, e.Key2, "lqlab", erabha, new string[1] { "erab" }, b);
            bool rule3 = Wordrule_applyer(e.Key1, e.Key2, "lqlab", n, sukon, b);
            bool rule4 = Wordrule_applyer(e.Key1, e.Key2, "lqlab", n, b);
            /*
            some of Quran surah that I checked manually for this filter :
            baqara:10,18,19,27,31,33
             */
            //return (rule1 || rule2 || rule3 || rule4);
        }
        /**
         * [filter_ikhfaa description]
         * check tajvid ikhfaa rules
         * @param  [type] key1 [description] the position of the character in array / row
         * @param  [type] key2 [description] the position of the character in array / column
         * @return [type] bool
         */
        public void Filter_ikhfaa(object source, ProcessEventArgs e)
        {
            string[] theseletter = new string[] {
        "\u062a",
        "\ufe95",
        "\ufe96", //ت
        "\ufe97",
        "\ufe98",
        "\ufe99",
        "\u062b",
        "\ufe99",
        "\ufe9a", //ث
        "\ufe9c",
        "\ufe9b",
        "\u062c",
        "\ufe9d",
        "\ufe9e", //ج
        "\ufea0",
        "\ufe9f",
        "\u062f",
        "\ufea9", //د
        "\ufeaa",
        "\u0630",
        "\ufeab", //ذ
        "\ufeac",
        "\u0632",
        "\ufeaf", //ز
        "\ufeb0",
        "\u0633",
        "\ufeb1",
        "\ufeb2", //س
        "\ufeb3",
        "\ufeb4",
        "\u0634",
        "\ufeb5",
        "\ufeb6", //ش
        "\ufeb7",
        "\ufeb8",
        "\u0635",
        "\ufeb9",
        "\ufeba", //ص
        "\ufebb",
        "\ufebc",
        "\u0636",
        "\ufebd",
        "\ufebe", //ض
        "\ufebf",
        "\ufec0",
        "\u0637",
        "\ufec1",
        "\ufec2", //ط
        "\ufec3",
        "\ufec4",
        "\u0638",
        "\ufec5",
        "\ufec6", //ط
        "\ufec7",
        "\ufec8",
        "\u0641",
        "\ufed1",
        "\ufed2", //ف
        "\ufed3",
        "\ufed4",
        "\u0642",
        "\ufed5",
        "\ufed6", //ق
        "\ufed7",
        "\ufed8",
        "\u0643",
        "\ufed9",
        "\ufeda", //ک
        "\ufedb",
        "\ufedc"
            };
            string[] erabha = new string[] {
        "\u064b",
        "\u064d",
        "\u064c"
            };
            string[] n = new string[] {
        "\u0646",
        "\ufee5",
        "\ufee6", // ن
        "\ufee8",
        "\ufee7"
            };
            string[] b = new string[] {
        "\u0628",
        "\ufe8f",
        "\ufe90", //ب
        "\ufe91",
        "\ufe92"
            };
            string[] m = new string[] {
        "\u0645",
        "\ufee1",
        "\ufee2", //م
        "\ufee4",
        "\ufee3"
            };
            string[] sukon = new string[1] { "\u0652" };
            bool rule1 = Wordrule_applyer(e.Key1, e.Key2, "ikhfaa", erabha, theseletter);
            bool rule2 = Wordrule_applyer(e.Key1, e.Key2, "ikhfaa", n, sukon, theseletter);
            bool rule3 = Wordrule_applyer(e.Key1, e.Key2, "ikhfaa", m, sukon, b);
            bool rule4 = Wordrule_applyer(e.Key1, e.Key2, "ikhfaa", erabha, new string[1] { "erab" }, theseletter);
            /*
            some of Quran surah that I checked manually for this filter :
            falaq,masadd:3,kafiroon,maun,quraish:4,fil:4,baqare:17,10
             */
            //return (rule1 && rule2 && rule3 && rule4);
        }
        /**
         * [filter_idgham description]
         * check tajvid idgham rules
         * @param  [type] key1 [description] the position of the character in array / row
         * @param  [type] key2 [description] the position of the character in array / column
         * @return [type] bool
         */
        public void Filter_idgham(object source, ProcessEventArgs e)
        {
            string[] erabha = new string[] {
        "\u064b",
        "\u064d",
        "\u064c"
            };
            string[] n = new string[] {
        "\u0646",
        "\ufee5",
        "\ufee6",
        "\ufee8",
        "\ufee7"
            };
            string[] theseletter = new string[] {
        "\u064a",
        "\ufef1",
        "\ufef2",
        "\ufef3",
        "\ufef4",
        "\u0649",
        "\ufeef",
        "\uFef0",
        "\u0646",
        "\ufee5",
        "\ufee6",
        "\ufee7",
        "\ufee8",
        "\u0645",
        "\ufee1",
        "\ufee2",
        "\ufee3",
        "\ufee4",
        "\u0648",
        "\ufeed",
        "\ufeee"
            };
            string[] m = new string[] {
        "\u0645",
        "\ufee1",
        "\ufee2", //م
        "\ufee4",
        "\ufee3"
            };
            string[] sukon = new string[1] { "\u0652" };
            string[] tasdid = new string[1] { "\u0651" };
            bool rule1 = Wordrule_applyer(e.Key1, e.Key2, "idgham", erabha, theseletter);
            bool rule2 = Wordrule_applyer(e.Key1, e.Key2, "idgham", erabha, new string[1] { "erab" }, theseletter);
            bool rule3 = Wordrule_applyer(e.Key1, e.Key2, "idgham", n, sukon, theseletter);
            bool rule4 = Wordrule_applyer(e.Key1, e.Key2, "idgham", n, theseletter);
            bool rule5 = Wordrule_applyer(e.Key1, e.Key2, "idgham", m, sukon, m, tasdid);
            bool rule6 = Wordrule_applyer(e.Key1, e.Key2, "idgham", m, m, tasdid);
            /*
            some of Quran surah that I checked manually for this filter :
            masadd,kafiroon:4,quraish:4,fil:4,5,humaza:2,zalzala:7,8
             */
            //return (rule1 || rule2 || rule3 || rule4 || rule5 || rule6);
        }
        /**
         * [filter_idgham_without_ghunna description]
         * check tajvid idgham without ghunna rules
         * @param  [type] key1 [description] the position of the character in array / row
         * @param  [type] key2 [description] the position of the character in array / column
         * @return [type] bool
         */
        public void Filter_idgham_without_ghunna(object source, ProcessEventArgs e)
        {
            string[] erabha = new string[] {
          "\u064b",
          "\u064d",
          "\u064c"
            };
            string[] n = new string[] {
          "\u0646",
          "\ufee5",
          "\ufee6", //ن
          "\ufee8",
          "\ufee7"
            };
            string[] theseletter = new string[] {
          "\u0644",
          "\ufedd",
          "\ufede",
          "\ufedf",
          "\ufee0",
          "\u0631",
          "\ufead",
          "\ufeae"
            };
            string[] sukon = new string[1] { "\u0652" };
            bool rule1 = Wordrule_applyer(e.Key1, e.Key2, "idghamwg", erabha, theseletter);
            bool rule2 = Wordrule_applyer(e.Key1, e.Key2, "idghamwg", erabha, new string[1] { "erab" }, theseletter);
            bool rule3 = Wordrule_applyer(e.Key1, e.Key2, "idghamwg", n, sukon, theseletter);
            bool rule4 = Wordrule_applyer(e.Key1, e.Key2, "idghamwg", n, theseletter);
            /*
            some of Quran surah that I checked manually for this filter :
            ikhlas:4,maun:4,humaza:1
             */
            //return (rule1 || rule2 || rule3 || rule4);
        }
        /**
         * [filter_maddah description]
         * check maddah
         * @param  [type] key1 [description] the position of the character in array / row
         * @param  [type] key2 [description] the position of the character in array / column
         * @return [type] bool
         */
        public void Filter_maddah(object source, ProcessEventArgs e)
        {
            string[] maddah = new string[1] { "\u0653" };
            bool rule1 = Wordrule_applyer(e.Key1, e.Key2, "maddah", maddah);
            //return rule1;
        }
        /**
         * [reorder description]
         * bascilly gluing words and flags to gather for render
         */
        public void Reorder()
        {
            foreach (KeyValuePair<int, Dictionary<int, logvis>> entry in text_ready)
            {
                for (int key2 = entry.Value.Count - 1; key2 >= 0; key2--)
                {
                    //fix deleting alef in first letter
                    if (Erab(text_ready[entry.Key][key2].chars))
                    {
                        if (text_ready.ContainsKey(entry.Key) && text_ready[entry.Key].ContainsKey(key2 - 1))
                        {
                            text_ready[entry.Key][key2 - 1] = new logvis((text_ready[entry.Key][key2 - 1].word + text_ready[entry.Key][key2].word), text_ready[entry.Key][key2 - 1].chars, text_ready[entry.Key][key2 - 1].flag);
                        }
                        if (text_ready[entry.Key][key2].flag != "none" && text_ready[entry.Key][key2 - 1].flag == "none")
                        {
                            text_ready[entry.Key][key2 - 1] = new logvis(text_ready[entry.Key][key2 - 1].word, text_ready[entry.Key][key2 - 1].chars, text_ready[entry.Key][key2].flag);
                        }
                        if (text_ready.ContainsKey(entry.Key) && text_ready[entry.Key].ContainsKey(key2 - 1))
                        {
                            text_ready[entry.Key].Remove(key2);
                        }
                    }
                }
            }
        }
        /**
         * [render description]
         * render whole text with rules applied to them
         * @param  string tag [description] tag name
         * @param  boolean return [description] return of echo / default is echo
         * @return [type] html    [description] final text
         */
        public void AppendText(RichTextBox box, SolidColorBrush color, string text)
        {
            TextRange range = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            range.Text = text;
            range.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }
        public void Render(RichTextBox box)
        {
            box.Document.Blocks.Clear();
            foreach (KeyValuePair<int, Dictionary<int, logvis>> entry in text_ready)
            {
                foreach (KeyValuePair<int, logvis> verse in entry.Value)
                {
                    AppendText(box, colors[verse.Value.flag], verse.Value.word);
                }
                box.AppendText(" ");
            }

        }
    }
}
