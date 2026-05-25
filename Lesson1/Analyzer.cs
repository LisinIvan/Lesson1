using Lesson1.DTO;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

namespace Lesson1
{
    public class Analyzer
    {
        public async Task<ResultInfoDTO> startAnalizeAsync(string filePath)
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
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var words = Regex.Split(line, @"\W+").Where(w => !string.IsNullOrEmpty(w)).ToArray();
                    resultInfoDto.wordsNum += words.Length;

                    foreach (var word in words)
                    {
                        string cleanWord = new string(
                            word.Where(c => !char.IsPunctuation(c)).ToArray());

                        if (string.IsNullOrEmpty(resultInfoDto.longWord))
                        {
                            resultInfoDto.longWord = cleanWord;
                        }
                        if (cleanWord.Length > resultInfoDto.longWord.Length)
                        {
                            resultInfoDto.longWord = cleanWord;
                        }
                    }

                    line = line.Replace(" ", "");
                    if (line.Count() > 0)
                    {
                        resultInfoDto.symbolNum += line.Count();
                    }

                    int symbolsInLine = line.Count(c => c != ' ');
                    resultInfoDto.symbolNum += symbolsInLine;

                    resultInfoDto.lineNum++;
                    resultInfoDto.changeFlag = 1;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Ошибка: У пользователя нет прав на чтение файла \"{Path.GetFileName(filePath)}\".");
                resultInfoDto.changeFlag = -1; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла \"{Path.GetFileName(filePath)}\": {ex.Message}");
                resultInfoDto.changeFlag = -1;
            }
            return resultInfoDto;
        }
    }
}
