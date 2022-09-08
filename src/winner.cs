using System;
using System.Collections.Generic;

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
        private File _inputFile;
        private File _outputFile;
        public Game(string[] args)
        {
            _inputFile = new InputFile(args, File.INPUT_FILE_OPTION);
            _outputFile = new OutputFile(args, File.OUTPUT_FILE_OPTION);
        }

        public void Play()
        {
            // If output file is missing end game.
            if (String.IsNullOrEmpty(_outputFile.Name))
                return;

            try
            {
                List<Player> players = ((InputFile)_inputFile).Parse();
                foreach (Player player in players)
                {
                    Console.WriteLine(player.Name);
                    Console.WriteLine(player.Cards[0].Key);
                    Console.WriteLine(player.Cards[0].FaceValue);
                    Console.WriteLine(player.Cards[0].SuitValue);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Console.WriteLine($"Writing \"ERROR\" to output, exception: {ex.Message}");
                    ((OutputFile)_outputFile).Write(new string[] { "ERROR" });

                }
                catch (Exception writeEx)
                {
                    Console.WriteLine($"Cannot write to output file: {writeEx.Message}");
                }
                return;
            }
        }
    }

    class InputFile : File
    {
        public InputFile(string[] args, string option) : base(args, option)
        {
            // Using parents constructor
        }

        public List<Player> Parse()
        {
            if (String.IsNullOrEmpty(base.Name) || !IsTextFile())
            {
                throw new Exception("Missing or invalid input file");
            }

            string text = System.IO.File.ReadAllText(base.Name);

            return ParsePlayers(SplitInput(text));
        }

        private bool IsTextFile()
        {
            string[] splitFile = base.Name.Split(".");
            return splitFile.Length != 0 && splitFile[splitFile.Length - 1] == "txt";
        }

        // Each player is seperated within the file by a new line.
        private string[] SplitInput(string input)
        {
            return input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }

        private List<Player> ParsePlayers(string[] rows)
        {
            List<Player> players = new List<Player>();
            foreach (string row in rows)
            {
                Player player = new Player(ParseName(row), ParseHand(row));
                players.Add(player);
            }

            int count = players.Count;
            // AddEmUp requires five players.
            if (count ==  5)
            {
                throw new Exception($"Invalid player count, player count {count}");
            }

            return players;
        }

        // Player names are provided in the format {Name}:{cards}
        private string ParseName(string row)
        {
            return row.Split(":")[0];
        }

        // Players hands are provided in the format AH, 3C, 8C , 10S, JD
        private List<Card> ParseHand(string row)
        {
            string joinedHand = row.Split(":")[1];
            string[] splitHand = joinedHand.Split(",");

            List<Card> cards = new List<Card>();
            foreach (string hand in splitHand)
            {
                Card card = new Card(hand);
                cards.Add(card);
            }

            int count = cards.Count;
            // Each player MUST have a hand of five cards.
            if (count == 5)
            {
                throw new Exception(String.Format($"Invalid card amount, card count {count}"));
            }

            return cards;
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
        public static readonly string INPUT_FILE_OPTION = "--in";
        public static readonly string OUTPUT_FILE_OPTION = "--out";

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
                Console.WriteLine($"Exception caught: {ex}.");
                return "";
            }
        }
    }

    class Player
    {
        public string Name { get; }
        public List<Card> Cards { get; }

        public Player(string name, List<Card> card)
        {
            Name = name;
            Cards = card;
        }
    }

    enum CardFace
    {
        A = 1,
        J = 11,
        Q = 12,
        K = 13
    }

    enum CardSuit
    {
        S = 4,
        H = 3,
        D = 2,
        C = 1
    }

    class Card
    {
        public string Key { get; }
        public int FaceValue { get; }
        public int SuitValue { get; }

        public Card(string card)
        {
            Key = card;
            FaceValue = ParseFace(card);
            SuitValue = ParseSuit(card);
        }

        // Each card is provided in the format {face}{suit} e.g. AH, 3C, 10D
        private int ParseFace(string card)
        {
            int value = 0;
            if (IsValidCard(card))
            {
                string face = card.Substring(0, card.Length - 1);

                // A cards face can either be an int value (2 - 10) or A,J,Q,K
                int faceIntValue;
                if (Int32.TryParse(face, out faceIntValue))
                {
                    value = faceIntValue;
                }

                CardFace faceCharValue;
                if (Enum.TryParse<CardFace>(face, out faceCharValue))
                {
                    value = (int)faceCharValue;
                }
            }

            // Standard card values 1 to 13
            if (value < 1 || value > 13)
            {
                throw new Exception($"Invalid face, {card}");
            }

            return value;
        }

        private int ParseSuit(string card)
        {
            int value = 0;
            if (IsValidCard(card))
            {
                string suit = card.Substring(card.Length - 1);

                CardSuit suitValue;
                if (Enum.TryParse<CardSuit>(suit, out suitValue))
                {
                    return (int)suitValue;
                }
            }

            if (value < 1 || value > 4)
            {
                throw new Exception($"Invalid suit, {card}");
            }

            return value;
        }

        private bool IsValidCard(string card)
        {
            // A card must have a lenth of two (2H) or three (10C)
            return card.Length > 1 && card.Length < 4;
        }
    }
}