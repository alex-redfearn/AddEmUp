using System;
using Winner;

namespace WinnerTest
{
    public class CardTest
    {
        [Fact]
        public void GetFaceValue_WhenValidCardAceOfHeartsIsSupplied_ShouldReturnOne()
        {
            //Arrange
            string validCard = "AH";
            int faceValue = 1;
            //Act
            Card card = new Card(validCard);

            //Assert
            Assert.Equal(faceValue, card.FaceValue);
            Assert.Equal(validCard, card.Key);
        }

        [Fact]
        public void GetSuitValue_WhenAceOfHeartsIsSupplied_ShouldReturnThree()
        {
            //Arrange
            string validCard = "AH";
            int suitValue = 3;
            //Act
            Card card = new Card(validCard);

            //Assert
            Assert.Equal(suitValue, card.SuitValue);
            Assert.Equal(validCard, card.Key);
        }
    }
}

