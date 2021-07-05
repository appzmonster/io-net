using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Appzmonster.IO
{
    /// <summary>
    /// A comma separated value (CSV) reader that reads from a stream in a particular encoding.
    /// </summary>
    public class CsvStreamReader : MarshalByRefObject, IDisposable
    {
        private StreamReader _streamReader;
        private List<string> _fieldNames;
        private bool _useFirstLineForFieldNames;

        /// <summary>
        /// Initializes a new <see cref="CsvStreamReader"/> instance for the specified stream, 
        /// with the specified character encoding. This instance reads line 1 as field names
        /// from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public CsvStreamReader(Stream stream, Encoding encoding = null)
        {
            _streamReader = new StreamReader(stream, encoding);
            _useFirstLineForFieldNames = true;
        }

        /// <summary>
        /// Initializes a new <see cref="CsvStreamReader"/> instance for the specified stream, 
        /// with the specified field names and character encoding. If <see cref="fieldNames"/>
        /// argument is empty, this instance does not use field name for the CSV (unnamed CSV).
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="fieldNames">The field names to use, (set as empty if using unnamed csv).</param>
        /// <param name="encoding">The character encoding to use.</param>
        public CsvStreamReader(Stream stream, List<string> fieldNames, Encoding encoding = null)
        {
            ValidateFieldNames(fieldNames);

            _streamReader = new StreamReader(stream, encoding);
            _fieldNames = fieldNames;
            _useFirstLineForFieldNames = false;
        }

        /// <summary>
        /// Initializes a new <see cref="CsvStreamReader"/> instance for the specified file name, 
        /// with the specified character encoding. This instance reads line 1 as field names
        /// from the specified stream.
        /// </summary>
        /// <param name="path">The file path to read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public CsvStreamReader(string path, Encoding encoding = null)
            : this(new FileStream(path, FileMode.Open), encoding) { }

        /// <summary>
        /// Initializes a new <see cref="CsvStreamReader"/> instance for the specified file name, 
        /// with the specified field names and character encoding. If <see cref="fieldNames"/>
        /// argument is empty, this instance does not use field name for the CSV (unnamed CSV).
        /// </summary>
        /// <param name="path">The file path to read</param>
        /// <param name="fieldNames">The field names to use, (set as empty if using unnamed csv).</param>
        /// <param name="encoding">The character encoding to use.</param>
        public CsvStreamReader(string path, List<string> fieldNames, Encoding encoding = null)
            : this(new FileStream(path, FileMode.Open), fieldNames, encoding) { }

        /// <summary>
        /// Returns the underlying stream.
        /// </summary>
        public Stream BaseStream
        {
            get
            {
                return _streamReader.BaseStream;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="CsvStreamReader"/> object.
        /// </summary>
        public void Dispose()
        {
            if (_streamReader != null)
            {
                _streamReader.Dispose();
            }
        }

        /// <summary>
        /// Reads csv data from line 1 until end of stream.
        /// </summary>
        /// <returns>
        /// Collection of CSV items. For named CSV, an instance of <see cref="NamedCsvCollection"/>
        /// is returned. For unnamed CSV, an instance of <see cref="UnnamedCsvCollection"/> is returned.
        /// </returns>
        public ICsvCollection ReadToEnd()
        {
            // Resets position to 0 everytime before reading.
            _streamReader.BaseStream.Position = 0;
            _streamReader.DiscardBufferedData();

            ICsvCollection collection = null;
            NamedCsvCollection namedCsvCollection = null;
            UnnamedCsvCollection unnamedCsvCollection = null;

            int currentLineNumber = 0;
            do
            {
                string line = _streamReader.ReadLine();
                currentLineNumber++;

                string[] lineParts = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                if ((lineParts != null) && (lineParts.Length > 0))
                {
                    if ((_useFirstLineForFieldNames == true) && (currentLineNumber == 1))
                    {
                        // This is for named CSV stream with line 1 field names.
                        _fieldNames = new List<string>();
                        for (int i = 0; i <= (lineParts.Length - 1); i++)
                        {
                            _fieldNames.Add(HandleDoubleQuoteIfAny(lineParts[i]));
                        }

                        ValidateFieldNames(_fieldNames);
                    }
                    else
                    {
                        if ((_fieldNames != null) && (_fieldNames.Count > 0))
                        {
                            // This is for named CSV stream with external provided field names.
                            if (collection == null)
                            {
                                namedCsvCollection = new NamedCsvCollection(_fieldNames);
                                collection = namedCsvCollection;
                            }

                            if (lineParts.Length != namedCsvCollection.ExpectedFieldCount)
                            {
                                throw new Exception($"Line number {currentLineNumber} has ({lineParts.Length} field(s) but expects {namedCsvCollection.ExpectedFieldCount} field(s)");
                            }

                            List<KeyValuePair<string, string>> lineData = new List<KeyValuePair<string, string>>();
                            for (int i = 0; i <= (namedCsvCollection.ExpectedFieldCount - 1); i++)
                            {
                                lineData.Add(new KeyValuePair<string, string>(_fieldNames[i], HandleDoubleQuoteIfAny(lineParts[i])));
                            }

                            namedCsvCollection.Add(lineData);
                        }
                        else
                        {
                            // This is for unnamed csv stream. The total number of fields for entire CSV data structure
                            // is determined at line 1.
                            if (collection == null)
                            {
                                unnamedCsvCollection = new UnnamedCsvCollection(lineParts.Length);
                                collection = unnamedCsvCollection;
                            }

                            if (lineParts.Length != unnamedCsvCollection.ExpectedFieldCount)
                            {
                                throw new Exception($"Line number {currentLineNumber} has ({lineParts.Length} field(s) but expects {unnamedCsvCollection.ExpectedFieldCount} field(s)");
                            }

                            List<string> lineData = new List<string>();
                            for (int i = 0; i <= (unnamedCsvCollection.ExpectedFieldCount - 1); i++)
                            {
                                lineData.Add(HandleDoubleQuoteIfAny(lineParts[i]));
                            }

                            unnamedCsvCollection.Add(lineData);
                        }
                    }
                }
            }
            while (_streamReader.EndOfStream == false);

            return collection;
        }

        /// <summary>
        /// Handles double quoted CSV field value.
        /// </summary>
        /// <param name="data">The field value to process.</param>
        /// <returns>Field value without double quotes enclosure.</returns>
        private string HandleDoubleQuoteIfAny(string data)
        {
            data = data.Trim();
            if (data.StartsWith('"'))
            {
                var reg = new Regex("^\".*?\"$");
                var match = reg.Match(data);
                if (match.Success)
                {
                    return match.Value.TrimStart('"').TrimEnd('"');
                }
                else
                {
                    return data;
                }
            }
            else
            {
                return data;
            }
        }

        private void ValidateFieldNames(List<string> fieldNames)
        {
            if ((fieldNames != null) && (fieldNames.Count > 0))
            {
                for (int i = 0; i <= (fieldNames.Count - 1); i++)
                {
                    fieldNames[i] = fieldNames[i].Trim();
                    if (string.IsNullOrEmpty(fieldNames[i]))
                    {
                        throw new Exception($"Field name at index {i} is null or empty after trim");
                    }
                }
            }

            return;
        }
    }
}


