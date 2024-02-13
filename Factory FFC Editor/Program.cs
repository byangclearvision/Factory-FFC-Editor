using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

//using DALSA.SaperaLT.SapClassBasic;
//using DALSA.SaperaLT.Examples.NET.Utils;

namespace FactoryFFCEditor
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("This program edits the factory FFC");
            Console.WriteLine("--------------------------------------------------------------------------------\n");


            int inputChek = 1;
            string inputChekstr;
            string FFCfile;
            // Stores the location of the modifier .tif and the input .tif factory FFC
            string[] tifloc = {"n","n"};

            while (inputChek == 1)
            {
                Console.WriteLine("Input the name of the factory FFC including the file extension (.tif):");
                FFCfile = Console.ReadLine();
                /*while (!FFCfile.Substring(FFCfile.Length - 4).Equals(".tif"))
                {
                    Console.WriteLine("Input the name of the factory FFC including the file extension (.tif):");
                    FFCfile = Console.ReadLine();
                }*/

                Console.WriteLine("\nConfirm the file name is: {0}", FFCfile);
                Console.WriteLine("press 'y' if correct, 'n' to re-enter file name");
                inputChekstr = Console.ReadLine();

                if (inputChekstr.Equals("y"))
                {
                    inputChek = 0;
                    Console.WriteLine("pressed yes");
                }
                else if (inputChekstr.Equals("n"))
                {
                    inputChek = 1;
                    Console.WriteLine("pressed no");
                }
            }
            //Console.WriteLine("Exit Loop");

            tifloc[0] =  "ffc_scaler.tif";
            Console.WriteLine(tifloc[0]);
        }

    }
}