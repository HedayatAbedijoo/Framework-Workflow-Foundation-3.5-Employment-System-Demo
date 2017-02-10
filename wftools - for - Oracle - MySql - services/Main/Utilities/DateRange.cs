using System;

namespace WFTools.Utilities
{
    /// <summary>
    /// Basic implementation of a range of dates.
    /// </summary>
    public class DateRange : IEquatable<DateRange>
    {
        /// <summary>
        /// Create a new <see cref="DateRange" /> initialised with no start and end date.
        /// </summary>
        public DateRange() : this(null, null) { }
        /// <summary>
        /// Create a new <see cref="DateRange" /> initialised with the specified
        /// start and end dates.
        /// </summary>
        /// <param name="startDate">
        /// Nullable <see cref="DateTime"/> representing the start of the date range.
        /// </param>
        /// <param name="endDate">
        /// Nullable <see cref="DateTime"/> representing the end of the date range.
        /// </param>
        public DateRange(DateTime? startDate, DateTime? endDate)
        {
            validateStartAndEndDate(startDate, endDate);

            _startDate = startDate;
            _endDate = endDate;
        }

        private DateTime? _startDate;
        /// <summary>
        /// Gets/sets the start date of this date range.
        /// </summary>
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                validateStartDate(value);

                _startDate = value;
            }
        }

        private DateTime? _endDate;
        /// <summary>
        /// Gets/sets the end date of this date range.
        /// </summary>
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                validateEndDate(value);

                _endDate = value;
            }
        }

        /// <summary>
        /// Validates that the new start date is before the current end date.
        /// </summary>
        /// <param name="newStartDate">
        /// A nullable <see cref="DateTime" />.
        /// </param>
        private void validateStartDate(DateTime? newStartDate)
        {
            if (newStartDate.HasValue && _endDate.HasValue && newStartDate > _endDate)
                throw new ArgumentOutOfRangeException("newStartDate");
        }

        /// <summary>
        /// Validates that the new end date is after the current start date.
        /// </summary>
        /// <param name="newEndDate">
        /// A nullable <see cref="DateTime" />.
        /// </param>
        private void validateEndDate(DateTime? newEndDate)
        {
            if (_startDate.HasValue && newEndDate.HasValue && newEndDate < _startDate)
                throw new ArgumentOutOfRangeException("newEndDate");
        }

        /// <summary>
        /// Validates that the new start date is before the new end date.
        /// </summary>
        /// <param name="newStartDate">
        /// A nullable <see cref="DateTime" />.
        /// </param>
        /// <param name="newEndDate">
        /// A nullable <see cref="DateTime" />.
        /// </param>
        private static void validateStartAndEndDate(DateTime? newStartDate, DateTime? newEndDate)
        {
            if (newStartDate.HasValue && newEndDate.HasValue && newStartDate > newEndDate)
                throw new ArgumentOutOfRangeException("newStartDate");
        }

        ///<summary>
        ///Indicates whether the current object is equal to another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="dateToCompare">An object to compare with this object.</param>
        public Boolean Equals(DateRange dateToCompare)
        {
            if (ReferenceEquals(dateToCompare, null))
                return false;

            if (ReferenceEquals(dateToCompare, this))
                return true;

            return (_startDate == dateToCompare.StartDate && _endDate == dateToCompare.EndDate);
        }
    }
}