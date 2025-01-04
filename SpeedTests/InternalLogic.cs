using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpeedTests
{
    [TestClass]
    public class InternalLogic
    {
        [TestMethod]
        public void FindGapsTestWithVariousGapsReturnsCorrectResults()
        {
            // Arrange
            var input = new List<int> { 1, 0, 0, 2, 0, 0, 0, 3, 4, 0 };
            var expected = new List<string>
            {
                "Gap of size 2 from index 1 to 2",
                "Gap of size 3 from index 4 to 6",
                "Gap of size 1 at index 9"
            };

            // Act
            var actual = FindGaps(input);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindGapsTestNoGapsReturnsEmptyResults()
        {
            // Arrange
            var input = new List<int> { 1, 2, 3, 4, 5 };
            var expected = new List<string>();

            // Act
            var actual = FindGaps(input);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindGapsTestSingleGapOfSizeOneReturnsCorrectResult()
        {
            // Arrange
            var input = new List<int> { 1, 0, 2 };
            var expected = new List<string>
            {
                "Gap of size 1 at index 1"
            };

            // Act
            var actual = FindGaps(input);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindGapsTestSingleGapOfSizeGreaterThanOneReturnsCorrectResult()
        {
            // Arrange
            var input = new List<int> { 1, 0, 0, 0, 2 };
            var expected = new List<string>
            {
                "Gap of size 3 from index 1 to 3"
            };

            // Act
            var actual = FindGaps(input);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        public static List<string> FindGaps(List<int> ids)
        {
            var results = new List<string>();
            int? gapStart = null;

            for (var i = 0; i < ids.Count; i++)
            {
                if (ids[i] == 0) // If there's a gap
                {
                    if (gapStart == null)
                    {
                        gapStart = i; // Start of the gap
                    }
                }
                else // If the current value is not a gap
                {
                    if (gapStart != null)
                    {
                        var gapSize = i - gapStart.Value;

                        if (gapSize == 1)
                        {
                            results.Add($"Gap of size 1 at index {gapStart.Value}");
                        }
                        else if (gapSize > 1)
                        {
                            results.Add($"Gap of size {gapSize} from index {gapStart.Value} to {i - 1}");
                        }

                        gapStart = null; // Reset the gap tracking
                    }
                }
            }

            // If the list ends with a gap
            if (gapStart == null) return results;
            {
                var gapSize = ids.Count - gapStart.Value;
                switch (gapSize)
                {
                    case 1:
                        results.Add($"Gap of size 1 at index {gapStart.Value}");
                        break;
                    case > 1:
                        results.Add($"Gap of size {gapSize} from index {gapStart.Value} to {ids.Count - 1}");
                        break;
                }
            }

            return results;
        }
    }
}
