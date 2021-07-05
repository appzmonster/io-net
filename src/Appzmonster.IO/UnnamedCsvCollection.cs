using System;
using System.Collections.Generic;

namespace Appzmonster.IO
{
    /// <summary>
    /// Implements <see cref="ICsvCollection"/> to store CSV items for unnamed CSV.
    /// </summary>
    public class UnnamedCsvCollection : ICsvCollection<string>
    {
        private List<List<string>> _list;

        /// <summary>
        /// Initializes an instance of <see cref="UnnamedCsvCollection"/> with the specified expected field count.
        /// </summary>
        /// <param name="expectedFieldCount">The expected field count for every CSV item in this collection.</param>
        public UnnamedCsvCollection(int expectedFieldCount)
        {
            if (expectedFieldCount <= 0)
            {
                throw new ArgumentException("Expected field count must be larger than 0");
            }

            _list = new List<List<string>>();
            ExpectedFieldCount = expectedFieldCount;
        }

        /// <summary>
        /// Returns expected total field count.
        /// </summary>
        public int ExpectedFieldCount { private set; get; }


        /// <summary>
        /// Gets field value by CSV item index and field index.
        /// </summary>
        /// <param name="index">The CSV item index to get.</param>
        /// <param name="fieldIndex">The field index to get.</param>
        /// <returns></returns>
        public string this[int index, int fieldIndex]
        {
            get
            {
                if ((index >= 0) && (index <= (_list.Count - 1)))
                {
                    var item = _list[index];

                    if ((fieldIndex >= 0) && (fieldIndex <= (ExpectedFieldCount - 1)))
                    {
                        return item[fieldIndex];
                    }
                    else
                    {
                        throw new ArgumentException("Field index is out of range");
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException("Index is out of range");
                }
            }
        }

        /// <summary>
        /// Returns total collection count.
        /// </summary>
        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        /// <summary>
        /// Adds single unnamed CSV item. Each unnamed CSV item is a collection of string fields.
        /// </summary>
        /// <param name="fields">The collection of unnamed fields to add.</param>
        public void Add(List<string> fields)
        {
            if (fields.Count != ExpectedFieldCount)
            {
                throw new ArgumentException($"Found {fields.Count} field(s) but expects {ExpectedFieldCount} field(s)");
            }

            _list.Add(fields);

            return;
        }
    }
}
