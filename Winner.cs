using System;

namespace Winner
{
    class Winner
    {
        static void Main(string[] args)
        {
            Game game = new Game(args);
            game.Play();
        }
    }

    class Game
    {
        private static readonly string INPUT_FILE_OPTION = "--in";
        private static readonly string OUTPUT_FILE_OPTION = "--out";

        private File _inputFile;
        private File _outputFile;
        public Game(string[] args)
        {
            _inputFile = new InputFile(args, INPUT_FILE_OPTION);
            _outputFile = new OutputFile(args, OUTPUT_FILE_OPTION);
        }

        public void Play()
        {
            // If output file is missing end game.
            if (String.IsNullOrEmpty(_outputFile.Name))
                return;

            try
            {
                string input = ((InputFile)_inputFile).Parse();
                Console.WriteLine(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Writing \"ERROR\" to output, exception: {0}", ex.Message);
                ((OutputFile)_outputFile).Write(new string[] { String.Format("ERROR") });
            }
        }
    }

    class InputFile : File
    {
        public InputFile(string[] args, string option) : base(args, option)
        {
            // Using parents constructor
        }

        public String Parse()
        {
            if (String.IsNullOrEmpty(base.Name))
            {
                throw new Exception("Missing input file");
            }

            return System.IO.File.ReadAllText(base.Name);
        }
    }

    class OutputFile : File
    {
        public OutputFile(string[] args, string option) : base(args, option)
        {
            // Using parents constructor
        }

        /* 
            Output file should contain the following:
                The name of the winner and their score (colon separated).
                A comma separated list of winners in the case of a tie and the score (colon separated).
                "ERROR", if the input file had any issue.
        */
        public void Write(string[] output)
        {
            System.IO.File.WriteAllLines(base.Name, output);
        }
    }

    class File
    {
        public string Name { get; }
        public File(string[] args, string option)
        {
            Name = GetFile(option, args);
        }

        // Files are passed as command line arguments: --in fileName.txt --out fileName.txt
        private string GetFile(string option, string[] args)
        {
            try
            {
                if (args[0] == option)
                {
                    return args[1];
                }

                if (args[2] == option)
                {
                    return args[3];
                }

                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: {0}.", ex);
                return "";
            }
        }
    }
}