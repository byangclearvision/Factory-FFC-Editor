using System;
using Microsoft.Win32.SystemEvents;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

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
            string ffcFile = "null";
            // Stores the location of the modifier .tif and the input .tif factory FFC
            string[] tifloc = new string [2];

            while (inputChek == 1)
            {
                Console.WriteLine("Input the name of the factory FFC including the file extension (.tif):");
                ffcFile = Console.ReadLine();
                        /*while (!ffcFile.Substring(ffcFile.Length - 4).Equals(".tif"))
                {
                    Console.WriteLine("Input the name of the factory FFC including the file extension (.tif):");
                    ffcFile = Console.ReadLine();
                }*/

                Console.WriteLine("\nConfirm the file name is: {0}", ffcFile);
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

            tifloc[0] = "ffc_scaler.tif";
            tifloc[1] = ffcFile;
            Console.WriteLine(tifloc[0]);
            Console.WriteLine(tifloc[1]);
        }

    }
}f