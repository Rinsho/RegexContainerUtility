using System;
using System.Collections;
using System.Collections.Generic;

namespace RegularExpression.Utility
{
    [CLSCompliant(true)]
    public class ContainerResultCollection<T> : IEnumerable<T>
    {
        private IList<T> _regexResults;

        public int Count =>
            _regexResults.Count;

        public ContainerResultCollection(IList<T> results) =>
            _regexResults = results;

        public T this[int index] =>
            _regexResults[index];

        public IEnumerator<T> GetEnumerator() =>
            _regexResults.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _regexResults.GetEnumerator();
    }
}
