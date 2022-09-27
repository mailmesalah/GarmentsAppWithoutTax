using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfServerApp.Services
{
    class BarcodeService 
    {
        public static Dictionary<string, string> NextCharacter = new Dictionary<string, string>();
        private static int noOfCharacters = 5;
        private static readonly object @barcodeLock = new object();
        
        public int NoOfCharacters
        {
            get { return noOfCharacters; }
            set { noOfCharacters = value; }
        }

        public List<string> ReadBarcodes(int limit)
        {
            List<string> barcodes = new List<string>();
            for(int i =1;i<= limit; ++i)
            {
                barcodes.Add(ReadNextBarcode());
            }

            return barcodes;
        }

        public string ReadNextBarcode()
        {
            string barcode = new String('0',NoOfCharacters);
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.barcode_generator.FirstOrDefault();
                    lock (@barcodeLock)
                    {
                        if (val == null)
                        {

                            barcode_generator bn = dataB.barcode_generator.Create();
                            bn.barcode = barcode;

                            dataB.barcode_generator.Add(bn);                            

                        }
                        else
                        {
                            barcode = val.barcode;
                            val.barcode = getNextBarcode(barcode);
                        }
                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {

            }
            return barcode;
        }

        private string getNextBarcode(string barcode)
        {
            string nextBarcode = "";
            
            for (int i = 0; i < barcode.Length; ++i)
            {

                if (barcode.ElementAt(i) == 'Z')
                {
                    nextBarcode += NextCharacter[barcode.ElementAt(i).ToString()];
                }
                else
                {
                    nextBarcode += NextCharacter[barcode.ElementAt(i).ToString()];
                    if (i != barcode.Length - 1)
                    {
                        nextBarcode += barcode.Substring(i+1,barcode.Length-(i+1));
                    }
                    
                    break;
                }
                
            }

            return nextBarcode;
        }
        public void initialiseBarcodeService()
        {
            //Numbers
            NextCharacter["0"] = "1";
            NextCharacter["1"] = "2";
            NextCharacter["2"] = "3";
            NextCharacter["3"] = "4";
            NextCharacter["4"] = "5";
            NextCharacter["5"] = "6";
            NextCharacter["6"] = "7";
            NextCharacter["7"] = "8";
            NextCharacter["8"] = "9";
            NextCharacter["9"] = "a";
            //Small letters
            NextCharacter["a"] = "b";
            NextCharacter["b"] = "c";
            NextCharacter["c"] = "d";
            NextCharacter["d"] = "e";
            NextCharacter["e"] = "f";
            NextCharacter["f"] = "g";
            NextCharacter["g"] = "h";
            NextCharacter["h"] = "i";
            NextCharacter["i"] = "j";
            NextCharacter["j"] = "k";
            NextCharacter["k"] = "l";
            NextCharacter["l"] = "m";
            NextCharacter["m"] = "n";
            NextCharacter["n"] = "o";
            NextCharacter["o"] = "p";
            NextCharacter["p"] = "q";
            NextCharacter["q"] = "r";
            NextCharacter["r"] = "s";
            NextCharacter["s"] = "t";
            NextCharacter["t"] = "u";
            NextCharacter["u"] = "v";
            NextCharacter["v"] = "w";
            NextCharacter["w"] = "x";
            NextCharacter["x"] = "y";
            NextCharacter["y"] = "z";
            NextCharacter["z"] = "A";
            //Big letters
            NextCharacter["A"] = "B";
            NextCharacter["B"] = "C";
            NextCharacter["C"] = "D";
            NextCharacter["D"] = "E";
            NextCharacter["E"] = "F";
            NextCharacter["F"] = "G";
            NextCharacter["G"] = "H";
            NextCharacter["H"] = "I";
            NextCharacter["I"] = "J";
            NextCharacter["J"] = "K";
            NextCharacter["K"] = "L";
            NextCharacter["L"] = "M";
            NextCharacter["M"] = "N";
            NextCharacter["N"] = "O";
            NextCharacter["O"] = "P";
            NextCharacter["P"] = "Q";
            NextCharacter["Q"] = "R";
            NextCharacter["R"] = "S";
            NextCharacter["S"] = "T";
            NextCharacter["T"] = "U";
            NextCharacter["U"] = "V";
            NextCharacter["V"] = "W";
            NextCharacter["W"] = "X";
            NextCharacter["X"] = "Y";
            NextCharacter["Y"] = "Z";
            NextCharacter["Z"] = "0";            
        }
        
    }
}
