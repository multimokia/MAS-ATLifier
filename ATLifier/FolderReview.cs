using System;
using System.IO;
using System.Collections.Generic;

namespace folderStuffs
{
    class FolderReview
    {
        /// <summary>
        /// Checks if the file at the path specified exists
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// </summary>
        /// <returns><see cref="bool"/> true if exists, false otherwise</returns>
        public Boolean exists(string filename, string filepath)
        {
            return (File.Exists(filepath + "/" + filename));
        }

        //START: File creation/modifying methods
        /// <summary>
        /// Creates a blank file in the specified location
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// </summary>
        /// <returns><see cref="int"/> indicating status</returns>
        public int create(string filename, string filepath)
        {
            //NOTE: This returns 0 here because these files are essentially just for state checking, rather than storing data
            if (exists(filename, filepath))
            {
                Console.WriteLine("Create, no data - File already exists");
                return (0);
            }
            else
            {
                try
                {
                    //Need to verify if the directory exists
                    if (!Directory.Exists(filepath))
                        createPath(filepath);

                    //Now we do the rest
                    var file = File.Create(filepath + "/" + filename);
                    file.Close();
                    Console.WriteLine("Create, no data - File creation successful");
                    return (1);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Create, no data - File creation failed");
                    return (2);
                }
            }
        }

        /// <summary>
        /// Creates the file specified with provided data string
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// <param name="data">A string of data to be put into the file</param>
        /// <param name="overwrite">A bool for whether we want to overwrite the file or not</param>
        /// </summary>
        /// <returns><see cref="int"/> indicating status</returns>
        public int create(string filename, string filepath, string data, bool overwrite = false)
        {
            //Delete the file if we want to overwrite it
            if (overwrite)
                delete(filename, filepath);


            //Need to verify if the directory exists
            if (!Directory.Exists(filepath))
                createPath(filepath);

            //Create the file should it not exist
            //NOTE: this method checks whether or not it exists already
            create(filename, filepath);

            //Write to the file
            using (StreamWriter sw = File.AppendText(filepath + "/" + filename))
            {
                try
                {
                    sw.WriteLine(data);
                    sw.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Create, single string data - File creation failed");
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine("Create, single string data - File creation successful");
            return (1);
        }

        /// <summary>
        /// Creates the file specified with provided data list
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// <param name="data">A string list of data to be put into the file</param>
        /// <param name="overwrite">A bool for whether we want to overwrite the file or not</param>
        /// </summary>
        /// <returns><see cref="int"/> indicating status</returns>
        public int create(string filename, string filepath, List<string> data, bool overwrite=false)
        {
            if (overwrite)
                delete(filename, filepath);

            //Need to verify if the directory exists
            if (!Directory.Exists(filepath))
                createPath(filepath);

            //Create the file should it not exist
            //NOTE: this method checks whether or not it exists already
            create(filename, filepath);

            //Write to the file
            using (StreamWriter sw = File.AppendText(filepath + "/" + filename))
            {
                try
                {
                    foreach (string s in data)
                    {
                        sw.WriteLine(s);
                    }
                    sw.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Create, string data list - File creation failed");
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine("Create, string data list - File creation successful");
            return (1);
        }

        /// <summary>
        /// Creates the filepath specified
        /// <param name="filepath">A local path for the file.</param>
        /// </summary>
        /// <returns><see cref="int"/> indicating status.</returns>
        public int createPath(string filepath)
        {
            try
            {
                Directory.CreateDirectory(filepath);
                Console.WriteLine("CreatePath - Path creation successful");
                return (0);
            }
            catch
            {
                Console.WriteLine("CreatePath - Path creation failed");
                return (1);
            }
        }

        /// <summary>
        /// Creates the file specified with provided data string
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// <param name="data">A list of strings of data to be put into the file</param>
        /// </summary>
        /// <returns><see cref="int"/> indicating status</returns>
        public int create<T>(string filename, string filepath, List<T> data)
        {
            if (exists(filename, filepath))
            {
                Console.WriteLine("Create, list <T> data - File already exists");
                return (0);
            }
            else
            {
                //Need to verify if the directory exists
                if (!Directory.Exists(filepath))
                    createPath(filepath);

                var file = File.Create(path: filepath + "/" + filename);
                file.Close();
                Console.WriteLine(1);

                //Our file to write to
                using (StreamWriter sw = File.AppendText(filepath + "/" + filename))
                {
                    try
                    {
                        foreach (var s in data)
                        {
                            sw.WriteLine(s);
                        }
                        sw.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Create, list <T> data - File creation failed");
                        Console.WriteLine(e);
                    }
                }
                Console.WriteLine("Create, list <T> data - File creation successful");
                return (1);
            }
        }

        //START: File deletion methods
        /// <summary>
        /// Deletes the file specified
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// </summary>
        /// <returns><see cref="int"/> indicating status</returns>
        public int delete(string filename, string filepath)
        {
            if (!exists(filename, filepath))
            {
                Console.WriteLine("Delete - File doesn't exist");
                return (0);
            }
            else
            {
                try
                {
                    File.SetAttributes(filepath + "/" + filename, FileAttributes.Normal);
                    File.Delete(filepath + "/" + filename);
                    Console.WriteLine("Delete - File deletion successful");
                    return (1);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Delete - File deletion failed");
                    return (2);
                }
            }
        }

        /// <summary>
        /// Reads the file specified
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// </summary>
        /// <returns><see cref="List"/> file data</returns>
        public List<string> read(string filename, string filepath)
        {
            //First, we check if the file exists, if not, return a blank list
            if (!exists(filename, filepath))
                return (new List<string>(new string[] {""}));

            //The streamreader
            StreamReader sr = new StreamReader(filepath + "/" + filename);

            //NOTE: To not skip over data we need to store the line
            string line;
            //The list of data
            List<string> data = new List<string>();

            //Iter thru all the lines in the file
            line = sr.ReadLine();
            do
            {
                data.Add(line);

                //Now we get the next line
                line = sr.ReadLine();
            } while (line != null);

            //Close the file and return the list
            sr.Close();
            return data;
        }
    }
}
