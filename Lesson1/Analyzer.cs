using Lesson1.DTO;
using System.Text.RegularExpressions;

namespace Lesson1
{
    public class Analyzer
    {
        public async Task<ResultInfoDTO> StartAnalyzeAsync(string filePath)
        {
            ResultInfoDTO resultInfoDto = new ResultInfoDTO();
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return resultInfoDto;
            }
            try
            {
                await using FileStream stream = File.OpenRead(filePath);

                using StreamReader reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {

                    string? line = await reader.ReadLineAsync();
                    resultInfoDto.LineNum++;
                    resultInfoDto.ChangeFlag = 1;

                    if (!string.IsNullOrWhiteSpace(line))
                    {

                        var words = Regex.Split(line, @"\W+").Where(w => !string.IsNullOrEmpty(w)).ToArray();
                        resultInfoDto.WordsNum += words.Length;

                        foreach (var word in words)
                        {
                            string cleanWord = new string(
                                word.Where(c => !char.IsPunctuation(c)).ToArray());

                            if (string.IsNullOrEmpty(resultInfoDto.LongWord))
                            {
                                resultInfoDto.LongWord = cleanWord;
                            }
                            if (cleanWord.Length > resultInfoDto.LongWord.Length)
                            {
                                resultInfoDto.LongWord = cleanWord;
                            }
                        }

                        int symbolsInLine = line.Count(c => !char.IsWhiteSpace(c));
                        resultInfoDto.SymbolNum += symbolsInLine;
                    }

                    
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Ошибка: У пользователя нет прав на чтение файла \"{Path.GetFileName(filePath)}\".");
                resultInfoDto.ChangeFlag = -1; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла \"{Path.GetFileName(filePath)}\": {ex.Message}");
                resultInfoDto.ChangeFlag = -1;
            }
            return resultInfoDto;
        }
    }
}
