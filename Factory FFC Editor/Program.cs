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
                        Bitmap modTif = new Bitmap(tifloc[0]);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine($"Error: {e.Message}. File location/name may be incorrect. Please enter menu 1 and enter file correct directory");
                    }

                    try
                    {
                        // Create bitmap with factory FFC tif file saved at location: tifloc[1]
                        Bitmap factoryTif = new Bitmap(tifloc[1]);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine($"Error: {e.Message}. Input file name/location may be incorrect.");
                        Console.WriteLine("Enter the name of the factory FFC including the file extension (.tif):");
                        // inputChek = 1 to stay in the while loop
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



        }
    
        private static bool ValidateDllPath(ref string path, string RelativePath = "", string Extension = "")
        {
        // Check if it contains any Invalid Characters.
        if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
        {
            try
            {
            // If path is relative take %IGXLROOT% as the base directory
            if (!Path.IsPathRooted(path))
            {
                if (string.IsNullOrEmpty(RelativePath))
                {
                // Exceptions handled by Path.GetFullPath
                // ArgumentException path is a zero-length string, contains only white space,
                // or contains one or more of the invalid characters defined in 
                // GetInvalidPathChars. -or- The system could not retrieve the absolute path.
                // 
                // SecurityException The caller does not have the required permissions.
                // 
                // ArgumentNullException path is null.
                // 
                // NotSupportedException path contains a colon (":") that is not part of a
                // volume identifier (for example, "c:\"). 
                // PathTooLongException The specified path, file name, or both exceed the
                // system-defined maximum length. For example, on Windows-based platforms,
                // paths must be fewer than 248 characters, and file names must be fewer than
                // 260 characters.

                // RelativePath is not passed so we would take the project path 
                path = Path.GetFullPath(RelativePath);

                }
                else
                {
                // Make sure the path is relative to the RelativePath and not our project
                // directory
                path = Path.Combine(RelativePath, path);
                }
            }

            // Exceptions from FileInfo Constructor:
            //   System.ArgumentNullException:
            //     fileName is null.
            //
            //   System.Security.SecurityException:
            //     The caller does not have the required permission.
            //
            //   System.ArgumentException:
            //     The file name is empty, contains only white spaces, or contains invalid
            //     characters.
            //
            //   System.IO.PathTooLongException:
            //     The specified path, file name, or both exceed the system-defined maximum
            //     length. For example, on Windows-based platforms, paths must be less than
            //     248 characters, and file names must be less than 260 characters.
            //
            //   System.NotSupportedException:
            //     fileName contains a colon (:) in the middle of the string.
            FileInfo fileInfo = new FileInfo(path);

            // Exceptions using FileInfo.Length:
            //   System.IO.IOException:
            //     System.IO.FileSystemInfo.Refresh() cannot update the state of the file or
            //     directory.
            //
            //   System.IO.FileNotFoundException:
            //     The file does not exist.-or- The Length property is called for a directory.
            bool throwEx = fileInfo.Length == -1;

            // Exceptions using FileInfo.IsReadOnly:
            //   System.UnauthorizedAccessException:
            //     Access to fileName is denied.
            //     The file described by the current System.IO.FileInfo object is read-only.
            //     -or- This operation is not supported on the current platform.
            //     -or- The caller does not have the required permission.
            throwEx = fileInfo.IsReadOnly;

            if (!string.IsNullOrEmpty(Extension))
            {
                // Validate the Extension of the file.
                if (Path.GetExtension(path).Equals(Extension,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                // Trim the Library Path
                path = path.Trim();
                return true;
                }
                else
                {
                return false;
                }
            }
            else
            {
                return true;

            }
            }
            catch (ArgumentNullException)
            {
            //   System.ArgumentNullException:
            //     fileName is null.
            }
            catch (System.Security.SecurityException)
            {
            //   System.Security.SecurityException:
            //     The caller does not have the required permission.
            }
            catch (ArgumentException)
            {
            //   System.ArgumentException:
            //     The file name is empty, contains only white spaces, or contains invalid
            //     characters.
            }
            catch (UnauthorizedAccessException)
            {
            //   System.UnauthorizedAccessException:
            //     Access to fileName is denied.
            }
            catch (PathTooLongException)
            {
            //   System.IO.PathTooLongException:
            //     The specified path, file name, or both exceed the system-defined maximum
            //     length. For example, on Windows-based platforms, paths must be less than
            //     248 characters, and file names must be less than 260 characters.
            }
            catch (NotSupportedException)
            {
            //   System.NotSupportedException:
            //     fileName contains a colon (:) in the middle of the string.
            }
            catch (FileNotFoundException)
            {
            // System.FileNotFoundException
            //  The exception that is thrown when an attempt to access a file that does not
            //  exist on disk fails.
            }
            catch (IOException)
            {
            //   System.IO.IOException:
            //     An I/O error occurred while opening the file.
            }
            catch (Exception)
            {
            // Unknown Exception. Might be due to wrong case or nulll checks.
            }
        }
        else
        {
            // Path contains invalid characters
        }
        return false;
        }   
    }
    
}


