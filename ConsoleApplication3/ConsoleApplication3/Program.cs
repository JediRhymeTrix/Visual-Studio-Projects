/*Exercise
WAP in C# which takes a string from user, passes it
as char array to CheckPalin(char[] s).
CheckPalin(char[] s) method should
return  " Not Palindrom";
or
return " is Palindrom";
*/
using System;
namespace Palindrome
{ //Include appropriate library namespace & create user defined namespace
    class ArrayPalindrome
    {//Create a class
        static string myString;//Add a array of char type as a data member  
        public static void Main()
        {//Define main
            char[] str;//Declare variables in main
            Console.WriteLine("Enter Palindrom");
            myString = Console.ReadLine();
            str = myString.ToCharArray();
            /* Call a function CheckPalin which takes char array as parameter 
            this function should return a string */
            Console.WriteLine(CheckPalin(str));
            Console.ReadKey();
        }
        static string CheckPalin(char[] s)//....CheckPalin(char[] s)
        {
            Array.Reverse(s);
            string temp = new string(s);
            string first = myString.Substring(0, myString.Length / 2);
            string second = temp.Substring(0, temp.Length / 2);
            if (!(first.Equals(second)))

                return "Not Palindrom";

           else
                return "Is A Palindrom";
        }
    }
};

