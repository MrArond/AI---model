using System;
using System.Collections.Generic;
using System.Text;

namespace AI___model.Dataset.ClassForNumbers.Interfaces
{
    internal interface INumberRules
    {
        public Task<byte> NumberRules(bool[,]Table); 
    }
}
