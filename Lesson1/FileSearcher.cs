using Lesson1.DTO;
using Lesson1.Interface;
using System.Collections.Concurrent;
using System.Text;

namespace Lesson1
{
    public class FileSearcher
    {
        private readonly IAnalyzer _analyzer;

        public FileSearcher(IAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

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
            else
            {
                var resultsBag = await GetParallelConcurrentBag(txtFiles, folderPath);

                string absoluteLongerWord = await SaveToCsvAsync(resultsBag, folderPath);

                return absoluteLongerWord;
            }
        }
        public async Task<ConcurrentBag<(string FileName, ResultInfoDTO Dto)>> GetParallelConcurrentBag(string[] txtFiles, string folderPath)
        {
            var resultsBag = new ConcurrentBag<(string FileName, ResultInfoDTO Dto)>();

            try
            {

                await Parallel.ForEachAsync(txtFiles, async (fileName, cancellationToken) =>
                {
                    string fullPath = Path.Combine(folderPath, fileName);
                    string fileContent = "";
                    ResultInfoDTO resultInfoDto;

                    try
                    {
                        fileContent = await File.ReadAllTextAsync(fullPath, cancellationToken);

                        resultInfoDto = _analyzer.Analyze(fileContent);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"Ошибка: Нет прав на чтение файла \"{fileName}\".");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при чтении файла \"{fileName}\": {ex.Message}");
                        return;
                    }

                    resultsBag.Add((fileName, resultInfoDto));

                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка при параллельном анализе файлов: {ex.Message}");
            }
            return resultsBag;
        }
        public async Task<string> SaveToCsvAsync(ConcurrentBag<(string FileName, ResultInfoDTO Dto)> resultBag, string folderPath)
        {
            string csvPath = Path.Combine(folderPath, "result.csv");
            string absoluteLongerWord = "";

            try
            {
                await using (StreamWriter writer = new StreamWriter(csvPath, false, Encoding.UTF8))
                {
                    await writer.WriteLineAsync("FileName;symbolNum;wordsNum;lineNum;longWord");

                    foreach (var item in resultBag)
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
