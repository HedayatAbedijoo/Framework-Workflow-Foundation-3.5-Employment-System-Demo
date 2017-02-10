using System;
using WFTools.Utilities.Threading;

namespace WFTools.Utilities.Threading
{
    /// <summary>
    /// Implements <see cref="IThreadStorage"/> by using a combination
    /// of one or more other <see cref="IThreadStorage" /> implementations.
    /// </summary>
    public class HybridStorage : IThreadStorage
    {
        /// <summary>
        /// Primary storage -  by default uses <see cref="HttpContextStorage" />.
        /// </summary>
        private readonly IThreadStorage primaryStorage = new HttpContextStorage();
        /// <summary>
        /// Secondary storage - by default uses <see cref="CallContextStorage" />.
        /// </summary>
        private readonly IThreadStorage secondaryStorage = new CallContextStorage();

        /// <summary>
        /// Retrieves an object with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <returns>
        /// The object in the current thread's context associated with the 
        /// specified <paramref name="name"/> or null if no object has been stored previously
        /// </returns>
        public object GetData(string name)
        {
            if (primaryStorage.IsAvailable())
                return primaryStorage.GetData(name);
            else if (secondaryStorage.IsAvailable())
                return secondaryStorage.GetData(name);
            else
                throw createNotSupportedException();
        }

        /// <summary>
        /// Stores a given object and associates it with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name with which to associate the new item.</param>
        /// <param name="value">The object to store in the current thread's context.</param>
        public void SetData(string name, object value)
        {
            if (primaryStorage.IsAvailable())
                primaryStorage.SetData(name, value);
            else if (secondaryStorage.IsAvailable())
                secondaryStorage.SetData(name, value);
            else
                throw createNotSupportedException();
        }

        /// <summary>
        /// Empties a data slot with the specified name.
        /// </summary>
        /// <remarks>
        /// If the object with the specified <paramref name="name"/> is not found, the method does nothing.
        /// </remarks>
        /// <param name="name">The name of the object to remove.</param>
        public void FreeNamedDataSlot(string name)
        {
            if (primaryStorage.IsAvailable())
                primaryStorage.FreeNamedDataSlot(name);
            else if (secondaryStorage.IsAvailable())
                secondaryStorage.FreeNamedDataSlot(name);
            else
                throw createNotSupportedException();
        }

        /// <summary>
        /// Indicates whether the underlying storage mechanism is currently available.
        /// </summary>
        public bool IsAvailable()
        {
            return primaryStorage.IsAvailable() || secondaryStorage.IsAvailable();
        }

        private static NotSupportedException createNotSupportedException()
        {
            return new NotSupportedException("No IThreadStorage implementation available.");
        }
    }
}