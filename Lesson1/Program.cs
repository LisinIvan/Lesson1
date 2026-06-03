using Lesson1;
using Lesson1.DTO;
using Lesson1.Interface;

ResultInfoDTO resultInfoDto;
IAnalyzer analyzer = new Analyzer();
FileSearcher fileSearch = new FileSearcher(analyzer);
string folderPath = "";
string longWord = "";



//Console.WriteLine("Enter path to .txt extension file");
Console.WriteLine("Enter path to folder");
Console.WriteLine("Exemple: D:\\myFolder\\");

folderPath = Console.ReadLine();
longWord = await fileSearch.StartScanAsync(folderPath);
Console.WriteLine("Longer word: " + longWord);

