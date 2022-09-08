/// <summary>
///  This program executes the game "Add 'Em Up".
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Winner
{
    /// <summary>
    /// Main method.
    /// </summary>
    class Winner
    {
        static void Main(string[] args)
        {
            Game game = new Game(args);
            game.Play();
        }
    }

    /// <summary>
    /// Responsible for initiating the game of "Add 'Em Up".
    /// </summary>
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

                if (!Player.ArePlayersUnique(players))
                    throw new Exception("Duplicate players");

                foreach (Player player in players)
                {
                    if (!player.IsHandUnique())
                    {
                        throw new Exception("Hand contains duplicate cards");
                    }

                    player.SetScore();
                }

                Console.WriteLine(Result.FormatResult(FaceResult.GetWinners(players)));
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

    /// <summary>
    /// Extends Result. Determines the result of the face values in a hand.
    /// </summary>
    class FaceResult : Result
    {
        public static List<Player> GetWinners(List<Player> players)
        {
            int maxScore = GetMaxScore(players.Select(player => player.FaceScore));

            return players
                .Where(player => player.FaceScore.Equals(maxScore))
                .Select(player => player)
                .ToList();
        }
    }

    /// <summary>
    /// Formats the result string.
    /// </summary>
    class Result
    {
        public static int GetMaxScore(IEnumerable<int> scores)
        {
            return scores.Max();
        }

        public static string FormatResult(List<Player> winners)
        {
            int score = winners[0].FaceScore;
            string names = String.Join(",", winners.Select(player => player.Name));

            return $"{names}:{score}";
        }
    }

    /// <summary>
    /// Extends File. Parses input file.
    /// </summary>
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
            if (count != 5)
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
            if (count != 5)
            {
                throw new Exception(String.Format($"Invalid card amount, card count {count}"));
            }

            return cards;
        }
    }

    /// <summary>
    /// Extends File. Writes to ouptut file.
    /// </summary>
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

    /// <summary>
    /// Gets the path of files passed as command line arguments.
    /// </summary>
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
    /// <summary>
    /// Represents a single player in the game.
    /// </summary>
    class Player
    {
        public string Name { get; }
        public List<Card> Hand { get; }
        public int FaceScore { get; private set; }
        public int SuitScore { get; private set; }

        public Player(string name, List<Card> cards)
        {
            Name = name;
            Hand = cards;
        }

        // If list.Distinct() has a count < list we can safely assume a duplicate is present.
        public static bool ArePlayersUnique(List<Player> players)
        {
            return players.Select(player => player.Name).Distinct().Count() == players.Count();
        }

        public bool IsHandUnique()
        {
            return Hand.Select(card => card.Key).Distinct().Count() == Hand.Count();
        }

        public void SetScore()
        {
            FaceScore = Hand.Select(card => card.FaceValue).Sum();
            SuitScore = Hand.Select(card => card.SuitValue).Sum();
        }
    }

    /// <summary>
    /// Defines non numeric card face values and their equivalent int values.
    /// </summary>
    enum CardFace
    {
        A = 1,
        J = 11,
        Q = 12,
        K = 13
    }

    /// <summary>
    /// Defines card suits and their int values.
    /// </summary>
    enum CardSuit
    {
        S = 4,
        H = 3,
        D = 2,
        C = 1
    }

    /// <summary>
    /// Represents a single card in a players hand.
    /// </summary>
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