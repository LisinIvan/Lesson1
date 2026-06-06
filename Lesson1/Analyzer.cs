using Lesson1.DTO;
using Lesson1.Interface;
using System.Text.RegularExpressions;

namespace Lesson1
{
    public class Analyzer:IAnalyzer
    {
        public ResultInfoDto Analyze(string content)
        {
            var dto = new ResultInfoDto();
            dto.ChangeFlag = 1;

            if (string.IsNullOrWhiteSpace(content))
            {
                return dto;
            }

            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            dto.LineNum = lines.Length;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var words = Regex.Split(line, @"\W+").Where(w => !string.IsNullOrEmpty(w)).ToArray();
                dto.WordsNum += words.Length;

                foreach (var word in words)
                {
                    var cleanWord = new string(word.Where(c => !char.IsPunctuation(c)).ToArray());

                    if (string.IsNullOrEmpty(dto.LongWord) || cleanWord.Length > dto.LongWord.Length)
                    {
                        dto.LongWord = cleanWord;
                    }
                }

                int symbolsInLine = line.Count(c => !char.IsWhiteSpace(c));
                dto.SymbolNum += symbolsInLine;
            }

            return dto;
        }
    }
}
