using System;
using System.Collections;
using WFTools.Utilities.Threading;

namespace WFTools.Utilities.Threading
{
    /// <summary>
    /// Implements <see cref="ThreadStaticAttribute"/> by using a <see cref="IThreadStorage"/> hashtable.
    /// </summary>
    /// <author>Erich Eichinger</author>
    /// <version>$Id: ThreadStaticStorage.cs,v 1.1 2007/02/02 21:30:34 oakinger Exp $</version>  
    /// <remarks>Copied from Spring.NET v1.1 (http://www.springframework.net/</remarks>
    public class ThreadStaticStorage : IThreadStorage
    {
        [ThreadStatic]
        private static Hashtable data;

        private static Hashtable Data
        {
            get
            {
                if (data == null) data = new Hashtable();
                return data;
            }
        }

        /// <summary>
        /// Retrieves an object with the specified name.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <returns>The object in the call context associated with the specified name or null if no object has been stored previously</returns>
        public object GetData(string name)
        {
            return Data[name];
        }

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item.</param>
        /// <param name="value">The object to store in the call context.</param>
        public void SetData(string name, object value)
        {
            Data[name] = value;
        }

        /// <summary>
        /// Empties a data slot with the specified name.
        /// </summary>
        /// <param name="name">The name of the data slot to empty.</param>
        public void FreeNamedDataSlot(string name)
        {
            Data.Remove(name);
        }

        /// <summary>
        /// Indicates whether the underlying storage mechanism is currently available.
        /// </summary>
        public bool IsAvailable()
        {
            return true;
        }
    }
}