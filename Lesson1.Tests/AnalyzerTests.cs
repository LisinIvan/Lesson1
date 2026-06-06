using Lesson1.DTO;

namespace Lesson1.Tests
{
    public class AnalyzerTests
    {
        [Fact]
        public void Analyze_StandardText_ReturnsCorrectStatistics()
        {
            var analyzer = new Analyzer();

            string testText = "Привет мир\nТест";

            ResultInfoDto result = analyzer.Analyze(testText);

            Assert.Equal(2, result.LineNum);  
            Assert.Equal(3, result.WordsNum);    
            Assert.Equal("Привет", result.LongWord); 
            Assert.Equal(13, result.SymbolNum);   
            Assert.Equal(1, result.ChangeFlag);
        }

        [Fact]
        public void Analyze_EmptyOrSpaces_ReturnsEmptyDtoWithChangeFlag()
        {
            var analyzer = new Analyzer();
            string testText = "   "; 

            ResultInfoDto result = analyzer.Analyze(testText);

            Assert.Equal(0, result.LineNum);
            Assert.Equal(0, result.WordsNum);
            Assert.Null(result.LongWord); 
            Assert.Equal(1, result.ChangeFlag);
        }

        [Fact]
        public void Analyze_TextWithPunctuation_CleansWordsCorrectly()
        {
            var analyzer = new Analyzer();
            string testText = "Привет, мир!!!";

            ResultInfoDto result = analyzer.Analyze(testText);

            Assert.Equal("Привет", result.LongWord); 
            Assert.Equal(2, result.WordsNum);
        }
    }
}