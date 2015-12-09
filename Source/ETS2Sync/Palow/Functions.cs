using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Palow.Library
{
    /// <summary>
    /// Functions Library
    /// </summary>
    public class Functions
    {



        /// <summary>
        /// Writes cookies into a file
        /// </summary>
        /// <param name="file">Output filename</param>
        /// <param name="cookieJar">Input cookies</param>
        public static void WriteCookiesToDisk(string file, CookieContainer cookieJar)
        {
            Stream stream;
            using (stream = File.Create(file))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookieJar);

                }
                catch (Exception)
                {
                    //Console.Out.WriteLine("Problem writing cookies to disk: " + e.GetType());
                }
            }
            stream.Close();
        }
        /// <summary>
        /// Reads cookies from an file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static CookieContainer ReadCookiesFromDisk(string file)
        {
            Stream stream;
            try
            {
                using (stream = File.Open(file, FileMode.Open))
                {
                    //Console.Out.Write("Reading cookies from disk... ");
                    BinaryFormatter formatter = new BinaryFormatter();
                    //Console.Out.WriteLine("Done.");
                    stream.Close();
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                //Console.Out.WriteLine("Problem reading cookies from disk: " + e.GetType());
                return new CookieContainer();
            }


        }

        /// <summary>
        /// Writes a stream into a file.
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="outputFile">Output filename</param>
        /// <param name="fileMode">Filestream file mode</param>
        public static void StreamToFile(Stream inputStream, string outputFile, FileMode fileMode)
        {
            if (inputStream == null)
                throw new ArgumentNullException("No input stream.");

            if (String.IsNullOrEmpty(outputFile))
                throw new ArgumentException("Argument null or empty.", "outputFile");

            FileStream outputStream;
            using (outputStream = new FileStream(outputFile, fileMode, FileAccess.Write))
            {
                int cnt = 0;
                const int LEN = 4096;
                byte[] buffer = new byte[LEN];

                while ((cnt = inputStream.Read(buffer, 0, LEN)) != 0)
                    outputStream.Write(buffer, 0, cnt);
            }
            outputStream.Close();


        }

        /// <summary>
        /// Converts a byte array to a string.
        /// </summary>
        /// <param name="arr">Input byte array</param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] arr)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(arr);
        }
        /// <summary>
        /// Reads a file by filename
        /// </summary>
        /// <param name="sFilename">Input filename</param>
        /// <returns>File content</returns>
        static public string ReadFile(String sFilename)
        {
            string sContent = "";

            if (File.Exists(sFilename))
            {
                StreamReader myFile = new StreamReader(sFilename);
                sContent = myFile.ReadToEnd();
                myFile.Close();
            }
            return sContent;
        }

        /// <summary>
        /// Writes passed content into a file
        /// </summary>
        /// <param name="filename">Output filename</param>
        /// <param name="data">Input content</param>
        static public void WriteFile(String filename, String data)
        {
            StreamWriter myFile = File.CreateText(filename);
            myFile.Write(data);
            myFile.Close();
        }



        static public Type ObjectFromJsonFile<Type>(String filename, Boolean useExtensions)
        {
            fastJSON.JSON.Parameters = new fastJSON.JSONParameters { UseExtensions = useExtensions, UseFastGuid = false, UseUTCDateTime = false };
            String json = Functions.ReadFile(filename);

            return fastJSON.JSON.ToObject<Type>(json);
        }



        /// <summary>
        /// Default Output für ExceptionText
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetExceptionText(Exception e)
        {
            return e.ToString();
        }


        public static string GetAssemblyDirectory()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

    }
}
