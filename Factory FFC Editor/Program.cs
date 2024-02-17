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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.CompilerServices;



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
                    if (System.Drawing.Image.GetPixelFormatSize(pixelFormat) != 32)
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

            /// The input image has been checked for data type, format, and size, now to edit it and create the output file
            
            // Call the function to convert TIF file to array
            byte[] modTempArray = TiffToArray(tifloc[0],1);
            byte[] factoryTempArray = TiffToArray(tifloc[1],1);
            byte[] factoryTopRow = TiffToArray(tifloc[1],0);
            int[,] outputTifArray = new int[2, modTempArray.Length];
            int[] modTifArray = new int[modTempArray.Length/4];
            int[] factoryTifArray = new int[factoryTempArray.Length/4];
            

            
            //save only the R values of modTif and factoryTif to get the greyscale pixel values
            for (int i = 0; i < modTempArray.Length; i += 4)
            {
                modTifArray[i / 4] = modTempArray[i];
            } 
            for (int i = 0; i < factoryTempArray.Length; i += 4)
            {
                factoryTifArray[i / 4] = factoryTempArray[i];
            }

            Console.WriteLine(factoryTifArray.Length);
            Console.WriteLine(factoryTifArray[factoryTifArray.Length - 1]);
            Console.WriteLine(factoryTopRow.Length);

            // Create output array
            /*for (int i = 0; i < factoryTempArray.Length; i++)
            {
                outputTifArray[0,i] = 
            }*/

            foreach (var i in factoryTifArray)
            {
                int index = i * 4;
                outputTifArray[0,index] = factoryTopRow[i];
                outputTifArray[1,index] = factoryTifArray[i];
                outputTifArray[1,index+1] = factoryTifArray[i];
                outputTifArray[1,index+2] = factoryTifArray[i];
                outputTifArray[1,index+3] = 255;
                
            }

            //SaveCSV(factoryTifArray,"out.csv");
            ArrayToTif(outputTifArray,"CorrectedUserFlatField.tif");
            ArrayToCsv(outputTifArray,"tif.csv");

        }

        // A function that takes a 2xn int array and a file name as parameters
        // and converts the array to a grayscale image and saves it as a tif file
        public static void ArrayToTif(int[,] array, string fileName)
        {
            // Get the dimensions of the array
            int rows = array.GetLength(0); // 2
            int cols = array.GetLength(1); // n

            // Create a new image with the same size as the array
            using (var image = new Image<L8>(cols, rows))
            {
                // Loop through the array and set the pixel values
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        // Get the int value from the array and clamp it to [0, 255]
                        int value = array[i, j];
                        value = Math.Max(0, Math.Min(255, value));

                        // Create a grayscale pixel with the value
                        var pixel = new L8((byte)value);

                        // Set the pixel at the corresponding position in the image
                        image[j, i] = pixel;
                    }
                }

                // Save the image as a tif file with the given file name
                image.Save(fileName, new TiffEncoder());
                Console.WriteLine($"TIFF file has been saved as {fileName}");
            }
        }
        
        
        public static void SaveCSV(int[] array, string filename)
        {
            // Create a StringBuilder to store the csv content
            StringBuilder csv = new StringBuilder();

            // Loop through the array and append each byte to the csv, separated by commas
            for (int i = 0; i < array.GetLength(0); i++)
            {
      
                csv.Append(array[i]);
                if (i < array.Length - 1)
                {
                    csv.Append(",");
                }
            
                csv.AppendLine();
            }

            // Save the csv content to a file
            File.WriteAllText(filename, csv.ToString());

            // Print a message to confirm the operation
            Console.WriteLine($"The array of bytes has been saved as {filename}");
        }

        // Define a method to convert a tiff file to an array of bytes
        public static byte[] TiffToArray(string filePath, int line)
        {
            // Load the tiff image from the file path
            using var image =  SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);

            // Get the width and height of the image
            int width = image.Width;

            // Create an array of bytes with the same size as the image
            byte[] array = new byte[width * 4];

            // Loop through the pixels of the image and copy their values to the array

            for (int x = 0; x < width; x++)
            {
                
                // Get the pixel at the current position
                var pixel = image[x, line];
                int index = x * 4;
                // Copy the pixel values to the array in RGBA order
                array[index] = pixel.R;
                array[index+1] = pixel.G;
                array[index+2] = pixel.B;
                array[index+3] = pixel.A;
            }


            // Return the array of bytes
            return array;
        }

        // A function that takes a 2xn int array and a file name as parameters
        // and converts the array to a comma-separated values (csv) file and saves it
        public static void ArrayToCsv(int[,] array, string fileName)
        {
            // Get the dimensions of the array
            int rows = array.GetLength(0); // 2
            int cols = array.GetLength(1); // n

            // Create a new string builder to store the csv content
            var sb = new StringBuilder();

            // Loop through the array and append the values to the string builder
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Get the int value from the array
                    int value = array[i, j];

                    // Append the value to the string builder, followed by a comma
                    sb.Append(value);
                    sb.Append(",");
                }

                // Remove the last comma and add a new line
                sb.Remove(sb.Length - 1, 1);
                sb.AppendLine();
            }

            // Save the string builder content as a csv file with the given file name
            File.WriteAllText(fileName, sb.ToString());
            Console.WriteLine($"The array has been saved as {fileName}");
        }
    }    
}


