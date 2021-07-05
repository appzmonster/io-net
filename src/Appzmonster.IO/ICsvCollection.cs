using System;
using System.Collections.Generic;

namespace Appzmonster.IO
{
    public interface ICsvCollection
    {
        string this[int index, int fieldIndex] { get; }
        int Count { get; }
        int ExpectedFieldCount { get; }
    }

    public interface ICsvCollection<T> : ICsvCollection
    {
        void Add(List<T> fields);
    }
}
