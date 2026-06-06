using Lesson1.DTO;
using Lesson1.Interface; 
using Moq;


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
    }
}