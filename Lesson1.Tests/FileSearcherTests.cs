using Lesson1.DTO;
using Lesson1.Interface; 
using Moq;
using System.Collections.Concurrent;


namespace Lesson1.Tests
{
    public class FileSearcherTests
    {
        [Fact]
        public async Task StartScanAsync_ShouldProcessFilesAndReturnLongestWord()
        {

            var mockAnalyzer = new Mock<IAnalyzer>();

            mockAnalyzer
                .Setup(a => a.Analyze(It.IsAny<string>()))
                .Returns(new ResultInfoDto { ChangeFlag = 1, LongWord = "СуперСлово" });

            var fileSearcher = new FileSearcher(mockAnalyzer.Object);

            string tempFolderPath = Path.Combine(Path.GetTempPath(), "FileSearcherTestFolder");
            Directory.CreateDirectory(tempFolderPath);

            string testFilePath = Path.Combine(tempFolderPath, "testFile.txt");
            await File.WriteAllTextAsync(testFilePath, "Какой-то текст");

            try
            {
                string result = await fileSearcher.StartScanAsync(tempFolderPath);

                Assert.Equal("СуперСлово", result);

                string expectedCsvPath = Path.Combine(tempFolderPath, "result.csv");
                Assert.True(File.Exists(expectedCsvPath));
            }
            finally
            {
                if (Directory.Exists(tempFolderPath))
                {
                    Directory.Delete(tempFolderPath, true);
                }
            }
        }
        [Fact]
        public async Task GetParallelConcurrentBag_ShouldReadFilesAndFillBag()
        {
            var mockAnalyzer = new Mock<IAnalyzer>();
            mockAnalyzer.Setup(a => a.Analyze(It.IsAny<string>()))
                        .Returns(new ResultInfoDto { ChangeFlag = 1, LongWord = "ТестРобота" });

            var fileSearcher = new FileSearcher(mockAnalyzer.Object);

            string tempFolderPath = Path.Combine(Path.GetTempPath(), "BagIsolatedTestFolder");
            Directory.CreateDirectory(tempFolderPath);

            await File.WriteAllTextAsync(Path.Combine(tempFolderPath, "file1.txt"), "контент");

            try
            {
                var bag = await fileSearcher.GetParallelConcurrentBag(new[] { "file1.txt" }, tempFolderPath);

                Assert.Single(bag); 
                var firstItem = bag.First();
                Assert.Equal("file1.txt", firstItem.FileName);
                Assert.Equal("ТестРобота", firstItem.Dto.LongWord);
            }
            finally
            {
                if (Directory.Exists(tempFolderPath))
                {
                    Directory.Delete(tempFolderPath, true);
                }
            }
        }

        [Fact]
        public async Task SaveToCsvAsync_ShouldCreateCorrectCsvFile()
        {
            var fileSearcher = new FileSearcher(null);

            string tempFolderPath = Path.Combine(Path.GetTempPath(), "CsvIsolatedTestFolder");
            Directory.CreateDirectory(tempFolderPath);

            var fakeBag = new ConcurrentBag<(string FileName, ResultInfoDto Dto)>();
            fakeBag.Add(("manual.txt", new ResultInfoDto { SymbolNum = 50, WordsNum = 10, LineNum = 5, LongWord = "Экскаватор" }));

            try
            {
                string absoluteLongest = await fileSearcher.SaveToCsvAsync(fakeBag, tempFolderPath);

                Assert.Equal("Экскаватор", absoluteLongest);

                string csvPath = Path.Combine(tempFolderPath, "result.csv");
                Assert.True(File.Exists(csvPath));

                string fileContent = await File.ReadAllTextAsync(csvPath);
                Assert.Contains("manual.txt", fileContent);
                Assert.Contains("Экскаватор", fileContent);
            }
            finally
            {
                if (Directory.Exists(tempFolderPath))
                {
                    Directory.Delete(tempFolderPath, true);
                }
            }
        }
    }
}