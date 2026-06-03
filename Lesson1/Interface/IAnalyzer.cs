using Lesson1.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lesson1.Interface
{
    public interface IAnalyzer
    {
        ResultInfoDTO Analyze(string content);
    }
}
