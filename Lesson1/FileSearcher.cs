using Lesson1.DTO;
using System.Collections.Concurrent;
using System.Text;

namespace Lesson1
{
    public class FileSearcher
    {
        private readonly Analyzer _analyzer = new Analyzer();

        public async Task<string> StartScanAsync(string folderPath)
        {

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                Console.WriteLine("Ошибка: Путь к папке не верный или папка не существует.");
                return "";
            }
            string[] allFiles;
            try
            {
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
            if (allFiles.Length == 0)
            {
                Console.WriteLine("Ошибка: Указанная папка пуста.");
                return "";
            }
            string[] txtFiles = allFiles
                .Where(f => Path.GetExtension(f).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                .Select(Path.GetFileName)
                .ToArray();

            if (txtFiles.Length == 0)
            {
                Console.WriteLine("Ошибка: В папке лежат не текстовые файлы (например, .jpg или .exe). Нет файлов с расширением .txt для анализа.");
                return "";
            }

            var resultsBag = new ConcurrentBag<(string FileName, ResultInfoDTO Dto)>();

            try
            {

                await Parallel.ForEachAsync(txtFiles, async (fileName, cancellationToken) =>
                {
                    string fullPath = Path.Combine(folderPath, fileName);

                    ResultInfoDTO resultInfoDto = await _analyzer.StartAnalyzeAsync(fullPath);
                    if (resultInfoDto.ChangeFlag == -1)
                    {
                        return;
                    }

                    resultsBag.Add((fileName, resultInfoDto));

                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка при параллельном анализе файлов: {ex.Message}");
            }

            string csvPath = Path.Combine(folderPath, "result.csv");
            string absoluteLongerWord = "";

            try
            {
                await using (StreamWriter writer = new StreamWriter(csvPath, false, Encoding.UTF8))
                {
                    await writer.WriteLineAsync("FileName;symbolNum;wordsNum;lineNum;longWord");

                    foreach (var item in resultsBag)
                    {
                        await writer.WriteLineAsync($"\"{item.FileName}\";\"{item.Dto.SymbolNum}\";\"{item.Dto.WordsNum}\";\"{item.Dto.LineNum}\";\"{item.Dto.LongWord}\"");

                        if (item.Dto.LongWord.Length > absoluteLongerWord.Length)
                        {
                            absoluteLongerWord = item.Dto.LongWord;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка при записи итогового CSV: {ex.Message}");
            }

            return absoluteLongerWord;
        }
    }
}
