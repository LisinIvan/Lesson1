
namespace Lesson1.DTO
{
    public record ResultInfoDto
    {
        public int SymbolNum { get; set; }
        public int WordsNum { get; set; }
        public int LineNum { get; set; }
        public string LongWord { get; set; }
        public int ChangeFlag { get; set; }
    }
}
