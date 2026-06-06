using Lesson1;
using Lesson1.DTO;

ResultInfoDto resultInfoDto;
var analyzer = new Analyzer();
var fileSearch = new FileSearcher(analyzer);
var folderPath = "";
var longWord = "";



Console.WriteLine("Enter path to folder");
Console.WriteLine("Exemple: D:\\myFolder\\");

folderPath = Console.ReadLine();
longWord = await fileSearch.StartScanAsync(folderPath);
Console.WriteLine("Longer word: " + longWord);

