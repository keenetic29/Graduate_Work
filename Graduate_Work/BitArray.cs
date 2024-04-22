using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduate_Work
{
    public class BitArray
    {
        public static int[] ToBits(int decimalnumber, int numberofbits)
        {
            int[] bitarray = new int[numberofbits];
            int k = numberofbits - 1;
            char[] bd = Convert.ToString(decimalnumber, 2).ToCharArray();

            for (int i = bd.Length - 1; i >= 0; --i, --k)
            {

                if (bd[i] == '1')
                    bitarray[k] = 1;
                else
                    bitarray[k] = 0;
            }

            while (k >= 0)
            {
                bitarray[k] = 0;
                --k;
            }

            return bitarray;
        }

        public static int ToDecimal(int[] bitsarray)
        {
            string stringvalue = "";
            for (int i = 0; i < bitsarray.Length; i++)
            {
                stringvalue += bitsarray[i].ToString();
            }
            int DecimalValue = Convert.ToInt32(stringvalue, 2);

            return DecimalValue;
        }
    }
}
