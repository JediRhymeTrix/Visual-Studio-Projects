using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a value: ");

            int a = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Value is: " + a);

            Console.ReadKey();
        }
    }
}
