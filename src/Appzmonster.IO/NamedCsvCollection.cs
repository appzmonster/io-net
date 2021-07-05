using System;
using System.Collections.Generic;

namespace Appzmonster.IO
{
    /// <summary>
    /// Implements <see cref="INamedCsvCollection"/> to store CSV items for named CSV.
    /// </summary>
    public class NamedCsvCollection : INamedCsvCollection
    {
        private List<Dictionary<string, string>> _list;
        private List<string> _fieldNames;
        
        /// <summary>
        /// Initializes an instance of <see cref="NamedCsvCollection"/> with the specified field names.
        /// </summary>
        /// <param name="fieldNames">The field names to use.</param>
        public NamedCsvCollection(List<string> fieldNames)
        {
            if ((fieldNames == null) || (fieldNames.Count == 0))
            {
                throw new ArgumentException("Field names are missing");
            }

            _list = new List<Dictionary<string, string>>();
            _fieldNames = fieldNames;
        }

        /// <summary>
        /// Returns expected total field count.
        /// </summary>
        public int ExpectedFieldCount
        {
            get
            {
                return _fieldNames.Count;
            }
        }

        /// <summary>
        /// Returns field names.
        /// </summary>
        public List<string> FieldNames
        {
            get
            {
                return _fieldNames;
            }
        }

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

                    if ((fieldIndex >= 0) && (fieldIndex <= (item.Count - 1)))
                    {
                        string[] fields = new string[item.Count];
                        item.Values.CopyTo(fields, 0);
                        return fields[fieldIndex];
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
        /// Gets field value by CSV item index and field name.
        /// </summary>
        /// <param name="index">The CSV item index to get.</param>
        /// <param name="fieldName">The field name to get.</param>
        /// <returns></returns>
        public string this[int index, string fieldName]
        {
            get
            {
                if ((index >= 0) && (index <= (_list.Count - 1)))
                {
                    var item = _list[index];

                    if (item.ContainsKey(fieldName) == true)
                    {
                        return item[fieldName];
                    }
                    else
                    {
                        throw new ArgumentException("Field name not found in collection");
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException("Index is out of range");
                }
            }
        }

        /// <summary>
        /// Returns total CSV items count.
        /// </summary>
        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        /// <summary>
        /// Adds single named CSV item. Each named CSV item is a collection of named fields and each field consists of name and value.
        /// </summary>
        /// <param name="fields">The collection of named fields to add.</param>
        public void Add(List<KeyValuePair<string, string>> fields)
        {
            if (fields.Count != _fieldNames.Count)
            {
                throw new ArgumentException($"Found {fields.Count} field(s) but expects {_fieldNames.Count} field(s)");
            }

            Dictionary<string, string> namedCsvFields = new Dictionary<string, string>();

            for (int i = 0; i <= (fields.Count - 1); i++)
            {
                if (_fieldNames.Contains(fields[i].Key) == true)
                {
                    if (namedCsvFields.ContainsKey(fields[i].Key) == false)
                    {
                        namedCsvFields.Add(fields[i].Key, fields[i].Value);
                    }
                    else
                    {
                        throw new ArgumentException($"Field '{fields[i].Key}' already exist");
                    }
                }
                else
                {
                    throw new ArgumentException($"Cannot find {fields[i].Key} in the field names");
                }
            }

            _list.Add(namedCsvFields);
            return;
        }
    }
}
