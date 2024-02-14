using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

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
            Bitmap modTif = default(Bitmap); 
            Bitmap factoryTif = default(Bitmap);
            
            // Stores the location of the modifier .tif and the input .tif factory FFC
            string[] tifloc = new string [2];
            
            Console.WriteLine("Ensure that factory FFC TIFF file is saved in the correct directory");
            Console.WriteLine("Enter the name of the factory FFC including the file extension (.tif):");
            
            // Ask for name of Factory FFC file
            while (inputChek == 1)
            {
                ffcFile = Console.ReadLine();
                        /*while (!ffcFile.Substring(ffcFile.Length - 4).Equals(".tif"))
                {
                    Console.WriteLine("Input the name of the factory FFC including the file extension (.tif):");
                    ffcFile = Console.ReadLine();
                }*/

                // Ask user to confirm the name of the file
                Console.WriteLine("\nConfirm the file name is: {0}", ffcFile);
                Console.WriteLine("press 'y' if correct, 'n' to re-enter file name");
                inputChekstr = Console.ReadLine();

                if (inputChekstr.Equals("y"))
                {
                    // inputChek = 0 to exit the while loop
                    inputChek = 0;
                    Console.WriteLine("pressed yes");

                    // Location of modifier TIFF
                    tifloc[0] = @"C:\Users\BHY\EngineeringTools\Factory_FFC_Editor\Factory FFC Editor\ffc_modifier.tif";
                    // Location of input factory FFC TIFF
                    tifloc[1] = @"C:\Users\BHY\EngineeringTools\Factory_FFC_Editor\Factory FFC Editor\" + ffcFile;

                    Console.WriteLine(tifloc[0]);
                    Console.WriteLine(tifloc[1]);

                    // Load modifier .tif                 
                    try
                    {
                        // Create bitmap with modifier tif file saved at location: tifloc[0]
                        modTif = new Bitmap(tifloc[0]);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine($"Error: {e.Message}. File location/name may be incorrect. Please enter menu 1 and enter file correct directory");
                    }
                    // Load factory FFC .tif                   
                    try
                    {
                        // Create bitmap with factory FFC tif file saved at location: tifloc[1]
                        factoryTif = new Bitmap(tifloc[1]);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine($"Error: {e.Message}. Input file name/location may be incorrect.");
                        Console.WriteLine("Enter the name of the factory FFC including the file extension (.tif):");
                        // inputChek = 1 to stay in the while loop
                        inputChek = 1;
                    }

                    // Check if factoryTif has the correct dimensions
                    if (factoryTif.Height != 2)
                    {
                        Console.WriteLine("Error: Incorrect dimensions. Input tiff must have a height of 2 pixels");
                        inputChek = 1;
                    }
                    // Check if length of factoryTif is equal to lenght of modTif
                    if (modTif.Width != factoryTif.Width)
                    {
                        Console.WriteLine("Error: Incorrect dimensions. tif dimensions must be equal");
                        inputChek = 1;
                    }
                    // Check if the image is 16bit
                    PixelFormat pixelFormat = factoryTif.PixelFormat;
                    Console.WriteLine(pixelFormat);
                    if (Image.GetPixelFormatSize(pixelFormat) != 32)
                    {
                        Console.WriteLine("Error: Incorrect BPP. Must be a 16 bit image");
                        inputChek = 1;
                    }
                }
                else if (inputChekstr.Equals("n"))
                {
                    // inputChek = 1 to stay in the while loop
                    inputChek = 1;
                    Console.WriteLine("pressed no");
                }
            }
            //Bitmap modTif = new Bitmap(tifloc[0]);

            /// Bitmaps are inputted, and can now be edited
            /// factoryTif
            /// modTif
            /// outTif
            
            // Create output tif
            Bitmap outTif = new Bitmap(modTif.Width, modTif.Height, PixelFormat.Format32bppArgb);

            // Read factoryTif numeric values
            string[] numericValues = File.ReadAllText(tifloc[1]).Split(',');
            Console.WriteLine(numericValues.Length);

            // Copy top row of modTif to outTif
            for (int x = 0; x < modTif.Width; x++)
            {
                outTif.SetPixel(x,0,modTif.GetPixel(x,0));
            }

            // Multiply bottom row on modTif by corresponding factoryTif
            for (int x = 0; x < modTif.Width; x++)
            {
                ushort pixelvalue = (ushort)(modTif.GetPixel(x,1).R * ushort.Parse(numericValues[x]));
                Color pixelcolor = Color.FromArgb(pixelvalue, pixelvalue, pixelvalue);
                outTif.SetPixel(x,1,pixelcolor);
            }

            // Save outTif
            outTif.Save(@"C:\Users\BHY\EngineeringTools\Factory_FFC_Editor\Factory FFC Editor", ImageFormat.Tiff);
            Console.WriteLine("Output file saved");
        }
    }    
}


