using System;

namespace Winner
{
    class Winner
    {
        static void Main(string[] args)
        {
            string input = GetInputFile(args);
            string ouptut = GetOutputFile(args);

            Console.WriteLine(input);
            Console.WriteLine(ouptut);

            if(input == "")
            {
                Console.WriteLine("Missing input file");
            }

            if(ouptut == "")
            {
                Console.WriteLine("Missing output file");
            }
            
        }

        private static string GetInputFile(string[] args)
        {
            return GetFile("--in", args);
        }

        private static string GetOutputFile(string[] args)
        {
            return GetFile("--out", args);
        }

        private static string GetFile(string option, string[] args)
        {
            try
            {
                if(args[0] == option)
                {
                    return args[1];
                }
            
                if(args[2] == option)
                {
                    return args[3];
                }

                return "";
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception caught: {0}.", e);
                return "";
            }
        }
    }
}