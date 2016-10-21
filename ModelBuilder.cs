using System;
using System.Configuration;

using NLog;

using ModelBuilder.DatabaseSetup;
using ModelBuilder.InitialData;


// COmment by Rajat
// Second comment by Rajat

// Hi There

// New comment
namespace ModelBuilder

{

    public enum ModelBuilderErrors { NewParam = 0, DBCreateFail = 1 }

    class ModelBuilder
    {
        /// <summary>
        /// Static instance of the Nlog logger for this class
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constant string keys that match those in the App.config file
        /// </summary>
        public static string KeyOutputFileName = "ModelBuilderSQLScriptFileOutput";
        public static string KeyDataPath = "ModelBuilderDataPath";
        public static string KeyOutputPath = "ModelBuilderOutputPath";
        public static string KeyCreateOutputFile = "ModelBuilderCreateSQLScriptFile";


        /// <summary>
        /// The program's entry point.
        /// 
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void Main(string[] args)
        {
            bool verbose = bool.Parse(ConfigurationManager.AppSettings[ModelBuilder.KeyCreateOutputFile]);
            
            logger.Info("ModelBuilder::Started.");

            if (args != null && args.Length == 0)
            {
                PrintUsage();
            }

            int argsRead = 0;
            while ((args.Length - argsRead) > 0)
            {
                string arg = args[argsRead].ToLower();

                if (argsRead == 0)
                {
                    argsRead++;

                    if (arg == "create")
                    {
                        logger.Debug("ModelBuilder:: Create DB");

                        try
                        {
                            DatabaseAdmin.Create(verbose);
                            DefaultUsers.GenerateDefaultUsers(verbose);
                        }
                        catch (Exception e)
                        {

                            Console.Write(e.Message);
                            logger.Error(e.Message);

                            if (e.InnerException != null)
                            {
                                Console.Write(e.InnerException.Message);
                                logger.Error(e.InnerException.Message);
                            }

                            PrintErrorAndExit("Database could not be correctly created.", ModelBuilderErrors.DBCreateFail);
                        }

                        logger.Debug("ModelBuilder:: Create DB.... Done.");
                    }
                    else if (arg == "printusers")
                    {
                        logger.Debug("ModelBuilder:: Printing Test Users");

                        DefaultUsers.GenerateSummaryFile();

                        logger.Debug("ModelBuilder:: Printing Test Users... Done.");
                    }
                }

                logger.Info("ModelBuilder::Completed.");
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("[Press any key to continue ...]");
            Console.ReadKey(true);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: ModelBuilder { Create }");
            Console.WriteLine("       Note that the tool is configured via the connection string and keys in the App.config file.");
            Console.WriteLine("       Logging is sent to file as configured in the NLog config file:  (by default) \\Program Data\\AAAS\\AppDev\\ModelBuilder-{date}");
            logger.Warn("Usage: ModelBuilder { Create }");
        }

        private static void PrintErrorAndExit(string error, ModelBuilderErrors errorNo)
        {
            logger.Warn("Error: " + error);
            Console.WriteLine("Error: " + error);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("[Press any key to continue ...]");
            Console.ReadKey(true);

            System.Environment.Exit((int)errorNo);
        }


    }

}
