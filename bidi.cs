using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rokhan.inc
{
    class bidi
    {
        /**
		* Returns the unicode caracter specified by UTF-8 code
		* @param int c UTF-8 code
		* @return Returns the specified character.
		* @author Miguel Perez, Nicola Asuni
		* @since 2.3.000 (2008-03-05)
		*/
        /**
         * converted to c# by peyman farahmand(pfndesigen@gmail.com)
         * 
         * 2018/10/12
         */
        public string unichr(int c)
        {
            string chstring;
            if (c <= 0x7F) {
                // one byte
                Byte[] MyCchars = {(byte)c};
                chstring = "" + (char)c;
                return Encoding.UTF8.GetString(MyCchars);
            } else if (c <= 0x7FF) {
                // two bytes
                Byte[] MyCchars = { (byte)(0xC0 | c >> 6), (byte)(0x80 | c & 0x3F) };
                return Encoding.UTF8.GetString(MyCchars);
            } else if (c <= 0xFFFF) {
                // three bytes
                Byte[] MyCchars = { (byte)(0xE0 | c >> 12), (byte)(0x80 | c >> 6 & 0x3F), (byte)(0x80 | c & 0x3F) };
                return Encoding.UTF8.GetString(MyCchars);
            } else if (c <= 0x10FFFF) {
                // four bytes
                Byte[] MyCchars = { (byte)(0xF0 | c >> 18), (byte)(0x80 | c >> 12 & 0x3F), (byte)(0x80 | c >> 6 & 0x3F), (byte)(0x80 | c & 0x3F) };
                return Encoding.UTF8.GetString(MyCchars);
            } else {
                return "";
            }
        }
        /**
		 * Converts UTF-8 strings to codepoints array.<br>
		 * Invalid byte sequences will be replaced with 0xFFFD (replacement character)<br>
		 * Based on: http://www.faqs.org/rfcs/rfc3629.html
		 * <pre>
		 * 	  Char. number range  |        UTF-8 octet sequence
		 *       (hexadecimal)    |              (binary)
		 *    --------------------+-----------------------------------------------
		 *    0000 0000-0000 007F | 0xxxxxxx
		 *    0000 0080-0000 07FF | 110xxxxx 10xxxxxx
		 *    0000 0800-0000 FFFF | 1110xxxx 10xxxxxx 10xxxxxx
		 *    0001 0000-0010 FFFF | 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
		 *    ---------------------------------------------------------------------
		 *
		 *   ABFN notation:
		 *   ---------------------------------------------------------------------
		 *   UTF8-octets = *( UTF8-char )
		 *   UTF8-char   = UTF8-1 / UTF8-2 / UTF8-3 / UTF8-4
		 *   UTF8-1      = %x00-7F
		 *   UTF8-2      = %xC2-DF UTF8-tail
		 *
		 *   UTF8-3      = %xE0 %xA0-BF UTF8-tail / %xE1-EC 2( UTF8-tail ) /
		 *                 %xED %x80-9F UTF8-tail / %xEE-EF 2( UTF8-tail )
		 *   UTF8-4      = %xF0 %x90-BF 2( UTF8-tail ) / %xF1-F3 3( UTF8-tail ) /
		 *                 %xF4 %x80-8F 2( UTF8-tail )
		 *   UTF8-tail   = %x80-BF
		 *   ---------------------------------------------------------------------
		 * </pre>
		 * @param string str string to process.
		 * @return array containing codepoints (UTF-8 characters values)
		 * @author Nicola Asuni
		 * @since 1.53.0.TC005 (2005-01-05)
		 */
        public static bool ContainsArabicCharacter(string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            for (int i = 1; i < bytes.Length; i += 2)
                if (bytes[i] == 6)  //0x06** is arabic code page
                    return true;
            return false;
        }
        public List<int> UTF8StringToArray(string str)
        {
            List<int> unicode = new List<int>(); // array containing unicode values
            List<int> bytes = new List<int>(); // array containing single character byte sequences
			int numbytes = 1; // number of octetc needed to represent the UTF-8 character
            int length,xchar;
            if (ContainsArabicCharacter(str))
            length = Encoding.UTF8.GetByteCount(str);
            else
            length = str.Length;
            byte[] strarray = Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < length; i++) {
				xchar = (int) strarray[i]; // get one string character at time
                if (bytes.Count == 0)
                { // get starting octect
                    if (xchar <= 0x7F) {
                        
						unicode.Add (xchar); // use the character "as is" because is ASCII
						numbytes = 1;
                    }
                    else if((xchar >> 0x05) == 0x06) { // 2 bytes character (0x06 = 110 BIN)
                        bytes.Add((xchar -0xC0) << 0x06); 
						numbytes = 2;
                    }
                    else if((xchar >> 0x04) == 0x0E) { // 3 bytes character (0x0E = 1110 BIN)
                        bytes.Add((xchar -0xE0) << 0x0C); 
						numbytes = 3;
                    }
                    else if((xchar >> 0x03) == 0x1E) { // 4 bytes character (0x1E = 11110 BIN)
                        bytes.Add((xchar - 0xF0) << 0x12);
						numbytes = 4;
                    } else {
                        // use replacement character for other invalid sequences
                        unicode.Add(0xFFFD);
                        bytes.Clear();
						numbytes = 1;
                    }
                }
                else if((xchar >> 0x06) == 0x02) { // bytes 2, 3 and 4 must start with 0x02 = 10 BIN
                    bytes.Add(xchar -0x80);
                    if (bytes.Count == numbytes) {
						// compose UTF-8 bytes to a single unicode value
						xchar = bytes[0];
                        for (int j = 1; j < numbytes; j++) {
							xchar += (bytes[j] << ((numbytes - j - 1) *0x06));
                        }
                        if (((xchar >= 0xD800) && (xchar <= 0xDFFF)) || (xchar >= 0x10FFFF)) {
                            /* The definition of UTF-8 prohibits encoding character numbers between
							U+D800 and U+DFFF, which are reserved for use with the UTF-16
							encoding form (as surrogate pairs) and do not directly represent
							characters. */
                            unicode.Add(0xFFFD); // use replacement character
                        }
						else {
                            unicode.Add(xchar); // add char to array
                        }
                        // reset data for next char
                        bytes.Clear(); 
						numbytes = 1;
                    }
                } else {
                    // use replacement character for other invalid sequences
                    unicode.Add(0xFFFD);
                    bytes.Clear();
					numbytes = 1;
                }
            }
            return unicode;
        }

        /**
		 * Reverse the RLT substrings using the Bidirectional Algorithm (http://unicode.org/reports/tr9/).
		 * @param array ta array of characters composing the string.
		 * @param bool forcertl if 'R' forces RTL, if 'L' forces LTR
		 * @return string
		 * @author Nicola Asuni
		 * @since 2.4.000 (2008-03-06)
		*/
        public struct remember
        {
            public int num;
            public int cell;
            public string dos;
            public remember(int num, int cell, string dos)
            {
                this.num = num;
                this.cell = cell;
                this.dos = dos;
            }
        }
        public struct chardata
        {
            public string chars;
            public int level;
            public string type;
            public string sor;
            public string eor;
            public int i;
            public int x;
            public chardata(string chars, int level, string type, string sor, string eor,int i=0, int x = 0)
            {
                this.chars = chars;
                this.level = level;
                this.type = type;
                this.sor = sor;
                this.eor = eor;
                this.i = i;
                this.x = x;
            }
        }

        public int[] Utf8Bidi(List<int> ta, string forcertl = "N")
        {

            var unicode = Unicode_data.Unicode1;
            var unicode_mirror = Unicode_data.unicode_mirror;
            var unicode_arlet = Unicode_data.unicode_arlet; 
            var laa_array = Unicode_data.laa_array;
            var diacritics = Unicode_data.diacritics;

			// paragraph embedding level
			int pel = 0;
            // max level
            int maxlevel = 0;
			
			// get number of chars
			int numchars = ta.Count;

            if (forcertl == "R") {
					pel = 1;
            }
            else if(forcertl == "L") {
					pel = 0;
            } else {
                // P2. In each paragraph, find the first character of type L, AL, or R.
                // P3. If a character is found in P2 and it is of type AL or R, then set the paragraph embedding level to one; otherwise, set it to zero.
                for (int i = 0; i < numchars; i++) {
					string type = unicode[ta[i]];
                    if (type == "L") {
						pel = 0;
                        break;
                    }
                    else if((type == "AL") || (type == "R")) {
						pel = 1;
                        break;
                    }
                }
            }
			
			// Current Embedding Level
			int cel = pel;
			// directional override status
			string dos = "N";
            List<remember> remember = new List<remember>();
            // start-of-level-run
            string sor= "L";
            if (pel==1)
                sor = "R";

			string eor = sor;

            //levels = array(array("level" => cel, "sor" => sor, "eor" => "", "chars" => array()));
            //current_level = &levels[count( levels )-1];

            // Array of characters data
            List<chardata> chardata = new List<chardata>();

            // X1. Begin by setting the current embedding level to the paragraph embedding level. Set the directional override status to neutral. Process each character iteratively, applying rules X2 through X9. Only embedding levels from 0 to 61 are valid in this phase.
            // 	In the resolution of levels in rules I1 and I2, the maximum embedding level of 62 can be reached.
            for (int i = 0; i < numchars; i++) {
                if (ta[i] == Unicode_data.K_RLE) {
					// X2. With each RLE, compute the least greater odd embedding level.
					//	a. If this new level would be valid, then this embedding code is valid. Remember (push) the current embedding level and override status. Reset the current level to this new level, and reset the override status to neutral.
					//	b. If the new level would not be valid, then this code is invalid. Do not change the current level or override status.
					int next_level = cel + (cel % 2) +1;
                    if (next_level < 62) {
                        remember ut = new remember(Unicode_data.K_RLE, cel, dos);
                        remember.Add(ut);
						cel = next_level;
						dos = "N";
						sor = eor;
                        eor = "L";
                        if (cel % 2 == 1)
                            eor = "R";
                    }
                }
                else if(ta[i] == Unicode_data.K_LRE) {
                    // X3. With each LRE, compute the least greater even embedding level.
                    //	a. If this new level would be valid, then this embedding code is valid. Remember (push) the current embedding level and override status. Reset the current level to this new level, and reset the override status to neutral.
                    //	b. If the new level would not be valid, then this code is invalid. Do not change the current level or override status.
                    int next_level = cel + 2 - (cel % 2);
                    if ( next_level < 62 ) {
                        remember ut = new remember(Unicode_data.K_LRE, cel, dos);
                        remember.Add(ut);
						cel = next_level;
						dos = "N";
						sor = eor;
                        eor = "L";
                        if (cel % 2 == 1)
                            eor = "R";
                    }
                }
                else if(ta[i] == Unicode_data.K_RLO) {
                    // X4. With each RLO, compute the least greater odd embedding level.
                    //	a. If this new level would be valid, then this embedding code is valid. Remember (push) the current embedding level and override status. Reset the current level to this new level, and reset the override status to right-to-left.
                    //	b. If the new level would not be valid, then this code is invalid. Do not change the current level or override status.
                    int next_level = cel + (cel % 2) +1;
                    if (next_level < 62) {
                        remember ut = new remember(Unicode_data.K_RLO, cel, dos);
                        remember.Add(ut);
						cel = next_level;
						dos = "R";
						sor = eor;
                        eor = "L";
                        if (cel % 2 == 1)
                            eor = "R";
                    }
                }
                else if(ta[i] == Unicode_data.K_LRO) {
					// X5. With each LRO, compute the least greater even embedding level.
					//	a. If this new level would be valid, then this embedding code is valid. Remember (push) the current embedding level and override status. Reset the current level to this new level, and reset the override status to left-to-right.
					//	b. If the new level would not be valid, then this code is invalid. Do not change the current level or override status.
					int next_level = cel + 2 - (cel % 2);
                    if ( next_level < 62 ) {
                        remember ut = new remember(Unicode_data.K_LRO, cel, dos);
                        remember.Add(ut);
						cel = next_level;
						dos = "L";
						sor = eor;
                        eor = "L";
                        if (cel % 2 == 1)
                            eor = "R";
                    }
                }
                else if(ta[i] == Unicode_data.K_PDF) {
                    // X7. With each PDF, determine the matching embedding or override code. If there was a valid matching code, restore (pop) the last remembered (pushed) embedding level and directional override.
                    if (remember.Count>0)
                    {
						int last = remember.Count - 1;
                        if ((remember[last].num == Unicode_data.K_RLE) ||
                              (remember[last].num == Unicode_data.K_LRE) ||
                              (remember[last].num == Unicode_data.K_RLO) ||
                              (remember[last].num == Unicode_data.K_LRO)) {
							remember match = remember[last];
                            remember.RemoveAt(last);
							cel = match.cell;
							dos = match.dos;
							sor = eor;
                            eor = "L";
                            if ((cel > match.cell ? cel : match.cell) % 2 == 1)
                            {
                                eor = "R";
                            }
						}
					}
				} else if ((ta[i] != Unicode_data.K_RLE) &&
								 (ta[i] != Unicode_data.K_LRE) &&
                                 (ta[i] != Unicode_data.K_RLO) &&
                                 (ta[i] != Unicode_data.K_LRO) &&
                                 (ta[i] != Unicode_data.K_PDF)) {
                    // X6. For all types besides RLE, LRE, RLO, LRO, and PDF:
                    //	a. Set the level of the current character to the current embedding level.
                    //	b. Whenever the directional override status is not neutral, reset the current character type to the directional override status.
                    string chardir;
                    if (dos != "N") {
						chardir = dos.ToString();//meh
					} else {
						chardir = Unicode_data.Unicode1[ta[i]];
					}
                    // stores string characters and other information
                    
                    chardata data = new chardata(ta[i].ToString(), cel, chardir, sor, eor);
                    chardata.Add(data); 
                }
			} // end for each char
			
			// X8. All explicit directional embeddings and overrides are completely terminated at the end of each paragraph. Paragraph separators are not included in the embedding.
			// X9. Remove all RLE, LRE, RLO, LRO, PDF, and BN codes.
			// X10. The remaining rules are applied to each run of characters at the same level. For each run, determine the start-of-level-run (sor) and end-of-level-run (eor) type, either L or R. This depends on the higher of the two levels on either side of the boundary (at the start or end of the paragraph, the level of the other run is the base embedding level). If the higher level is odd, the type is R; otherwise, it is L.
			
			// 3.3.3 Resolving Weak Types
			// Weak types are now resolved one level run at a time. At level run boundaries where the type of the character on the other side of the boundary is required, the type assigned to sor or eor is used.
			// Nonspacing marks are now resolved based on the previous characters.
			numchars = chardata.Count;
			
			// W1. Examine each nonspacing mark (NSM) in the level run, and change the type of the NSM to the type of the previous character. If the NSM is at the start of the level run, it will get the type of sor.
			int prevlevel = -1; // track level changes
			int levcount = 0; // counts consecutive chars at the same level
			for (int i=0; i < numchars; i++) {
				if (chardata[i].type == "NSM") {
					if (levcount>0) {
						chardata[i] = new chardata(chardata[i].chars, chardata[i].level, chardata[i].sor.ToString(), chardata[i].sor, chardata[i].eor);
					} else if (i > 0) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, chardata[(i - 1)].type, chardata[i].sor, chardata[i].eor);
					}
				}
				if (chardata[i].level != prevlevel) {
					levcount = 0;
				} else {
					levcount++;
				}
				prevlevel = chardata[i].level;
			}
			
			// W2. Search backward from each instance of a European number until the first strong type (R, L, AL, or sor) is found. If an AL is found, change the type of the European number to Arabic number.
			prevlevel = -1;
			levcount = 0;
			for (int i=0; i < numchars; i++) {
				if (chardata[i].chars == "EN") {
					for (int j=levcount; j >= 0; j--) {
						if (chardata[j].type == "AL") {
                            chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "AN", chardata[i].sor, chardata[i].eor);
                        } else if ((chardata[j].type == "L") || (chardata[j].type == "R")) {
							break;
						}
					}
				}
				if (chardata[i].level != prevlevel) {
					levcount = 0;
				} else {
					levcount++;
				}
				prevlevel = chardata[i].level;
			}
			
			// W3. Change all ALs to R.
			for (int i=0; i < numchars; i++) {
				if (chardata[i].type == "AL") {
                    chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "R", chardata[i].sor, chardata[i].eor);
                } 
			}
			
			// W4. A single European separator between two European numbers changes to a European number. A single common separator between two numbers of the same type changes to that type.
			prevlevel = -1;
			levcount = 0;
			for (int i=0; i < numchars; i++) {
				if ((levcount > 0) && ((i+1) < numchars) && (chardata[(i+1)].level == prevlevel)) {
					if ((chardata[i].type == "ES") && (chardata[(i-1)].type == "EN") && (chardata[(i+1)].type == "EN")) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "EN", chardata[i].sor, chardata[i].eor);
                    } else if ((chardata[i].type == "CS") && (chardata[(i-1)].type == "EN") && (chardata[(i+1)].type == "EN")) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "EN", chardata[i].sor, chardata[i].eor);
                    } else if ((chardata[i].type == "CS") && (chardata[(i-1)].type == "AN") && (chardata[(i+1)].type == "AN")) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "AN", chardata[i].sor, chardata[i].eor);
                    }
				}
				if (chardata[i].level != prevlevel) {
					levcount = 0;
				} else {
					levcount++;
				}
				prevlevel = chardata[i].level;
			}
			
			// W5. A sequence of European terminators adjacent to European numbers changes to all European numbers.
			prevlevel = -1;
			levcount = 0;
			for (int i=0; i < numchars; i++) {
				if(chardata[i].type == "ET") {
					if ((levcount > 0) && (chardata[(i-1)].type == "EN")) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "EN", chardata[i].sor, chardata[i].eor);
                    } else {
						int j = i+1;
						while ((j < numchars) && (chardata[j].level == prevlevel)) {
							if (chardata[j].type == "EN") {
                                chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "EN", chardata[i].sor, chardata[i].eor);
                                break;
							} else if (chardata[j].type != "ET") {
								break;
							}
							j++;
						}
					}
				}
				if (chardata[i].level != prevlevel) {
					levcount = 0;
				} else {
					levcount++;
				}
				prevlevel = chardata[i].level;
			}
			
			// W6. Otherwise, separators and terminators change to Other Neutral.
			prevlevel = -1;
			levcount = 0;
			for (int i=0; i < numchars; i++) {
				if ((chardata[i].type == "ET") || (chardata[i].type == "ES") || (chardata[i].type == "CS")) {
                    chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "ON", chardata[i].sor, chardata[i].eor);
                }
				if (chardata[i].level != prevlevel) {
					levcount = 0;
				} else {
					levcount++;
				}
				prevlevel = chardata[i].level;
			}
			
			//W7. Search backward from each instance of a European number until the first strong type (R, L, or sor) is found. If an L is found, then change the type of the European number to L.
			prevlevel = -1;
			levcount = 0;
			for (int i=0; i < numchars; i++) {
				if (chardata[i].chars == "EN") {
					for (int j=levcount; j >= 0; j--) {
						if (chardata[j].type == "L") {
                            chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "L", chardata[i].sor, chardata[i].eor);
                        } else if (chardata[j].type == "R") {
							break;
						}
					}
				}
				if (chardata[i].level != prevlevel) {
					levcount = 0;
				} else {
					levcount++;
				}
				prevlevel = chardata[i].level;
			}
			
			// N1. A sequence of neutrals takes the direction of the surrounding strong text if the text on both sides has the same direction. European and Arabic numbers act as if they were R in terms of their influence on neutrals. Start-of-level-run (sor) and end-of-level-run (eor) are used at level run boundaries.
			prevlevel = -1;
			levcount = 0;
			for (int i=0; i < numchars; i++) {
				if ((levcount > 0) && ((i+1) < numchars) && (chardata[(i+1)].level == prevlevel)) {
					if ((chardata[i].type == "N") && (chardata[(i-1)].type == "L") && (chardata[(i+1)].type == "L")) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "L", chardata[i].sor, chardata[i].eor);
                    } else if ((chardata[i].type == "N") &&
					 ((chardata[(i-1)].type == "R") || (chardata[(i-1)].type == "EN") || (chardata[(i-1)].type == "AN")) &&
					 ((chardata[(i+1)].type == "R") || (chardata[(i+1)].type == "EN") || (chardata[(i+1)].type == "AN"))) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "R", chardata[i].sor, chardata[i].eor);
                    } else if (chardata[i].type == "N") {
						// N2. Any remaining neutrals take the embedding direction
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level,chardata[i].sor.ToString(), chardata[i].sor, chardata[i].eor);
                    }
				} else if ((levcount == 0) && ((i+1) < numchars) && (chardata[(i+1)].level == prevlevel)) {
					// first char
					if ((chardata[i].type == "N") && (chardata[i].sor == "L") && (chardata[(i+1)].type == "L")) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "L", chardata[i].sor, chardata[i].eor);
                    } else if ((chardata[i].type == "N") &&
					 ((chardata[i].sor == "R") || (chardata[i].sor == "EN") || (chardata[i].sor == "AN")) &&
					 ((chardata[(i+1)].type == "R") || (chardata[(i+1)].type == "EN") || (chardata[(i+1)].type == "AN"))) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "R", chardata[i].sor, chardata[i].eor);
                    } else if (chardata[i].type == "N") {
                        // N2. Any remaining neutrals take the embedding direction
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, chardata[i].sor.ToString(), chardata[i].sor, chardata[i].eor);
                    }
                } else if ((levcount > 0) && (((i+1) == numchars) || ((i+1) < numchars) && (chardata[(i+1)].level != prevlevel))) {
					//last char
					if ((chardata[i].type == "N") && (chardata[(i-1)].type == "L") && (chardata[i].eor == "L")) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "L", chardata[i].sor, chardata[i].eor);
                    } else if ((chardata[i].type == "N") &&
					 ((chardata[(i-1)].type == "R") || (chardata[(i-1)].type == "EN") || (chardata[(i-1)].type == "AN")) &&
					 ((chardata[i].eor == "R") || (chardata[i].eor == "EN") || (chardata[i].eor == "AN"))) {
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, "R", chardata[i].sor, chardata[i].eor);
                    } else if (chardata[i].type == "N") {
						// N2. Any remaining neutrals take the embedding direction
                        chardata[i] = new chardata(chardata[i].chars, chardata[i].level, chardata[i].sor, chardata[i].sor, chardata[i].eor);
                    }
				} else if (chardata[i].type == "N") {
                    // N2. Any remaining neutrals take the embedding direction
                    chardata[i] = new chardata(chardata[i].chars, chardata[i].level, chardata[i].sor, chardata[i].sor, chardata[i].eor);
                }
				if (chardata[i].level != prevlevel) {
					levcount = 0;
				} else {
					levcount++;
				}
				prevlevel = chardata[i].level;
			}
			
			// I1. For all characters with an even (left-to-right) embedding direction, those of type R go up one level and those of type AN or EN go up two levels.
			// I2. For all characters with an odd (right-to-left) embedding direction, those of type L, EN or AN go up one level.
			for (int i=0; i < numchars; i++) {
				int odd = chardata[i].level % 2;
				if (odd>0) {
					if ((chardata[i].type == "L") || (chardata[i].type == "AN") || (chardata[i].type == "EN")){
                        chardata[i] = new chardata(chardata[i].chars, (chardata[i].level+1), chardata[i].type, chardata[i].sor, chardata[i].eor);
                    }
				} else {
					if (chardata[i].type == "R") {
                        chardata[i] = new chardata(chardata[i].chars, (chardata[i].level + 1), chardata[i].type, chardata[i].sor, chardata[i].eor);
                    } else if ((chardata[i].type == "AN") || (chardata[i].type == "EN")){
                        chardata[i] = new chardata(chardata[i].chars, (chardata[i].level + 2), chardata[i].type, chardata[i].sor, chardata[i].eor);
                    }
				}
				maxlevel = Math.Max(chardata[i].level,maxlevel);
			}
			
			// L1. On each line, reset the embedding level of the following characters to the paragraph embedding level:
			//	1. Segment separators,
			//	2. Paragraph separators,
			//	3. Any sequence of whitespace characters preceding a segment separator or paragraph separator, and
			//	4. Any sequence of white space characters at the end of the line.
			for (int i=0; i < numchars; i++) {
				if ((chardata[i].type == "B") || (chardata[i].type == "S")) {
                    chardata[i] = new chardata(chardata[i].chars, pel, chardata[i].type, chardata[i].sor, chardata[i].eor);
                } else if (chardata[i].type == "WS") {
					int j = i+1;
					while (j < numchars) {
						if (((chardata[j].type == "B") || (chardata[j].type == "S")) ||
							((j == (numchars-1)) && (chardata[j].type == "WS"))) {
                            chardata[i] = new chardata(chardata[i].chars, pel, chardata[i].type, chardata[i].sor, chardata[i].eor);
                            break;
						} else if (chardata[j].type != "WS") {
							break;
						}
						j++;
					}
				}
			}
			
			// Arabic Shaping
			// Cursively connected scripts, such as Arabic or Syriac, require the selection of positional character shapes that depend on adjacent characters. Shaping is logically applied after the Bidirectional Algorithm is used and is limited to characters within the same directional run. 
			int[] endedletter = new int[] { 1569, 1570, 1571, 1572, 1573, 1575, 1577, 1583, 1584, 1585, 1586, 1608, 1688 };
            int[] alfletter = new int[] { 1570, 1571, 1573, 1575 };
			List<chardata> chardata2 = new List<chardata>();
            chardata2.AddRange(chardata);
			bool laaletter = false;
            List<chardata> charAL = new List<chardata>();
            int x = 0;
			for (int i=0; i < numchars; i++) {
				if ((Unicode_data.Unicode1[Convert.ToInt32(chardata[i].chars)] == "AL") || (unicode[Convert.ToInt32(chardata[i].chars)] == "WS")) {
					charAL.Add(new chardata(chardata[i].chars,chardata[i].level, chardata[i].type, chardata[i].sor, chardata[i].eor,i));
					chardata[i] = new chardata(chardata[i].chars, chardata[i].level, chardata[i].type, chardata[i].sor, chardata[i].eor, 0,x);
					x++;
				}
			}
			int numAL = x;
            chardata prevchar;
            chardata nextchar;
            chardata thischar;
            Dictionary<int, int[]> arabicarr;
            //string test = Array.Exists(alfletter, element => element == Convert.ToInt32("1570")) ? "Yes, there is" : "No, there isn't";
            for (int i = 0; i < numchars; i++) {
                thischar = chardata[i];
				if (i > 0) {
                    prevchar = chardata[(i-1)];
				} else {
                    prevchar = new chardata("", -1, "", "", "");
                }
				if ((i+1) < numchars) {
                    nextchar = chardata[(i+1)];
				} else {
                    nextchar = new chardata("",-1,"","","");
				}
				if (unicode[Convert.ToInt32(thischar.chars)] == "AL") {
					x = thischar.x;
					if (x > 0) {
                        prevchar = charAL[(x-1)];
					} else {
                        prevchar = new chardata("", -1, "", "", "");
                    }
					if ((x+1) < numAL) {
						nextchar = charAL[(x+1)];
					} else {
						nextchar = new chardata("", -1, "", "", "");
                    }
					// if laa letter
					if ((prevchar.level != -1) && (Convert.ToInt32(prevchar.chars) == 1604) && alfletter.Contains(Convert.ToInt32(thischar.chars))) {
                        arabicarr = laa_array;
						laaletter = true;
						if (x > 1) {
							prevchar = charAL[(x-2)];
						} else {
							prevchar = new chardata("", -1, "", "", "");
                        }
					} else {
                        arabicarr = unicode_arlet;
						laaletter = false;
					}
					if ((prevchar.level != -1) && (nextchar.level != -1) &&
						((unicode[Convert.ToInt32(prevchar.chars)] == "AL") || (unicode[Convert.ToInt32(prevchar.chars)] == "NSM")) &&
						((unicode[Convert.ToInt32(nextchar.chars)] == "AL") || (unicode[Convert.ToInt32(nextchar.chars)] == "NSM")) &&
						(prevchar.type == thischar.type) &&
						(nextchar.type == thischar.type) &&
						(Convert.ToInt32(nextchar.chars) != 1567)) {
						if (endedletter.Contains(Convert.ToInt32(prevchar.chars))) {
							if (arabicarr.ContainsKey(Convert.ToInt32(thischar.chars))) {
                                // initial
                                int[] temp = arabicarr[Convert.ToInt32(thischar.chars)];
                                if(temp.Length >= 3)
                                chardata2[i] = new chardata(temp[2].ToString(), chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);

                            }
						} else {
                            if (arabicarr.ContainsKey(Convert.ToInt32(thischar.chars)))
                            {
                                // medial
                                int[] temp = arabicarr[Convert.ToInt32(thischar.chars)];
                                if (temp.Length >= 4)
                                    chardata2[i] = new chardata(temp[3].ToString(), chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);
                            }
						}
					} else if ((nextchar.level != -1) &&
						((unicode[Convert.ToInt32(nextchar.chars)] == "AL") || (unicode[Convert.ToInt32(nextchar.chars)] == "NSM")) &&
						(nextchar.type == thischar.type) &&
						(Convert.ToInt32(nextchar.chars) != 1567)) {
						if (arabicarr.ContainsKey(Convert.ToInt32(chardata[i].chars))) {

                            // initial
                            int[] temp = arabicarr[Convert.ToInt32(thischar.chars)];
                            if (temp.Length >= 3)
                                chardata2[i] = new chardata(temp[2].ToString(), chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);
                        }
					} else if (((prevchar.level != -1) &&
						((unicode[Convert.ToInt32(prevchar.chars)] == "AL") || (unicode[Convert.ToInt32(prevchar.chars)] == "NSM")) &&
						(prevchar.type == thischar.type)) ||
						((nextchar.level != -1) && (Convert.ToInt32(nextchar.chars) == 1567))) {
						// final
						if ((i > 1) && (Convert.ToInt32(thischar.chars) == 1607) &&
							(Convert.ToInt32(chardata[i-1].chars) == 1604) &&
							(Convert.ToInt32(chardata[i-2].chars) == 1604)) {
							//Allah Word
							// mark characters to delete with false
							chardata2[i-2] = new chardata(null, chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);
							chardata2[i-1] = new chardata(null, chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor); 
							chardata2[i] = new chardata("65010", chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);
						} else {
							if ((prevchar.level != -1) && endedletter.Contains(Convert.ToInt32(prevchar.chars))) {
								if (arabicarr.ContainsKey(Convert.ToInt32(thischar.chars))) {
                                    // isolated
                                    chardata2[i] = new chardata(Convert.ToString(arabicarr[Convert.ToInt32(thischar.chars)][0]), chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);
								}
							} else {
								if (arabicarr.ContainsKey(Convert.ToInt32(thischar.chars))) {
                                    // final
                                    int[] temp = arabicarr[Convert.ToInt32(thischar.chars)];
                                    if (temp.Length >= 2)
                                        chardata2[i] = new chardata(temp[1].ToString(), chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);
								}
							}
						}
					} else if (arabicarr.ContainsKey(Convert.ToInt32(thischar.chars))) {
                        // isolated
                        chardata2[i] = new chardata(Convert.ToString(arabicarr[Convert.ToInt32(thischar.chars)][0]), chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);
                    }
					// if laa letter
					if(laaletter) {
						// mark characters to delete with false
						//fixing la
                        			chardata2[i] = new chardata("65166", chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);
						//chardata2[(charAL[(x-1)].i)] = new chardata(null, chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);
					}
				} // end if AL (Arabic Letter)
			} // end for each char
            
            // remove marked characters
            int k = 0;
			foreach(chardata value in chardata2.ToList()) {
				if (value.chars == null) {
                    chardata2.RemoveAt(k);
				}
                k++;
            }
			chardata = chardata2;
			numchars = chardata.Count;
			
			// L2. From the highest level found in the text to the lowest odd level on each line, including intermediate levels not actually present in the text, reverse any contiguous sequence of characters that are at that level or higher.
			for (int j=maxlevel; j > 0; j--) {
                List<chardata> ordarray = new List<chardata>();
                List<chardata> revarr=new List<chardata>();
				bool onlevel = false;
				for (int i=0; i < numchars; i++) {
					if (chardata[i].level >= j) {
						onlevel = true;
						if (unicode_mirror.ContainsKey((ushort)(Convert.ToUInt32(chardata[i].chars)))) {
							// L4. A character is depicted by a mirrored glyph if and only if (a) the resolved directionality of that character is R, and (b) the Bidi_Mirrored property value of that character is true.
							chardata[i] = new chardata(Convert.ToString(unicode_mirror[(ushort)(Convert.ToUInt32(chardata[i].chars))]), chardata2[i].level, chardata2[i].type, chardata2[i].sor, chardata2[i].eor);
                        }
						revarr.Add(chardata[i]);
					} else {
						if(onlevel) {
                            revarr.Reverse();
							ordarray.AddRange(revarr);
							revarr.Clear();
							onlevel = false;
						}
						ordarray.Add(chardata[i]);
					}
				}
				if(onlevel) {
                    revarr.Reverse();
                    ordarray.AddRange(revarr);
                }
				chardata = ordarray;
			}
			
			int[] ordarray2 = new int[numchars];
			for (int i=0; i < numchars; i++) {
				ordarray2[i] = Convert.ToInt32(chardata[i].chars);
			}
			
			return ordarray2;
		}
    }
}
