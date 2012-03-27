//The MIT License

//Copyright (c) 2010 Jouni Uusimaa

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Security.Permissions;
using System.Collections;

[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum,
    ViewAndModify = "HKEY_CURRENT_USER")]

namespace FortuneSharp
{
    class Program
    {
        #region Assemblies
        /// <summary>
        /// Gets the company name from the assembly information.
        /// </summary>
        private static string AssemblyCompany
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().
                    GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        /// <summary>
        /// Gets the program version from the assembly information.
        /// </summary>
        private static string AssemblyVersion
        {
            get
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." +
                    Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." +
                    Assembly.GetExecutingAssembly().GetName().Version.MajorRevision.ToString();
                return version;
            }
        }

        /// <summary>
        /// Gets the copyright from the assembly information.
        /// </summary>
        private static string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().
                    GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        /// <summary>
        /// Gets the product name from the assembly information.
        /// </summary>
        private static string AssemblyProduct
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        /// <summary>
        /// Gets the product description from the assembly information.
        /// </summary>
        private static string AssemblyDescription
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }
        #endregion

        /// <summary>
        /// Gets the install path from the registry.
        /// </summary>
        /// <returns>Program install path.</returns>
        private static string getPath()
        {
            string path = "";

            try
            {
                RegistryKey softKey = Registry.CurrentUser.OpenSubKey("Software", true);
                RegistryKey pathKey = softKey.OpenSubKey(AssemblyProduct + " " + AssemblyVersion, true);
                path = pathKey.GetValue("FortunePath").ToString();
                softKey.Close();
                pathKey.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return path;
        }

        /// <summary>
        /// Generates a positive random number within range [0, pMax].
        /// </summary>
        /// <param name="pMax">Specified maximum.</param>
        /// <returns>A 32-bit signed integer greater than or equal to zero, and less than pLines; 
        /// that is, the range of return values includes zero but not pLines.</returns>
        private static int getRandomNumber(int pMax)
        {
            int result = 0;
            int seed = DateTime.Now.Millisecond;
            Random randObj = new Random(seed);

            result = randObj.Next(pMax);
            return result;
        }

        /// <summary>
        /// Reads one fortune from the file which name is passes as a parameter.
        /// </summary>
        /// <param name="pFile">File name.</param>
        /// <param name="pShort">Get short cookie.</param>
        /// <param name="pLong">Get long cookie.</param>
        /// <param name="pLength">The longest length of cookie.</param>
        /// <returns>String containing on fortune.</returns>            
        private static string getRandomFortune(string pFile, bool pShort, bool pLong, int pLength)
        {
            string fortune = "";
            string line = "";
            System.Collections.ArrayList al = new ArrayList();
            int numberOfFortunes = 0;
            int randomNumber = 0;
            int iterations = 0;

            // find the number of fortunes in the file
            // fortunes are separated with "%" lines
            try
            {
                using (StreamReader sr = new StreamReader(pFile))
                {
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        if (line.CompareTo("%") != 0)
                        {
                            fortune += line + Environment.NewLine;
                        }
                        else
                        {
                            numberOfFortunes++;
                            al.Add(fortune);
                            fortune = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // if it's a BOFH excuse...
            randomNumber = getRandomNumber(al.Count);
            if (pFile.Contains("bofh"))
            {
                fortune = "BOFH Excuse #" + randomNumber + ":\n\t";
                fortune += al[randomNumber].ToString();
                //return fortune;
            }

            // ...or if short fortune was requested...  
            else if (pShort)
            {
                iterations = 0;
                while (al[randomNumber].ToString().Length > 160)
                {
                    randomNumber = getRandomNumber(al.Count);
                    iterations++;
                    if (iterations >= al.Count)
                        break;
                }
                if (iterations != al.Count)
                    fortune += al[randomNumber].ToString();
            }
            // ...or long fortune was requested...
            else if (pLong)
            {
                iterations = 0;
                while (al[randomNumber].ToString().Length < 160)
                {
                    randomNumber = getRandomNumber(al.Count);
                    iterations++;
                    if (iterations >= al.Count)
                        break;
                }
                if (iterations != al.Count)
                    fortune += al[randomNumber].ToString();
            }
            // ...or fortune with maximum length was requested...
            else if (pLength > 0)
            {
                iterations = 0;
                while (al[randomNumber].ToString().Length > pLength)
                {
                    randomNumber = getRandomNumber(al.Count);
                    iterations++;
                    if (iterations >= al.Count)
                        break;
                }
                if (iterations != al.Count)
                    fortune += al[randomNumber].ToString();
            }
            // ...and finally if no special requirements
            else if (iterations == 0)
            {
                fortune += al[randomNumber].ToString();
            }
            return fortune;
        }

        /// <summary>
        /// Shows the MIT License.
        /// </summary>
        private static void showLicense()
        {
            Console.WriteLine("\nThe MIT License");
            
            Console.WriteLine("\nCopyright (c) 2008 Jouni Uusimaa");

            Console.WriteLine("\nPermission is hereby granted, free of charge, to any person obtaining a copy");
            Console.WriteLine("of this software and associated documentation files (the \"Software\"), to deal");
            Console.WriteLine("in the Software without restriction, including without limitation the rights");
            Console.WriteLine("to use, copy, modify, merge, publish, distribute, sublicense, and/or sell");
            Console.WriteLine("copies of the Software, and to permit persons to whom the Software is");
            Console.WriteLine("furnished to do so, subject to the following conditions:");

            Console.WriteLine("\nThe above copyright notice and this permission notice shall be included in");
            Console.WriteLine("all copies or substantial portions of the Software.");

            Console.WriteLine("\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR");
            Console.WriteLine("IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,");
            Console.WriteLine("FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE");
            Console.WriteLine("AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER");
            Console.WriteLine("LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,");
            Console.WriteLine("OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN");
            Console.WriteLine("THE SOFTWARE.");
        }

        /// <summary>
        /// Shows commandline help.
        /// </summary>
        private static void showHelp()
        {
            Console.WriteLine("\nNAME");
            Console.WriteLine("\tfortune - print a random, hopefully interesting, adage");
            
            Console.WriteLine("\nSYNOPSIS");
            Console.WriteLine("\n\tfortune [-a -c -f -o -s -v] [file]");
            
            Console.WriteLine("\nDESCRIPTION");
            Console.WriteLine("\tWhen  fortune  is run with no arguments it prints out a random epigram.");
            Console.WriteLine("\tEpigrams are divided into several categories, where  each  category  is");
            Console.WriteLine("\tsub-divided  into those which are potentially offensive and those which");
            Console.WriteLine("\tare not.");
           
            Console.WriteLine("\nOptions");
            Console.WriteLine("\t-a\tChoose from all lists of maxims, both offensive and  not.");
            Console.WriteLine("\t\t(See the -o option for more information on offensive fortunes.)");
            
            Console.WriteLine("\n\t-c\tShow the cookie file from which the fortune came.");
            
            Console.WriteLine("\n\t-f\tPrint out the list of files which would be searched,\n\t\tbut  don�t print a fortune.");

            Console.WriteLine("\n\t-l\tLong  dictums  only.   See -n on how ��long�� is defined in this");
            Console.WriteLine("\t\tsense.");

            Console.WriteLine("\n\t-n length");
            Console.WriteLine("\t\tSet the longest fortune length (in characters) considered to  be");
            Console.WriteLine("\t\t��short��  (the  default is 160).  All fortunes longer than this");
            Console.WriteLine("\t\tare considered ��long��.  Be careful!  If you set the length too");
            Console.WriteLine("\t\tshort  and  ask for short fortunes, or too long and ask for long");
            Console.WriteLine("\t\tones, fortune goes into a never-ending thrash loop.");
            
            Console.WriteLine("\n\t-o\tChoose only from potentially offensive aphorisms. The -o option");
            Console.WriteLine("\t\tis ignored if a fortune directory is specified.");
            Console.WriteLine("\n\t\tPlease,  please,  please request a potentially offensive fortune");
            Console.WriteLine("\t\tif and only if you believe, deep in your  heart,  that  you  are");
            Console.WriteLine("\t\twilling  to  be  offended.  (And  that you�ll just quit using -o");
            Console.WriteLine("\t\trather than give us grief about it, okay?)");
            Console.WriteLine("\n\t\t... let us keep in mind the basic governing  philosophy  of  The");
            Console.WriteLine("\t\tBrotherhood, as handsomely summarized in these words: we believe");
            Console.WriteLine("\t\tin healthy, hearty laughter -- at the expense of the whole human");
            Console.WriteLine("\t\trace, if needs be.  Needs be.");
            Console.WriteLine("\t\t\t--H. Allen Smith, \"Rude Jokes\"\n");

            Console.WriteLine("\n\t-s\tShort  apothegms  only.  See -n on which fortunes are considered");
            Console.WriteLine("\t\t��short��.");

            
            Console.WriteLine("\n\t-v\tDisplay version info");
            
            Console.WriteLine("\n\t/?\tDisplay this help.");
        }

        /// <summary>
        /// Print out the list of cookie files.
        /// </summary>
        private static void showCookies()
        {
            string[] filesNormal = null;
            string[] filesOff = null;

            // get the cookie files in sub directories
            try
            {
                filesNormal = Directory.GetFiles(getPath() + "Cookies", "*", SearchOption.TopDirectoryOnly);
                filesOff = Directory.GetFiles(getPath() + "Off", "*", SearchOption.TopDirectoryOnly);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            foreach (string cookie in filesNormal)
            {
                Console.WriteLine(cookie);
            }
            foreach (string cookie in filesOff)
            {
                Console.WriteLine(cookie);
            }
        }

        /// <summary>
        /// Main program. 
        /// TODO:
        /// - if long fortune is requested and random file does not contain any long
        ///   fortunes, get new random file
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // flags to determine cookie
            bool offensive = false;
            bool printCookie = false;
            bool all = false;
            bool selectedFile = false;
            bool shortCookie = false;
            bool longCookie = false;

            int cookieLenght = 0;
            
            string[] files = null;
            string cookieFile = "";
            string randomFile = "";
            string fortune = "";

            System.Collections.ArrayList fortuneList = new ArrayList();

            // check command line arguments
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "/?":
                            showHelp();
                            return;
                        case "-a":
                            all = true;
                            break;
                        case "-c":
                            printCookie = true;
                            break;
                        case "-f":
                            showCookies();
                            return;
                        case "-l":
                            longCookie = true;
                            break;
                        case "-n":
                            if (args[i + 1] != null)
                            {
                                cookieLenght = System.Convert.ToInt16(args[i + 1], 10);
                                i++;
                            }
                            break;
                        case "-o":
                            offensive = true;
                            break;
                        case "-s":
                            shortCookie = true;
                            break;
                        case "-v":
                            Console.WriteLine(AssemblyProduct +
                                " v" + AssemblyVersion);
                            showLicense();
                            return;
                        default:
                            cookieFile = args[i];
                            selectedFile = true;
                            break;
                    }
                }
            }

            try
            {
                // only non-offensive (=default) cookies
                if (!offensive && !all && !selectedFile)
                {
                    files = Directory.GetFiles(getPath() + "Cookies", "*", SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        fortuneList.Add(file);
                    }
                }
                // only offensive cookies
                else if (offensive && !all && !selectedFile)
                {
                    files = Directory.GetFiles(getPath() + "Off", "*", SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        fortuneList.Add(file);
                    }
                }
                // all cookies
                else if (all && !selectedFile)
                {
                    files = Directory.GetFiles(getPath() + "Cookies", "*", SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        fortuneList.Add(file);
                    }
                    files = Directory.GetFiles(getPath() + "Off", "*", SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        fortuneList.Add(file);
                    }
                }
                // selected cookie file
                else if (selectedFile)
                {
                    if (!offensive)
                    {
                        files = Directory.GetFiles(getPath() + "Cookies", cookieFile, SearchOption.TopDirectoryOnly);
                        foreach (string file in files)
                        {
                            fortuneList.Add(file);
                        }
                    }
                    else
                    {
                        files = Directory.GetFiles(getPath() + "Off", cookieFile, SearchOption.TopDirectoryOnly);
                        foreach (string file in files)
                        {
                            fortuneList.Add(file);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
           
            // get random fortune from the file
            while (fortune.Length == 0)
            {
                // get random file number
                try
                {
                    randomFile = fortuneList[getRandomNumber(fortuneList.Count)].ToString();
                }
                catch (Exception)
                {
                    Console.WriteLine("No fortunes found.");
                    return;
                }
                fortune = getRandomFortune(randomFile, shortCookie, longCookie, cookieLenght);
            }
#if DEBUG
            Console.Write("DEBUG");
            Console.Write("\tAll:{0}", all.ToString());
            Console.Write("\tFile: {0}", cookieFile);
            Console.Write("\t\tLong:{0}", longCookie.ToString());
            Console.Write("\tLength:{0}", cookieLenght.ToString());
            Console.Write("\nDEBUG");
            Console.Write("\tOffensive:{0}", offensive.ToString());
            Console.Write("\tShort:{0}", shortCookie.ToString());
            Console.Write("\tChar count:{0}", fortune.Length.ToString());
            Console.Write("\tNumber of fortune files:{0}\n", fortuneList.Count);
#endif 
            // print the cookie
            if (!printCookie)
            {
                Console.WriteLine("\n{0}", fortune);
            }
            else
            {
                int index = randomFile.LastIndexOf("\\");
                randomFile = randomFile.Remove(0, index + 1);
                Console.WriteLine("\n{0}\n{1}", fortune, randomFile);
            }
        }
    }
}
