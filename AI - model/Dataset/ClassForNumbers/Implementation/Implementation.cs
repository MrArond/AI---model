using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AI___model.Dataset.ClassForNumbers.Implementation
{
    internal class Implementation : Interfaces.INumberRules
    {
        public async Task<byte> NumberRules(bool[,] Table)
        {
            byte[] array = new byte[27];
            
            for (int i = 0; i <= 27; i++)
            {
                byte count = 0;
                for (int j = 0; j <= 27; i++)
                {
                    if (Table[i,j] == true)
                    {
                        count++;
                    }
                    array[i] = count;
                }
            }
            byte MaxValue = array.Max();
            byte AffiliationToNumber = (byte)(MaxValue / 28);
            return AffiliationToNumber;
        }
    }
}
