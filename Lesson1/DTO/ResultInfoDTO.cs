
namespace Lesson1.DTO
{
    public record ResultInfoDTO
    {
        public int SymbolNum { get; set; } = 0;
        public int WordsNum { get; set; } = 0;
        public int LineNum { get; set; } = 0;
        public string LongWord { get; set; } = "";
        public int ChangeFlag { get; set; } = 0;
    }
}
