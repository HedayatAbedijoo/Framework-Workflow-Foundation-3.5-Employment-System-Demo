using System;
using System.Collections;
using System.Collections.Generic;

namespace WFTools.Utilities
{
    /// <summary>
    /// Represents the results from a search operation.
    /// </summary>
    public class SearchResults<T> : IEnumerable<T>
    {
        /// <summary>
        /// Construct a new <see cref="SearchResults{T}" />.
        /// </summary>
        public SearchResults() { }

        /// <summary>
        /// Construct a new <see cref="SearchResults{T}" /> specifying the 
        /// start offset, maximum results and total results of a search 
        /// operation, as well as the results of the search operation.
        /// </summary>
        /// <param name="startOffset">
        /// Start offset of the search operation.
        /// </param>
        /// <param name="maxResults">
        /// The maximum number of results to return.
        /// </param>
        /// <param name="totalResults">
        /// The total results returned from the search operation.
        /// </param>
        /// <param name="results">
        /// The results themselves.
        /// </param>
        public SearchResults(Int32 startOffset, Int32 maxResults, Int32 totalResults, IEnumerable<T> results)
        {
            _startOffset = startOffset;
            _maxResults = maxResults;
            _totalResults = totalResults;

            if (results != null)
            {
                foreach (T result in results)
                    _results.Add(result);
            }
        }

        private readonly Int32 _startOffset;
        /// <summary>
        /// Gets the start offset within which to retrieve results.
        /// </summary>
        public Int32 StartOffset
        {
            get { return _startOffset; }
        }

        private readonly Int32 _maxResults;
        /// <summary>
        /// Gets the maximum required results.
        /// </summary>
        public Int32 MaxResults
        {
            get { return _maxResults; }
        }

        private readonly Int32 _totalResults;
        /// <summary>
        /// Gets the total available results.
        /// </summary>
        public Int32 TotalResults
        {
            get { return _totalResults; }
        }

        private readonly IList<T> _results = new List<T>();
        /// <summary>
        /// Gets the actual results.
        /// </summary>
        public IList<T> Results
        {
            get { return _results; }
        }

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        /// <summary>
        /// Convert actual results into an array of type <typeparamref name="T"/>.
        /// </summary>
        public static implicit operator T[](SearchResults<T> searchResults)
        {
            T[] arrayOfT = new T[searchResults.Results.Count];
            searchResults.Results.CopyTo(arrayOfT, 0);
            return arrayOfT;
        }

        /// <summary>
        /// Convert actual results into a <see cref="List{T}" />.
        /// </summary>
        public static implicit operator List<T>(SearchResults<T> searchResults)
        {
            return new List<T>(searchResults.Results);
        }
    }
}
