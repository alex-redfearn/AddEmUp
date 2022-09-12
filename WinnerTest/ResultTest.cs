using System;
using Winner;

namespace WinnerTest
{
    /// <summary>
    /// Class for testing the Result object.
    /// <para>
    /// Card Suit Key: C (Clubs) = 1, D (Diamonds) = 2, H (Hearts) = 3, S (Spades) = 4.
    /// Card Face Key: A (Ace) = 1, J (Jack) = 11, Q (Queen) = 12, K (King) = 13.
    /// </para>
    /// </summary>
    public class ResultTest
    {
        [Fact]
        public void GetWinnersByFaceValue_WhenPlayerOneHasTheHighestFaceValueScore_ShouldReturnPlayerOne()
        {
            //Arrange
            string playerOneName = "Alex";
            List<Card> playerOneHand = new List<Card>();
            playerOneHand.Add(new Card("AH"));
            playerOneHand.Add(new Card("10C"));
            playerOneHand.Add(new Card("QC"));
            playerOneHand.Add(new Card("KS"));
            playerOneHand.Add(new Card("JD"));

            Player playerOne = new Player(playerOneName, playerOneHand);

            string playerTwoName = "Bob";
            List<Card> playerTwoHand = new List<Card>();
            playerTwoHand.Add(new Card("AH"));
            playerTwoHand.Add(new Card("2C"));
            playerTwoHand.Add(new Card("AC"));
            playerTwoHand.Add(new Card("3S"));
            playerTwoHand.Add(new Card("JD"));

            Player playerTwo = new Player(playerTwoName, playerTwoHand);

            List<Player> players = new List<Player>();
            players.Add(playerOne);
            players.Add(playerTwo);

            // Act
            FaceResult faceResult = new FaceResult(players);
            List<Player> winners = faceResult.Winners;

            // Assert
            Assert.Equal(1, winners.Count);
            Assert.Equal(playerOne.Name, faceResult.Winners[0].Name);
        }

        [Fact]
        public void GetWinnersBySuitValue_WhenPlayerOneHasTheHighestSuitValueScore_ShouldReturnPlayerOne()
        {
            //Arrange
            string playerOneName = "Alex";
            List<Card> playerOneHand = new List<Card>();
            playerOneHand.Add(new Card("AH"));
            playerOneHand.Add(new Card("10S"));
            playerOneHand.Add(new Card("QC"));
            playerOneHand.Add(new Card("KS"));
            playerOneHand.Add(new Card("JD"));

            Player playerOne = new Player(playerOneName, playerOneHand);

            string playerTwoName = "Bob";
            List<Card> playerTwoHand = new List<Card>();
            playerTwoHand.Add(new Card("AH"));
            playerTwoHand.Add(new Card("2C"));
            playerTwoHand.Add(new Card("AC"));
            playerTwoHand.Add(new Card("3S"));
            playerTwoHand.Add(new Card("JD"));

            Player playerTwo = new Player(playerTwoName, playerTwoHand);

            List<Player> players = new List<Player>();
            players.Add(playerOne);
            players.Add(playerTwo);

            // Act
            SuitResult suitResult = new SuitResult(players);
            List<Player> winners = suitResult.Winners;

            // Assert
            Assert.Equal(1, winners.Count);
            Assert.Equal(playerOne.Name, suitResult.Winners[0].Name);
        }

        [Fact]
        public void GetWinnersByFaceValue_WhenPlayerOneAndPlayerTwoHaveEqualScore_ShouldReturnBothPlayers()
        {
            //Arrange
            string playerOneName = "Alex";
            List<Card> playerOneHand = new List<Card>();
            playerOneHand.Add(new Card("AH"));
            playerOneHand.Add(new Card("10C"));
            playerOneHand.Add(new Card("QC"));
            playerOneHand.Add(new Card("KS"));
            playerOneHand.Add(new Card("JD"));

            Player playerOne = new Player(playerOneName, playerOneHand);

            string playerTwoName = "Bob";
            List<Card> playerTwoHand = new List<Card>();
            playerTwoHand.Add(new Card("AH"));
            playerTwoHand.Add(new Card("10C"));
            playerTwoHand.Add(new Card("QC"));
            playerTwoHand.Add(new Card("KS"));
            playerTwoHand.Add(new Card("JD"));

            Player playerTwo = new Player(playerTwoName, playerTwoHand);

            List<Player> players = new List<Player>();
            players.Add(playerOne);
            players.Add(playerTwo);

            // Act
            FaceResult faceResult = new FaceResult(players);
            List<Player> winners = faceResult.Winners;

            // Assert
            Assert.Equal(players.Count, faceResult.Winners.Count);
        }

        [Fact]
        public void GetResult_WhenPlayerOneHasTheHighestFaceValueScore_ShouldReturnPlayerOneNameAndScore()
        {
            //Arrange
            string playerOneName = "Alex";
            List<Card> playerOneHand = new List<Card>();
            playerOneHand.Add(new Card("AH"));
            playerOneHand.Add(new Card("10C"));
            playerOneHand.Add(new Card("QC"));
            playerOneHand.Add(new Card("KS"));
            playerOneHand.Add(new Card("JD"));

            Player playerOne = new Player(playerOneName, playerOneHand);

            string playerTwoName = "Bob";
            List<Card> playerTwoHand = new List<Card>();
            playerTwoHand.Add(new Card("AH"));
            playerTwoHand.Add(new Card("2C"));
            playerTwoHand.Add(new Card("AC"));
            playerTwoHand.Add(new Card("3S"));
            playerTwoHand.Add(new Card("JD"));

            Player playerTwo = new Player(playerTwoName, playerTwoHand);

            List<Player> players = new List<Player>();
            players.Add(playerOne);
            players.Add(playerTwo);

            // Act
            FaceResult faceResult = new FaceResult(players);

            // Assert
            Assert.Equal($"{playerOne.Name}:{playerOne.FaceScore}", faceResult.GetResult());
        }

        [Fact]
        public void GetResult_WhenPlayerOneAndTwoHaveEqualScores_ShouldReturnPlayerOneAndTwoNameAndScore()
        {
            //Arrange
            string playerOneName = "Alex";
            List<Card> playerOneHand = new List<Card>();
            playerOneHand.Add(new Card("AH"));
            playerOneHand.Add(new Card("10C"));
            playerOneHand.Add(new Card("QC"));
            playerOneHand.Add(new Card("KS"));
            playerOneHand.Add(new Card("JD"));

            Player playerOne = new Player(playerOneName, playerOneHand);

            string playerTwoName = "Bob";
            List<Card> playerTwoHand = new List<Card>();
            playerTwoHand.Add(new Card("AH"));
            playerTwoHand.Add(new Card("10C"));
            playerTwoHand.Add(new Card("QC"));
            playerTwoHand.Add(new Card("KS"));
            playerTwoHand.Add(new Card("JD"));

            Player playerTwo = new Player(playerTwoName, playerTwoHand);

            List<Player> players = new List<Player>();
            players.Add(playerOne);
            players.Add(playerTwo);

            // Act
            FaceResult faceResult = new FaceResult(players);
            List<Player> winners = faceResult.Winners;

            // Assert
            Assert.Equal($"{playerOne.Name},{playerTwo.Name}:{playerOne.FaceScore}", faceResult.GetResult());
        }
    }

}