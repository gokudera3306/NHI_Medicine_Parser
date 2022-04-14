using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHI_Medicine_Parser.Class
{
    public struct NHIDataStruct
    {
        #region ----- Define Variables -----
        public string Name { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int DataLength => EndIndex - StartIndex + 1;
        public DataFormat Format { get; set; }
        #endregion

        public NHIDataStruct(string name, int startIndex, int endIndex, DataFormat format)
        {
            Name = name;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Format = format;
        }
    }
}
