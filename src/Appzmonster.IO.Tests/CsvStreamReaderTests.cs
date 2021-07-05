using System;
using System.IO;
using Xunit;
using Appzmonster.IO;
using System.Collections.Generic;
using System.Linq;

namespace Appzmonster.IO.CsvTests
{
    public class CsvStreamReaderTests
    {
        [Theory]
        [InlineData(1, "First Name", "るろうに", true)]
        [InlineData(0, "First Name", "Jimmy", true)]
        [InlineData(0, "Mailing Address", "Some street, XX, yy...", true)]
        [InlineData(2, "First Name", "Leong", false)]
        public void NamedCsv_GetByFieldName_IsEqual(int index, string fieldName, string testValue, bool expectedResult)
        {
            string path = "./assets/csv/named-sample.csv";
            NamedCsvCollection csvCollection = null;

            using (var reader = new CsvStreamReader(path))
            {
                csvCollection = reader.ReadToEnd() as NamedCsvCollection;
            }

            string value = csvCollection[index, fieldName];
            bool isEqual = value.Equals(testValue, StringComparison.InvariantCulture);
            Assert.True(isEqual == expectedResult);
        }

        [Theory]
        [InlineData(1, 1, "るろうに", true)]
        [InlineData(0, 1, "Jimmy", true)]
        [InlineData(0, 3, "Some street, XX, yy...", true)]
        [InlineData(2, 1, "Leong", false)]
        [InlineData(2, 2, "Leong", true)]
        public void NamedCsv_GetByFieldIndex_IsEqual(int index, int fieldIndex, string testValue, bool expectedResult)
        {
            string path = "./assets/csv/named-sample.csv";
            NamedCsvCollection csvCollection = null;

            using (var reader = new CsvStreamReader(path))
            {
                csvCollection = reader.ReadToEnd() as NamedCsvCollection;
            }

            string value = csvCollection[index, fieldIndex];
            bool isEqual = value.Equals(testValue, StringComparison.InvariantCulture);
            Assert.True(isEqual == expectedResult);
        }

        [Theory]
        [InlineData(new[] { "Id", "Abc" }, false)]
        [InlineData(new[] { "Id", "First Name", "Last Name", "Mailing Address" }, true)]
        [InlineData(new[] { "Id", "fiRSt Name", "LAST Name", "Mailing Address" }, false)]
        public void NamedCsv_FieldNames_IsEqual(IEnumerable<string> testValue, bool expectedResult)
        {
            string path = "./assets/csv/named-sample.csv";
            NamedCsvCollection csvCollection = null;
            List<string> testFieldNames = new List<string>(testValue);
            
            using (var reader = new CsvStreamReader(path))
            {
                csvCollection = reader.ReadToEnd() as NamedCsvCollection;
            }

            bool isFieldCountEqual = csvCollection.ExpectedFieldCount == testFieldNames.Count;

            bool isFieldNamesEqual = isFieldCountEqual;
            if (isFieldCountEqual)
            {
                for (int i = 0; i <= (csvCollection.ExpectedFieldCount - 1); i++)
                {
                    if (csvCollection.FieldNames[i].Equals(testFieldNames[i], StringComparison.InvariantCulture) == false)
                    {
                        isFieldNamesEqual = false;
                    }
                }
            }

            Assert.True((isFieldCountEqual && isFieldNamesEqual) == expectedResult);
        }

        [Theory]
        [InlineData(new[] { "Id", "First Name", "Last Name", "Mailing Address" }, 1, "First Name", "るろうに", true)]
        [InlineData(new[] { "Id", "First Name", "Last Name", "Mailing Address" }, 1, "Last Name", "るろうに", false)]
        [InlineData(new[] { "Id", "First Name", "Last Name", "Mailing Address" }, 1, "Last Name", "剣心", true)]
        public void NamedCsvWithExternalFieldNames_GetByFieldName_IsEqual(IEnumerable<string> fieldNames, int index, string fieldName, string testValue, bool expectedResult)
        {
            // This is for csv file that does not have field names (usually first line)
            // but want to use field names and the source of the csv file is unable to
            // include field names in the csv file...let's assume no budget to do that :(
            string path = "./assets/csv/unnamed-sample.csv";
            NamedCsvCollection csvCollection = null;
            List<string> csvFieldNames = new List<string>(fieldNames);

            using (var reader = new CsvStreamReader(path, fieldNames: csvFieldNames))
            {
                csvCollection = reader.ReadToEnd() as NamedCsvCollection;
            }

            string value = csvCollection[index, fieldName];
            bool isEqual = value.Equals(testValue, StringComparison.InvariantCulture);
            Assert.True(isEqual == expectedResult);
        }

        [Theory]
        [InlineData(1, 1, "るろうに", true)]
        [InlineData(0, 1, "Jimmy", true)]
        [InlineData(0, 3, "Some street, XX, yy...", true)]
        [InlineData(2, 1, "Leong", false)]
        [InlineData(2, 2, "Leong", true)]
        public void UnnamedCsv_GetByFieldIndex_IsEqual(int index, int fieldIndex, string testValue, bool expectedResult)
        {
            string path = "./assets/csv/unnamed-sample.csv";
            UnnamedCsvCollection csvCollection = null;

            using (var reader = new CsvStreamReader(path, fieldNames: null))
            {
                csvCollection = reader.ReadToEnd() as UnnamedCsvCollection;
            }

            string value = csvCollection[index, fieldIndex];
            bool isEqual = value.Equals(testValue, StringComparison.InvariantCulture);
            Assert.True(isEqual == expectedResult);
        }

        [Theory]
        [InlineData(1, "First Name", "るろうに", true)]
        public void NamedCsv_InvokeReadToEndMultipleTimes(int index, string fieldName, string testValue, bool expectedResult)
        {
            string path = "./assets/csv/named-sample.csv";
            ICsvCollection csvCollection = null;

            using (var reader = new CsvStreamReader(path))
            {
                csvCollection = reader.ReadToEnd();
                csvCollection = reader.ReadToEnd();
            }

            NamedCsvCollection namedCsvCollection = csvCollection as NamedCsvCollection;
            string value = namedCsvCollection[index, fieldName];
            bool isEqual = value.Equals(testValue, StringComparison.InvariantCulture);
            Assert.True(isEqual == expectedResult);
        }
    }
}
