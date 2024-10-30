using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Data.Common.Contracts
{
    public abstract class PagedDataSource<T> : IOneWayDataSource<T>
    {
        private static readonly T[] EMPTY = Array.Empty<T>();

        private readonly IDictionary<int, IEnumerable<T>> previousResults;
        public IEnumerable<T> Entirety => previousResults.Values.SelectMany(cached => cached);

        public int PageSize { get; }

        public int CurrentPage { get; private set; } = 0;

        public bool EndReached { get; private set; } = false;

        protected PagedDataSource(int pageSize)
        {
            PageSize = pageSize;

            previousResults = new Dictionary<int, IEnumerable<T>>();
        }

        public IEnumerable<T> Next()
        {
            int nextPage = CurrentPage + 1;

            if (previousResults.TryGetValue(nextPage, out IEnumerable<T>? cached)) return cached;
            else
            {
                T[] currentResult = EMPTY;

                if (!EndReached)
                {
                    IIndexRange range = CalculatePageDimensions(nextPage);

                    currentResult = _Get(range).ToArray();

                    int length = currentResult.Length;

                    if (!(EndReached = length == 0 || length < PageSize))
                    {
                        previousResults.Add(nextPage, currentResult);

                        CurrentPage = nextPage;
                    }
                }

                return currentResult;
            }
        }

        public IEnumerable<T> JumpToPage(in int page)
        {
            if (page > 0)
            {
                if (previousResults.TryGetValue(page, out IEnumerable<T> cached)) return cached;
                else
                {
                    IIndexRange range = CalculatePageDimensions(page);

                    T[] currentResult = _Get(range).ToArray();

                    if (currentResult.Length > 0)
                    {
                        CurrentPage = page;

                        previousResults.Add(page, currentResult);

                        return currentResult;
                    }
                    else return EMPTY;
                }
            }
            else throw new ArgumentException("Cannot browse negative index.");
        }

        protected abstract IEnumerable<T> _Get(IIndexRange indexRange);

        protected interface IIndexRange
        {
            int Min { get; }
            int Max { get; }
        }

        private class IndexRange : IIndexRange
        {
            public int Min { get; }

            public int Max { get; }

            public IndexRange(int min, int max)
            {
                Min = min;
                Max = max;
            }
        }

        private IIndexRange CalculatePageDimensions(int page)
        {
            int min = ((page * PageSize) - PageSize) + 1;

            int max = page * PageSize;

            return new IndexRange(min, max);
        }
    }

    public abstract class AsyncPagedDataSource<T> : IAsyncOneWayDataSource<T>
    {
        private static readonly T[] EMPTY = Array.Empty<T>();

        private readonly IDictionary<int, IEnumerable<T>> previousResults;
        public IEnumerable<T> Entirety => previousResults.Values.SelectMany(cached => cached);

        public int PageSize { get; }

        public int CurrentPage { get; private set; } = 0;

        public bool EndReached { get; private set; } = false;

        protected AsyncPagedDataSource(int pageSize)
        {
            PageSize = pageSize;

            previousResults = new Dictionary<int, IEnumerable<T>>();
        }

        public async Task<IEnumerable<T>> NextAsync(CancellationToken token)
        {
            int nextPage = CurrentPage + 1;

            if (previousResults.TryGetValue(nextPage, out IEnumerable<T>? cached)) return cached;
            else
            {
                T[] currentResult = EMPTY;

                if (!EndReached)
                {
                    IIndexRange range = CalculatePageDimensions(nextPage);

                    currentResult = (await Get(range, token)).ToArray();

                    int length = currentResult.Length;

                    if (!(EndReached = length == 0 || length < PageSize))
                    {
                        previousResults.Add(nextPage, currentResult);

                        CurrentPage = nextPage;
                    }
                }

                return currentResult;
            }
        }

        public async Task<IEnumerable<T>> JumpToPage(int page, CancellationToken token)
        {
            if (page > 0)
            {
                if (previousResults.TryGetValue(page, out IEnumerable<T>? cached)) return cached;
                else
                {
                    IIndexRange range = CalculatePageDimensions(page);

                    T[] currentResult = (await Get(range, token)).ToArray();

                    if (currentResult.Length > 0)
                    {
                        CurrentPage = page;

                        previousResults.Add(page, currentResult);

                        return currentResult;
                    }
                    else return EMPTY;
                }
            }
            else throw new ArgumentException("Cannot browse negative index.");
        }

        protected abstract Task<IEnumerable<T>> Get(IIndexRange indexRange, CancellationToken token);

        protected interface IIndexRange
        {
            int Min { get; }
            int Max { get; }
        }

        private class IndexRange : IIndexRange
        {
            public int Min { get; }

            public int Max { get; }

            public IndexRange(int min, int max)
            {
                Min = min;
                Max = max;
            }
        }

        private IIndexRange CalculatePageDimensions(int page)
        {
            int min = ((page * PageSize) - PageSize) + 1;

            int max = page * PageSize;

            return new IndexRange(min, max);
        }
    }
}
