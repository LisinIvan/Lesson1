using Lesson1.DTO;

namespace Lesson1.Interface
{
    public interface IAnalyzer
    {
        ResultInfoDto Analyze(string content);
    }
}
