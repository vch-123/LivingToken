using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MagicBook
{
    public IEnumerable<string> GetSpells()
    {
        yield return "火球术";
        yield return "冰锥术";
        yield return "雷击术";
    }
}

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        var book = new MagicBook();
//        foreach (var spell in book.GetSpells())
//        {
//            Console.WriteLine($"你学会了：{spell}");
//        }
//    }
//}


