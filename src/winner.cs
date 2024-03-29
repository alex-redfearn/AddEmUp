﻿/// <summary>
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
    public class Winner
    {
        public static void Main(string[] args)
        {
            Game game = new Game(args);
            game.Play();
        }
    }

    /// <summary>
    /// Responsible for initiating the game of "Add 'Em Up".
    /// </summary>
    public class Game
    {
        private readonly File _inputFile;
        private readonly File _outputFile;

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
                }

                WriteResult(players);
            }
            catch (Exception ex)
            {
                try
                {
                    Console.WriteLine($"Writing \"ERROR\" to output, exception: {ex.Message}");
                    ((OutputFile)_outputFile).Write("ERROR");
                }
                catch (Exception writeEx)
                {
                    Console.WriteLine($"Cannot write to output file: {writeEx.Message}");
                }
                return;
            }
        }

        private void WriteResult(List<Player> players)
        {
            FaceResult faceResult = new FaceResult(players);
            string result = faceResult.GetResult();

            // If the face values of > 1 players match.
            if (faceResult.Winners.Count > 1)
            {
                // Then the result is based on the suit values of these players.
                SuitResult suitResult = new SuitResult(faceResult.Winners);
                result = suitResult.GetResult();
            }

            Console.WriteLine(result);
            ((OutputFile)_outputFile).Write(result);
        }
    }

    /// <summary>
    /// Implements Result. Determines the result by suit value.
    /// </summary>
    public class SuitResult : Result
    {
        public SuitResult(List<Player> players) : base(players)
        {
            // Using parents constructor.
        }

        protected override List<Player> GetWinners(List<Player> players)
        {
            return players
                .Where(player => player.SuitScore.Equals(_maxSuitScore))
                .Select(player => player)
                .ToList();
        }
    }

    /// <summary>
    /// Implements Result. Determines the result by face value.
    /// </summary>
    public class FaceResult : Result
    {
        public FaceResult(List<Player> players) : base(players)
        {
            // Using parents constructor.
        }

        protected override List<Player> GetWinners(List<Player> players)
        {
            return players
                .Where(player => player.FaceScore.Equals(_maxFaceScore))
                .Select(player => player)
                .ToList();
        }
    }

    /// <summary>
    /// Abstract Result. Returns formatted result.
    /// </summary>
    public abstract class Result
    {
        public List<Player> Winners { get; private set; }
        protected readonly int _maxFaceScore;
        protected readonly int _maxSuitScore;

        public Result(List<Player> players)
        {
            _maxFaceScore = GetMaxFaceScore(players);
            _maxSuitScore = GetMaxSuitScore(players);
            Winners = GetWinners(players);
        }

        protected int GetMaxFaceScore(List<Player> players)
        {
            return players.Select(player => player.FaceScore).Max();
        }

        protected int GetMaxSuitScore(List<Player> players)
        {
            return players.Select(player => player.SuitScore).Max();
        }

        protected abstract List<Player> GetWinners(List<Player> players);

        public string GetResult()
        {
            // The result should be in the format {Name}:{score} or {Name},{Name}:{Score} in a tie.
            string names = String.Join(",", Winners.Select(player => player.Name));

            return $"{names}:{_maxFaceScore}";
        }
    }

    /// <summary>
    /// Represents a single player in the game.
    /// </summary>
    public class Player
    {
        public string Name { get; private set; }
        public List<Card> Hand { get; private set; }
        public int FaceScore
        {
            get { return Hand.Select(card => card.FaceValue).Sum(); }
        }
        public int SuitScore
        {
            get { return Hand.Select(card => card.SuitValue).Sum(); }
        }

        public Player(string name, List<Card> cards)
        {
            Name = name;
            Hand = cards;
        }

        public static bool ArePlayersUnique(List<Player> players)
        {
            // If list.Distinct() has a count < list we can safely assume a duplicate is present.
            return players.Select(player => player.Name).Distinct().Count() == players.Count;
        }

        public bool IsHandUnique()
        {
            return Hand.Select(card => card.Key).Distinct().Count() == Hand.Count;
        }
    }

    /// <summary>
    /// Defines non numeric card face values and their equivalent int values.
    /// </summary>
    public enum CardFace
    {
        A = 1,
        J = 11,
        Q = 12,
        K = 13
    }

    /// <summary>
    /// Defines card suits and their int values.
    /// </summary>
    public enum CardSuit
    {
        S = 4,
        H = 3,
        D = 2,
        C = 1
    }

    /// <summary>
    /// Represents a single card in a players hand.
    /// </summary>
    public class Card
    {
        public string Key { get; private set; }
        public int FaceValue { get; private set; }
        public int SuitValue { get; private set; }

        public Card(string card)
        {
            Key = card;
            FaceValue = GetFaceValue(card);
            SuitValue = GetSuitValue(card);
        }

        // Each card is provided in the format {face}{suit} e.g. AH,3C,10D
        private static int GetFaceValue(string card)
        {
            int value = 0;

            if (IsValidCard(card))
            {
                string face = card.Substring(0, card.Length - 1);
                value = ParseFace(face);
            }

            // Standard card values 1 to 13.
            if(value == 0)
                throw new Exception($"Invalid suit{card}");

            return value;
        }

        private static int ParseFace(string face)
        {
            int value = 0;
            if (Int32.TryParse(face, out value))
            {
                if (value > 0 && value < 11)
                {
                    return value;
                }
            }
            else if (Enum.TryParse<CardFace>(face, out CardFace charFace))
            {
                return (int)charFace;
            }

            return value;
        }

        private static int GetSuitValue(string card)
        {
            int value = 0;
            if (IsValidCard(card))
            {
                string suit = card.Substring(card.Length - 1);
                value = ParseSuit(suit);
            }

            if(value == 0)
                throw new Exception($"Invalid suit{card}");

            return value;
        }

        private static int ParseSuit(string suit)
        {
            int value = 0;
            if (Enum.TryParse<CardSuit>(suit, out CardSuit suitValue))
            {
                value = (int)suitValue;
            }

            return value;
        }

        private static bool IsValidCard(string card)
        {
            // A card must have a lenth of two (2H) or three (10C).
            return card.Length > 1 && card.Length < 4;
        }
    }

    /// <summary>
    /// Parses input file.
    /// </summary>
    public class InputFile : File
    {

        public InputFile(string[] args, string option) : base(args, option)
        {
            // Using parents constructor.
        }

        public List<Player> Parse()
        {
            // Returns text from implicit file
            string text = GetText();

            // Parse text into rows
            string[] rows = ParseRows(text);

            // Parse players from rows
            return ParsePlayers(rows);
        }

        private string GetText()
        {
            if (!IsTextFile())
            {
                throw new Exception("Missing or invalid input file");
            }

            return System.IO.File.ReadAllText(base.Name);
        }

        private static string[] ParseRows(string text)
        {
            // Each player is seperated within the file by a new line.
            return text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }

        private static List<Player> ParsePlayers(string[] rows)
        {
            List<Player> players = new List<Player>();
            foreach (string row in rows)
            {
                Player player = new Player(ParseName(row), ParseHand(row));
                players.Add(player);
            }

            int count = players.Count;
            if (count != 5)
            {
                throw new Exception($"Invalid player count, player count {count}");
            }

            return players;
        }

        // Player names are provided in the format {Name}:{cards}
        private static string ParseName(string row)
        {
            return row.Split(":")[0];
        }

        // Players hands are provided in the format AH, 3C, 8C, 10S, JD
        private static List<Card> ParseHand(string row)
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
    public class OutputFile : File
    {
        public OutputFile(string[] args, string option) : base(args, option)
        {
            // Using parents constructor.
        }

        /* 
            Output file should contain the following:
            The name of the winner and their score (colon separated).
            A comma separated list of winners in the case of a tie and the score (colon separated).
            "ERROR", if the input file had any issue.
        */
        public void Write(string output)
        {
            System.IO.File.WriteAllLines(base.Name, new string[] { output });
        }
    }

    /// <summary>
    /// Gets the path of files passed as command line arguments.
    /// </summary>
    public class File
    {
        public static readonly string INPUT_FILE_OPTION = "--in";
        public static readonly string OUTPUT_FILE_OPTION = "--out";

        public string Name { get; private set; }

        public File(string[] args, string option)
        {
            Name = GetFile(option, args);
        }

        // Files are passed as command line arguments: --in fileName.txt --out fileName.txt
        private static string GetFile(string option, string[] args)
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

        protected bool IsTextFile()
        {
            string[] splitFile = Name.Split(".");
            return splitFile.Length != 0 && splitFile[splitFile.Length - 1] == "txt";
        }
    }
}