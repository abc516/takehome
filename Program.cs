using System;
using System.IO;
using System.Collections.Generic;
namespace takehome
{
    class Program
    {
        static Dictionary<string, FileInfo> nameToFile = new Dictionary<string, FileInfo>();
        public static Dictionary<long, FileInfo> byteToFile = new Dictionary<long, FileInfo>();
        public static Dictionary<string, List<FileInfo>> atrToFile = new Dictionary<string, List<FileInfo>>();

        static void Main(string[] args)
        {
            bool isValidInputDirectory = false;
            string dirName = "";
            while(!isValidInputDirectory){

                Console.WriteLine("Please enter the absolute path of a directory on your filesystem you would like to parse.");
                dirName = Console.ReadLine();
                if(!String.IsNullOrWhiteSpace(dirName) && Directory.Exists(dirName)) isValidInputDirectory = true;
                else{
                    Console.WriteLine($"The provided directory path {dirName} is invalid, please try again.");
                }
            }
            
            ParseData(dirName);

            Search();
        }

        private static void ParseData(string startDirName){
            try
            {                
                string[] files = System.IO.Directory.GetFiles(startDirName);
                foreach(string file in files){ 
                        System.IO.FileInfo fi = new System.IO.FileInfo(file);
                        nameToFile[fi.Name] = fi;
                        byteToFile[fi.Length] = fi;
                        if(!atrToFile.ContainsKey(fi.Extension)){
                            atrToFile[fi.Extension] = new List<FileInfo>();
                        } 
                        atrToFile[fi.Extension].Add(fi);
                }
                
                string[] subDirs = System.IO.Directory.GetDirectories(startDirName);
                foreach(string subDir in subDirs){
                    ParseData(subDir);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Ran into an exception, cannot parse {startDirName} any further.");
            }
        }

        private static void Search(){
            bool shouldSearch = true;

            while(shouldSearch){
                List<FileInfo> results = new List<FileInfo>();
                // ask for file name
                string fileNameStr;
                Console.WriteLine("Searching has begun. Note, at least 1 filter criteria must be specified to return any results");
                Console.WriteLine("Please enter a search string for file name (e.g. 'foo') or press the 'enter' key to skip this filter criteria. ");
                fileNameStr = Console.ReadLine();

                if(!String.IsNullOrEmpty( fileNameStr)){
                    foreach(FileInfo fi in nameToFile.Values){
                        if(fi.Name.ToLower().IndexOf(fileNameStr.Trim().ToLower()) > -1){
                            results.Add(fi);
                        }
                    }
                }
                // ask for file size
                Console.WriteLine("Please enter the maximum size of the file to search for in bytes (e.g. '1000') or press the 'enter' key to skip this filter criteria.");
                string sizeStr = Console.ReadLine();

                if(!String.IsNullOrWhiteSpace(sizeStr)){
                    if(long.TryParse(sizeStr.Trim(), out long maxFileSizeInBytes)){
                        if(results.Count > 0){
                            results.RemoveAll((FileInfo fi) => {
                                return fi.Length > maxFileSizeInBytes;
                            });
                        } else {
                            foreach(FileInfo fi in byteToFile.Values){
                                if(fi.Length <= maxFileSizeInBytes) results.Add(fi);
                            }
                        }
                    }
                }
                // ask for file type
                Console.WriteLine("Please enter the name of the file extension you would like to search for (e.g. '.docx') or press the 'enter' key to skip this filter criteria. ");
                // show results
                string atrStr = Console.ReadLine();
                if(!String.IsNullOrWhiteSpace(atrStr )){
                    if(results.Count > 0){
                        results.RemoveAll((FileInfo fi) => {
                            return fi.Extension.ToLower() != atrStr.Trim().ToLower();
                        });
                    } else {
                        if(atrToFile.ContainsKey(atrStr)){
                            results.AddRange(atrToFile[atrStr]);
                        }
                    }
                }

                foreach(FileInfo fi in results){
                    Console.WriteLine($"Path: {fi.FullName}, Size: {fi.Length}, type: {fi.Extension}");
                }

                Console.WriteLine("Thanks! Please type in 'y' to search again, or any other character to exit the application");

                string searchAg = Console.ReadLine();
                if(searchAg == null || searchAg.ToLower() != "y") shouldSearch = false;
            }
        }    
    }
}
