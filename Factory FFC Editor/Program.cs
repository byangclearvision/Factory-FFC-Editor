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
using System.Reflection;
using Aspose.Imaging;
using Aspose.Imaging.FileFormats.Tiff.FileManagement;

using DALSA.SaperaLT.SapClassBasic;



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
            Bitmap factoryTif = default(Bitmap);
            
            // Stores the location of the input .tif factory FFC
            string tifloc;
            
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

                    // Location of input factory FFC TIFF
                    tifloc = @"C:\Users\BHY\EngineeringTools\Factory_FFC_Editor\Factory FFC Editor\" + ffcFile;

                    Console.WriteLine(tifloc);

                    // Load factory FFC .tif                   
                    try
                    {
                        // Create bitmap with factory FFC tif file saved at location: tifloc
                        factoryTif = new Bitmap(tifloc);
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
                        Console.WriteLine("Error: Incorrect dimensions. Input tiff must have a height of 2 pixels. Input image height: " + factoryTif.Height);
                        inputChek = 1;
                    }
                    // Check if the image is 16bit
                    System.Drawing.Imaging.PixelFormat pixelFormat = factoryTif.PixelFormat;
                    Console.WriteLine(pixelFormat);
                    if (System.Drawing.Image.GetPixelFormatSize(pixelFormat) != 32)
                    {
                        Console.WriteLine("Error: Incorrect BPP. Must be a 16 bit image. Entered Pixel Format: " + pixelFormat);
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
            int[,] testout = TiffToArray(@"C:\Users\BHY\EngineeringTools\Factory_FFC_Editor\Factory FFC Editor\" + ffcFile);
            ArrayToCsv(testout, "testout.csv");

            // Call the function to convert TIF file to array
            //byte[] factoryTempArray = TiffToArray(@"C:\Users\BHY\EngineeringTools\Factory_FFC_Editor\Factory FFC Editor\" + ffcFile,1);
            byte[] factoryTempArray = TiffToArray(@"C:\Users\BHY\EngineeringTools\Factory_FFC_Editor\Factory FFC Editor\" + ffcFile);
            byte[] factoryTopRow = TiffToArray(@"C:\Users\BHY\EngineeringTools\Factory_FFC_Editor\Factory FFC Editor\" + ffcFile);
            
            foreach (int i in factoryTopRow)
            {
                factoryTopRowInt[i] = factoryTopRow[i];
            }
            SaveCSV(factoryTopRowInt,"factorytopRow.csv");
            int[] factoryTempArrayInt = new int[factoryTempArray.Length];

            SaveCSV(factoryTempArrayInt,"factorybottomRow.csv");

            int[,] outputTifArray = new int[2, factoryTempArray.Length/4];
            int[] factoryTifArray = new int[factoryTempArray.Length/4];

            //save only the R values of factoryTif to get the greyscale pixel values
            for (int i = 0; i < factoryTempArray.Length; i += 4)
            {
                factoryTifArray[i / 4] = factoryTempArray[i];
            }

            double xscale = 1 / (factoryTifArray.Length / 512.0);
            double multiplied;
            double round;

            //Create output array and save multiplied values to output array
            for (int i = 0; i < factoryTifArray.Length; i++)
            {
                multiplied = factoryTifArray[i] * multiplierFunction(i,xscale);
                round = Math.Round(multiplied, 0, MidpointRounding.AwayFromZero);  
                //Console.WriteLine(round); 
                outputTifArray[1,i] = Convert.ToInt32(round);   
                outputTifArray[0,i] = factoryTopRow[i / 4];         
            }

            //SaveCSV(factoryTifArray,"out.csv");
            ArrayToTif(outputTifArray,"CorrectedUserFlatField.tif");
            ArrayToCsv(outputTifArray,"tif.csv");
        }

        public static int[,] TiffToArray(string fileName)
        {
            // Check the file extension and the pixel format of the image
            string extension = Path.GetExtension(fileName);
            if (extension != ".tif")
            {
                // Throw an exception or display an error message
                throw new ArgumentException("The file is not a TIFF file.");
                // MessageBox.Show("The file is not a TIFF file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // return null;
            }
            System.Drawing.Image imageToCheck = System.Drawing.Image.FromFile(fileName, true);
            int bitsPerPixel = System.Drawing.Image.GetPixelFormatSize(imageToCheck.PixelFormat);
            if (bitsPerPixel != 32)
            {
                // Throw an exception or display an error message
                throw new ArgumentException("The file is not a 32-bit ARGB TIFF file.");
                // MessageBox.Show("The file is not a 16-bit greyscale TIFF file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // return null;
            }

            // Load the TIFF file into a Bitmap object
            Bitmap image = new Bitmap(fileName);

            // Get the pixel values from the Bitmap object
            BitmapData imageData = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            Console.WriteLine("check: " + System.Drawing.Image.GetPixelFormatSize(image.PixelFormat));
            Console.Write("Image stride: " + imageData.Stride);
            int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 4;
            int byteCount = imageData.Stride * image.Height;
            byte[] pixelValues = new byte[byteCount];
            Marshal.Copy(imageData.Scan0, pixelValues, 0, byteCount);
            image.UnlockBits(imageData); 

            // Create a 2D array of int values from the byte array
            int[,] pixelData = new int[image.Height, image.Width];
            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width; j++)
                {
                    // Get the index of the byte array for the current pixel
                    int index = i * imageData.Stride + j * bytesPerPixel;
                    // Convert four bytes into an int value
                    //Console.WriteLine("Index: " + index);
                    ushort value = BitConverter.ToUInt16(pixelValues, index);
                    //Console.WriteLine("Value: " + value);
                    // Assign the value to the 2D array
                    pixelData[i, j] = value;
                }

            // Return the 2D array
            return pixelData;
        }
        
        public static void ArrayToTif(int[,] pixelValues, string filePath)
        {
            int width = pixelValues.GetLength(1);
            //Console.WriteLine("Width: " + width);
            int height = pixelValues.GetLength(0);
            //Console.WriteLine("Height: " + height);

            using (Image<L16> image = new Image<L16>(width, height))
            {
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < height; x++)
                    {
                        ushort pixelValue = (ushort)pixelValues[x, y];
                        image[y, x] = new L16(pixelValue);
                    }
                }
                image.Save(filePath, new SixLabors.ImageSharp.Formats.Tiff.TiffEncoder());
                Console.WriteLine("File has been saved as: " + filePath);
            }
        }

        public static void SaveCSV(int[] array, string filename)
        {
            // Convert the array to a comma-separated string
            string csv = string.Join(",", array);

            // Specify the file name and path to save the csv
            string filePath = Path.Combine(Environment.CurrentDirectory, filename);

            // Save the csv content to a file
            File.WriteAllText(filename, csv);

            // Print a message to confirm the operation
            Console.WriteLine($"The array has been saved as {filename}");
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

        public static double multiplierFunction(int x, double xscale)
        {
            double a = -1.95831826665063 * Math.Pow(10,-16);
            double b = 3.01385181237523 * Math.Pow(10,-13);
            double c = -1.75874993836724 * Math.Pow(10,-10);
            double d = 4.82556824079856 * Math.Pow(10,-8);
            double e = -5.70155060917491 * Math.Pow(10,-6);
            double f = 0.000054080877889916;
            double g = 0.034784144;
            double yscale = 61.0;

            double function = yscale * (a * Math.Pow(xscale * x,6) + b * Math.Pow(xscale * x,5) + c * Math.Pow(xscale * x,4) + d * Math.Pow(xscale * x,3)
                                + e * Math.Pow(xscale * x,2) + f * xscale * x + g);
            return function;
        }
    }    
}