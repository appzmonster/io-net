using System;
using System.Collections.Generic;

namespace Appzmonster.IO
{
    public interface INamedCsvCollection : ICsvCollection<KeyValuePair<string, string>>
    {
        string this[int index, string fieldName] { get; }
    }
}
