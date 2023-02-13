using System.Drawing;
using System.IO;

internal class Program
{
    static string toreplace = "";
    static string replace = "";
    private static void Main(string[] args)
    {
        Console.WriteLine("path:");
        string path = Console.ReadLine();
        Console.WriteLine("what to replace:");
         toreplace = Console.ReadLine();
        Console.WriteLine("replace by what:");
         replace = Console.ReadLine();
        RenameInFolder(path);
    }
    public static void RenameInFolder(string path)
    {
        foreach (var item in Directory.GetFiles(path))
        {
            try
            {
                string a = item;
                string b = item.Remove(item.Length - item.Split('\\').Last().Length);
                b += item.Split('\\').Last().Replace(toreplace, replace);
                System.IO.File.Move(item, b);
            }
            catch
            {
            }
        }
        foreach (var item in Directory.GetDirectories(path))
        {
            RenameInFolder(item);
        }
    }
}