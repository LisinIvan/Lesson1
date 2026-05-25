using System;
using System.Collections.Generic;
using System.Text;

namespace Lesson1.DTO
{
    public struct ResultInfoDTO
    {
        public int symbolNum = 0;
        public int wordsNum = 0;
        public int lineNum = 0;
        public string longWord = "";
        public int changeFlag = 0;

        public ResultInfoDTO()
        { }
    }
}
