using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using Lesson1.DTO;
using Lesson1;

ResultInfoDTO resultInfoDto;
Analyzer analyzer = new Analyzer();
FileSearcher fileSearch = new FileSearcher();
string folderPath = "";
string longWord = "";



//Console.WriteLine("Enter path to .txt extension file");
Console.WriteLine("Enter path to folder");
Console.WriteLine("Exemple: D:\\myFolder\\");

folderPath = Console.ReadLine();
longWord = await fileSearch.StartScanAsync(folderPath);
Console.WriteLine("Longer word: " + longWord);

