using System;
using Winner;

namespace WinnerTest
{
    public class PlayerTest
    {
        [Fact]
        public void GetFaceScore_WhenPlayersHandHasBeenScored_ShouldReturnSumOfFaceValues()
        {
            //Arrange
            string playerName = "Alex";
            List<Card> hand = new List<Card>();
            hand.Add(new Card("AH"));
            hand.Add(new Card("3C"));
            hand.Add(new Card("8C"));
            hand.Add(new Card("2S"));
            hand.Add(new Card("JD"));
            int faceScore = 25;

            //Act
            Player player = new Player(playerName, hand);
            player.SetScore();

            //Assert
            Assert.Equal(faceScore, player.FaceScore);
        }

        [Fact]
        public void GetSuitScore_WhenPlayersHandHasBeenScored_ShouldReturnSumOfSuitValues()
        {
            //Arrange
            string playerName = "Alex";
            List<Card> hand = new List<Card>();
            hand.Add(new Card("AH"));
            hand.Add(new Card("3C"));
            hand.Add(new Card("8C"));
            hand.Add(new Card("2S"));
            hand.Add(new Card("JD"));
            int suitScore = 11;

            //Act
            Player player = new Player(playerName, hand);
            player.SetScore();

            //Assert
            Assert.Equal(suitScore, player.SuitScore);
        }
    }
}

