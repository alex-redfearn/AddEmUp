using System;
using Winner;

namespace WinnerTest
{
    public class FileTest
    {
        [Fact]
        public void GetFileName_WhenInputFileIsPresent_ShouldReturnInputFileName()
        {
            //Arrange
            string filePath = "TestFiles/Input.txt";
            string inputOption = Winner.File.INPUT_FILE_OPTION;
            string[] arguents = new string[] { inputOption, filePath };

            //Act
            Winner.File file = new Winner.File(arguents, inputOption);

            //Assert
            Assert.Equal(filePath, file.Name);
        }


        [Fact]
        public void GetFileName_WhenOutputFileIsPresent_ShouldReturnOutputFileName()
        {
            //Arrange
            string filePath = "TestFiles/Output.txt";
            string outputOption = Winner.File.INPUT_FILE_OPTION;
            string[] arguents = new string[] { outputOption, filePath };

            //Act
            Winner.File file = new Winner.File(arguents, outputOption);

            //Assert
            Assert.Equal(filePath, file.Name);
        }
    }
}
