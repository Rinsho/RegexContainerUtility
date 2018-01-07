using System.Collections;
using System.Collections.Generic;

namespace RegularExpression.Utility
{
    public class ContainerResultCollection<T> : IEnumerable<ContainerResult<T>>
    {
        private IList<ContainerResult<T>> _regexResults;

        public int Count =>
            _regexResults.Count;

        public ContainerResultCollection(IList<ContainerResult<T>> results) =>
            _regexResults = results;

        public ContainerResult<T> this[int index] =>
            _regexResults[index];

        public IEnumerator<ContainerResult<T>> GetEnumerator() =>
            _regexResults.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _regexResults.GetEnumerator();
    }
}
