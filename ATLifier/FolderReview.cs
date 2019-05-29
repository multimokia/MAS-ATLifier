using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

using crypt;
namespace folderStuffs
{
    class FolderReview
    {
        //NOTE: Used for reading/writing encrypted files as needed
        Cryptography cryptgen = new Cryptography();

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
        /// <param name="encrypt">A bool for whether or not we want to encrypt the data</param>
        /// <param name="overwrite">A bool for whether we want to overwrite the file or not</param>
        /// </summary>
        /// <returns><see cref="int"/> indicating status</returns>
        public int create(string filename, string filepath, string data, bool encrypt = false, bool overwrite = false)
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
                    //Encrypt if needed
                    if (encrypt)
                        sw.WriteLine(cryptgen.Encrypt(data));
                    else
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
        /// <param name="encrypt">A bool for whether or not we want to encrypt the data</param>
        /// <param name="overwrite">A bool for whether we want to overwrite the file or not</param>
        /// </summary>
        /// <returns><see cref="int"/> indicating status</returns>
        public int create(string filename, string filepath, List<string> data, bool encrypt=false, bool overwrite=false)
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
                        //Encrypt if needed
                        if (encrypt)
                            sw.WriteLine(cryptgen.Encrypt(s));
                        else
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

        /// <summary>
        /// Creates/modifies the file specified with configs list
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// <param name="config">A List of configs to add onto the existing json.</param>
        /// <param name="overwrite">A bool true if you want to overwrite the json, false by default</param>
        /// </summary>
        /// <returns><see cref="string"/> indicating status (to be sent as a message by bbn)</returns>
        public string createJson<T>(string filename, string filepath, List<T> obj, bool overwrite = false)
        {
            //Sanity Check
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            //If we want to overwrite the file, we just delete it
            if (overwrite)
                delete(filename, filepath);

            //Make a new one if the file doesn't exist
            if (!exists(filename, filepath))
                create(filename, filepath);

            var listConfigs = fixReadJson<T>(filename, filepath);
            listConfigs.AddRange(obj);

            try
            {
                File.WriteAllText(filepath + "/" + filename, JsonConvert.SerializeObject(listConfigs, Formatting.Indented));
                Console.WriteLine("CreateJson - File creation successful");
                return ("Success! Your configs were added!");
            }
            catch (Exception e)
            {
                Console.WriteLine("CreateJson - File creation failed");
                Console.WriteLine(e);
                return ("Your configs couldn't be added");
            }
        }

        /// <summary>
        /// Writes a json file with the name and path specified
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// </summary>
        /// <returns>List of configs</returns>
        public void writeJson<T>(string filename, string filepath, List<T> data)
        {
            File.WriteAllText(filepath + "/" + filename, JsonConvert.SerializeObject(data, Formatting.Indented));
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

        //START: File reading methods
        /// <summary>
        /// Reads the file specified
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// </summary>
        /// <returns>List of specified type</returns>
        public List<T> readJson<T>(string filename, string filepath)
        {
            if (exists(filename, filepath))
                return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filepath + "/" + filename)) ?? new List<T>();
            else
                return new List<T>();
        }

        /// <summary>
        /// attempts to read the json.async if fails, fixes until readable, and returns the readable
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// </summary>
        /// <returns>List of specified type</returns>
        public List<T> fixReadJson<T>(string filename, string filepath)
        {
            if (exists(filename, filepath))
            {
                fixJson<T>(filename,filepath);
                return readJson<T>(filename, filepath);
            }
            else
                return new List<T>();
        }

        /// <summary>
        /// Reads the file specified
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// <param name="encrypted">Should the file be read as an encrypted file</param>
        /// </summary>
        /// <returns><see cref="List"/> file data</returns>
        public List<string> read(string filename, string filepath, bool encrypted = false)
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
                //Decrypt if needed
                if (encrypted)
                    data.Add(cryptgen.Decrypt(line));
                else
                    data.Add(line);

                //Now we get the next line
                line = sr.ReadLine();
            } while (line != null);

            //Close the file and return the list
            sr.Close();
            return data;
        }

        /// <summary>
        /// Attempts to fix json files
        /// <param name="filename">A string filename.</param>
        /// <param name="filepath">A local path for the file.</param>
        /// </summary>
        public void fixJson<T>(string filename, string filepath)
        {
            bool canRead = true;
            do
            {
                //We need to first, read the file
                try
                {
                    //If we can read the json, then we just do nothing and fall thru to the return
                    readJson<T>(filename, filepath);
                    canRead = true;
                }
                catch
                {
                    //Otherwise, if we fail to read, we need to read it as a txt and get rid of the broken entry
                    List<string> bad_data = read(filename, filepath);

                    //First, we need to check if we attempted before, because now there's an extra line
                    if (!canRead)
                        bad_data.RemoveAt(bad_data.Count-1);

                    //Now we start cleaning up
                    if (bad_data[bad_data.Count - 1].EndsWith(','))
                        //If we end in a comma, then we try and just delete the comma and see if we're good
                        bad_data[bad_data.Count - 1] = bad_data[bad_data.Count - 1].Substring(0,bad_data[bad_data.Count - 1].Length-1);

                    else
                        //Otherwise, we just delete the line itself
                        bad_data.RemoveAt(bad_data.Count-1);

                    //Add the terminal character
                    bad_data.Add("]");

                    //Now we write this all to file again
                    try
                    {
                        create(filename,filepath,bad_data,overwrite: true);
                    }
                    catch
                    {
                        Console.WriteLine("FixJson - File permission error-- dumping log file");
                    }

                    //Setup another pass to see if we can read it
                    canRead = false;
                }
            } while (!canRead);
        }
    }
}
