using System;
using System.Threading;

namespace WFTools.Services.Persistence
{
    /// <summary>
    /// Wraps a <see cref="Timer" /> by automatically updating its timeout based
    /// upon information derived during the callback.
    /// </summary>
    public sealed class SmartTimer : IDisposable
    {
        /// <summary>
        /// Construct a new instance of the SmartTimer using the specified
        /// callback, state, due time and interval.
        /// </summary>
        /// <param name="callback">
        /// A <see cref="TimerCallback" /> delegate representing a method to be executed. 
        /// </param>
        /// <param name="state">
        /// An object containing information to be used by the callback method, 
        /// or a null reference.
        /// </param>
        /// <param name="dueTime">
        /// The <see cref="TimeSpan" /> representing the amount of time to delay 
        /// before the callback parameter invokes its methods. 
        /// Specify negative one (-1) milliseconds to prevent the timer from starting. 
        /// Specify zero (0) to start the timer immediately. 
        /// </param>
        /// <param name="period">
        /// The time interval between invocations of the methods referenced by 
        /// callback. Specify negative one (-1) milliseconds to disable 
        /// periodic signaling. 
        /// </param>
        public SmartTimer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            _timerLock = new object();
            _period = period;
            _callback = callback;
            _next = DateTime.UtcNow + dueTime;
            _innerTimer = new Timer(handleCallback, state, dueTime, _infiniteInterval);
        }

        /// <summary>
        /// <see cref="TimeSpan" /> representing an infinite interval.
        /// </summary>
        private static readonly TimeSpan _infiniteInterval = new TimeSpan(-1);

        /// <summary>
        /// <see cref="TimeSpan" /> representing minimum allowable 
        /// time between intervals.
        /// </summary>
        private static readonly TimeSpan _minInterval = new TimeSpan(0, 0, 5);

        /// <summary>
        /// Delegate to be executed.
        /// </summary>
        private readonly TimerCallback _callback;

        /// <summary>
        /// Lock used when modifying the timer.
        /// </summary>
        private readonly object _timerLock;

        /// <summary>
        /// The inner timer wrapped by this class.
        /// </summary>
        private Timer _innerTimer;

        /// <summary>
        /// <see cref="DateTime" /> representing the next timeout for 
        /// the timer.
        /// </summary>
        private DateTime _next;

        /// <summary>
        /// Indicates the next timeout has changed.
        /// </summary>
        private Boolean _nextChanged;

        /// <summary>
        /// <see cref="TimeSpan" /> representing the interval between
        /// invocations of the callback.
        /// </summary>
        private readonly TimeSpan _period;

        /// <summary>
        /// Dispose the underlying timer.
        /// </summary>
        public void Dispose()
        {
            lock (_timerLock)
            {
                if (_innerTimer != null)
                {
                    _innerTimer.Dispose();
                    _innerTimer = null;
                }
            }
        }

        /// <summary>
        /// Handle a callback, update the inner timer with the next timeout value.
        /// </summary>
        private void handleCallback(object state)
        {
            try
            {
                _callback(state);
            }
            finally
            {
                lock (_timerLock)
                {
                    if (_innerTimer != null)
                    {
                        if (!_nextChanged)
                            _next = DateTime.UtcNow + _period;
                        else
                            _nextChanged = false;

                        TimeSpan newDueTime = _next - DateTime.UtcNow;
                        if (newDueTime < TimeSpan.Zero)
                            newDueTime = TimeSpan.Zero;

                        _innerTimer.Change(newDueTime, _infiniteInterval);
                    }
                }
            }
        }

        /// <summary>
        /// Update the inner timer with the next timeout period, but only
        /// if it is greater than the minimum interval.
        /// </summary>
        /// <param name="newNext"></param>
        public void Update(DateTime newNext)
        {
            if ((newNext < _next) && ((_next - DateTime.UtcNow) > _minInterval))
            {
                lock (_timerLock)
                {
                    if (((newNext < _next) && ((_next - DateTime.UtcNow) > _minInterval)) && (_innerTimer != null))
                    {
                        _next = newNext;
                        _nextChanged = true;
                        TimeSpan newDueTime = _next - DateTime.UtcNow;
                        if (newDueTime < TimeSpan.Zero)
                            newDueTime = TimeSpan.Zero;

                        _innerTimer.Change(newDueTime, _infiniteInterval);
                    }
                }
            }
        }
    }
}