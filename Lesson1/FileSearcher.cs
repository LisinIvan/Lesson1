using Lesson1.DTO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Lesson1
{
    public class FileSearcher
    {
        private readonly Analyzer _analyzer = new Analyzer();

        public async Task<string> StartScanAsync(string folderPath)
        {
            // 1. Проверка: Верный ли путь к папке
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                Console.WriteLine("Ошибка: Путь к папке не верный или папка не существует.");
                return "";
            }
            string[] allFiles;
            try
            {
                // Получаем вообще все файлы в папке для анализа её содержимого
                allFiles = Directory.GetFiles(folderPath);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Ошибка: У вас нет прав на доступ к этой папке.");
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении папки: {ex.Message}");
                return "";
            }
            // 2. Проверка: Пустая папка
            if (allFiles.Length == 0)
            {
                Console.WriteLine("Ошибка: Указанная папка пуста.");
                return "";
            }
            // Фильтруем только .txt файлы
            string[] txtFiles = allFiles
                .Where(f => Path.GetExtension(f).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                .Select(Path.GetFileName)
                .ToArray();

            // 3. Проверка: В папке лежат не текстовые файлы (например, .jpg или .exe)
            if (txtFiles.Length == 0)
            {
                Console.WriteLine("Ошибка: В папке лежат не текстовые файлы (например, .jpg или .exe). Нет файлов с расширением .txt для анализа.");
                return "";
            }


            //string[] files = Directory.GetFiles(folderPath, "*.txt").Select(Path.GetFileName).ToArray();
            //if (files == null || files.Length == 0)
            //    return "";

            string csvPath = Path.Combine(folderPath, "result.csv");
            object fileLock = new object();
            var localLongWords = new ConcurrentBag<string>();

            try
            {
                await using (StreamWriter writer = new StreamWriter(csvPath, false, Encoding.UTF8))
                {
                    await writer.WriteLineAsync("FileName;symbolNum;wordsNum;lineNum;longWord");

                    await Parallel.ForEachAsync(txtFiles, async (fileName, cancellationToken) =>
                    {
                        string fullPath = Path.Combine(folderPath, fileName);

                        ResultInfoDTO resultInfoDto = await _analyzer.startAnalizeAsync(fullPath);
                        if (resultInfoDto.changeFlag == -1)
                        {
                            return;
                        }

                        if (!string.IsNullOrEmpty(resultInfoDto.longWord))
                        {
                            localLongWords.Add(resultInfoDto.longWord);
                        }

                        lock (fileLock)
                        {
                            writer.WriteLine($"\"{fileName}\";\"{resultInfoDto.symbolNum}\";\"{resultInfoDto.wordsNum}\";\"{resultInfoDto.lineNum}\";\"{resultInfoDto.longWord}\"");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка при записи итогового CSV: {ex.Message}");
            }

            string absoluteLongerWord = localLongWords
                .OrderByDescending(w => w.Length)
                .FirstOrDefault() ?? "";

            return absoluteLongerWord;
        }
    }
}
