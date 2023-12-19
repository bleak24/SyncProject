using SyncProject.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SyncProject
{
    internal class Program
    {
        static string sourceFolderPath;
        static string replicaFolderPath;
        static string syncIntervalMinutes;
        static string logFilePath;
        static void Main(string[] args)
        {
            if (args.Length == 4)
            {
                sourceFolderPath = args[0];
                replicaFolderPath = args[1];
                syncIntervalMinutes = args[2];
                logFilePath = args[3];

                Console.WriteLine($"Synchronization will occur every {syncIntervalMinutes} minute(s).");
                LogHelper.WriteToLog(logFilePath, $"Synchronization will occur every {syncIntervalMinutes} minute(s).");

                System.Timers.Timer aTimer = new System.Timers.Timer();
                aTimer.Elapsed += new ElapsedEventHandler(timer_Tick);
                aTimer.Interval = Convert.ToInt32(syncIntervalMinutes) * 60 * 1000;
                aTimer.Enabled = true;
                aTimer.AutoReset = true;
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Incorrect number of arguments provided, application will exit now");
            }
        }

        private static void PerformSynchronization() 
        {
            List<string> sourceFolders = new List<string>();
            List<string> sourceFiles = new List<string>();
            List<string> replicaFolders = new List<string>();
            List<string> replicaFiles = new List<string>();

            try
            {
                sourceFolders = InOutHelper.GetDirectories(sourceFolderPath).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLog(logFilePath, $"Exception occurred when getting source folders: {ex.Message}");
            }

            try
            {
                sourceFiles = InOutHelper.GetFiles(sourceFolderPath).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLog(logFilePath, $"Exception occurred when getting source folder files: {ex.Message}");
            }

            try
            {
                replicaFolders = InOutHelper.GetDirectories(replicaFolderPath).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLog(logFilePath, $"Exception occurred when getting replica folders: {ex.Message}");
            }

            try
            {
                replicaFiles = InOutHelper.GetFiles(replicaFolderPath).ToList();
            }
            catch (Exception ex)   
            {
                LogHelper.WriteToLog(logFilePath, $"Exception occurred when getting replica folder files: {ex.Message}");
            }
           
            //files to delete
            var filesToDelete = replicaFiles.Except(sourceFiles);
            foreach (var file in filesToDelete)
            {
                try
                {
                    InOutHelper.DeleteFile($"{replicaFolderPath}{file}");
                    Console.WriteLine($"DELETED file {replicaFolderPath}{file}");
                    LogHelper.WriteToLog(logFilePath, $"DELETED file {replicaFolderPath}{file}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred when DELETING file {replicaFolderPath}{file}: {ex.Message}");
                    LogHelper.WriteToLog(logFilePath, $"Exception occurred when DELETING file {replicaFolderPath}{file}: {ex.Message}");
                }
            }

            //folders to delete
            var foldersToDelete = replicaFolders.Except(sourceFolders);
            foreach (var folder in foldersToDelete)
            {
                try
                {
                    InOutHelper.DeleteFolder($"{replicaFolderPath}{folder}");
                    Console.WriteLine($"DELETED folder {replicaFolderPath}{folder}");
                    LogHelper.WriteToLog(logFilePath, $"DELETED folder {replicaFolderPath}{folder}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred when DELETING folder {replicaFolderPath}{folder}: {ex.Message}");
                    LogHelper.WriteToLog(logFilePath, $"Exception occurred when DELETING folder {replicaFolderPath}{folder}: {ex.Message}");
                }
            }

            //folders to create
            var foldersToCreate = sourceFolders.Except(replicaFolders);
            foreach(var folder in foldersToCreate)
            {
                try
                {
                    InOutHelper.CreateFolder($"{replicaFolderPath}{folder}");
                    Console.WriteLine($"CREATED folder {replicaFolderPath}{folder}");
                    LogHelper.WriteToLog(logFilePath, $"CREATED folder {replicaFolderPath}{folder}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred when CREATING folder {replicaFolderPath}{folder}: {ex.Message}");
                    LogHelper.WriteToLog(logFilePath, $"Exception occurred when CREATING folder {replicaFolderPath}{folder}: {ex.Message}");
                }
            }

            //files to create
            var filesToCreate = sourceFiles.Except(replicaFiles);
            foreach (var file in filesToCreate)
            {
                try
                {
                    InOutHelper.CreateFile($"{replicaFolderPath}{file}");
                    Console.WriteLine($"CREATED file {replicaFolderPath}{file}");
                    LogHelper.WriteToLog(logFilePath, $"CREATED file {replicaFolderPath}{file}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred when CREATING file {replicaFolderPath}{file}: {ex.Message}");
                    LogHelper.WriteToLog(logFilePath, $"Exception occurred when CREATING file {replicaFolderPath}{file}: {ex.Message}");
                }
            }

            //files to overwrite
            var filesToUpdate = sourceFiles.Intersect(replicaFiles);
            foreach (var file in filesToUpdate)
            {
                try
                {
                    if (InOutHelper.OverwriteFile($@"{sourceFolderPath}{file}", $@"{replicaFolderPath}{file}"))
                    {
                        Console.WriteLine($"UPDATED file {replicaFolderPath}{file}");
                        LogHelper.WriteToLog(logFilePath, $"UPDATED file {replicaFolderPath}{file}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred when UPDATING file {replicaFolderPath}{file}: {ex.Message}");
                    LogHelper.WriteToLog(logFilePath, $"Exception occurred when UPDATING file {replicaFolderPath}{file}: {ex.Message}");
                }
            }
        }


        private static void TestSync()
        {
            sourceFolderPath = @"C:\Testpath\sourceFolder";
            replicaFolderPath = @"C:\Testpath\replicaFolder";
            syncIntervalMinutes = "1";
            logFilePath = @"C:\Testpath";

            LogHelper.WriteToLog(logFilePath, $"Synchronization will begin every {syncIntervalMinutes} minutes.");

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(timer_Tick);
            aTimer.Interval = 2000;
            aTimer.Enabled = true;
            Console.ReadKey();
        }

        private static void timer_Tick(object source, ElapsedEventArgs e)
        {
            PerformSynchronization();
        }
    }
}
