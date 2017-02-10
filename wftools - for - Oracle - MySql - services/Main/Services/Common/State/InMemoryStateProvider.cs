using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WFTools.Services.Common.State
{
    /// <summary>
    /// Implementation of <see cref="IStateProvider" /> that uses an in-memory
    /// dictionary indexed by key.
    /// </summary>
    public class InMemoryStateProvider : IStateProvider
    {
        private readonly Dictionary<String, Object> _stateStorage = new Dictionary<String, Object>();

        /// <summary>
        /// Retrieve an item from storage.
        /// </summary>
        /// <param name="key">
        /// Unique identifier of an item in storage.
        /// </param>
        /// <returns>
        /// An <see cref="Object" /> representing the item in storage, 
        /// or <c>null</c> if the item was not found.
        /// </returns>
        public Object Get(String key)
        {
            if (_stateStorage.ContainsKey(key))
                return _stateStorage[key];
                
            return null;
        }

        /// <summary>
        /// Retrieve an item from storage, casting it to the specified type.
        /// </summary>
        /// <param name="key">
        /// Unique identifier of an item in storage.
        /// </param>
        /// <typeparam name="T">
        /// Type to cast the returned item to.
        /// </typeparam>
        /// <returns>
        /// The item from storage, or <c>null</c> if the item was not found.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Code analysis mistakenly marks this method as failing this rule.")]
        public T Get<T>(String key)
        {
            if (_stateStorage.ContainsKey(key))
                return (T) _stateStorage[key];
            
            return default(T);
        }

        /// <summary>
        /// Add an item to storage using the specified unique key.
        /// </summary>
        /// <param name="key">
        /// Unique identifier of the item.
        /// </param>
        /// <param name="value">
        /// The item to add.
        /// </param>
        public void Add(String key, Object value)
        {
            if (_stateStorage.ContainsKey(key))
                throw new ArgumentException(RM.Get_Error_KeyAlreadyExists(key));

            _stateStorage.Add(key, value);
        }

        /// <summary>
        /// Add an item to storage using the specified unique key.
        /// </summary>
        /// <param name="key">
        /// Unique identifier of the item.
        /// </param>
        /// <param name="value">
        /// The item to add.
        /// </param>
        public void Add<T>(String key, T value)
        {
            if (_stateStorage.ContainsKey(key))
                throw new ArgumentException(RM.Get_Error_KeyAlreadyExists(key));

            _stateStorage.Add(key, value);
        }

        /// <summary>
        /// Remove an item to storage using the specified unique key.
        /// </summary>
        /// <param name="key">
        /// Unique identifier of the item.
        /// </param>
        public void Remove(String key)
        {
            if (!_stateStorage.ContainsKey(key))
                throw new KeyNotFoundException(RM.Get_Error_KeyNotFound(key));

            _stateStorage.Remove(key);
        }

        /// <summary>
        /// Update an item in storage using the specified unique key.
        /// </summary>
        /// <param name="key">
        /// Unique identifier of the item to update.
        /// </param>
        /// <param name="value">
        /// The item to update.
        /// </param>
        public void Update(String key, Object value)
        {
            if (!_stateStorage.ContainsKey(key))
                throw new KeyNotFoundException(RM.Get_Error_KeyNotFound(key));
            
            _stateStorage[key] = value;
        }

        /// <summary>
        /// Update an item in storage using the specified unique key.
        /// </summary>
        /// <param name="key">
        /// Unique identifier of the item to update.
        /// </param>
        /// <param name="value">
        /// The item to update.
        /// </param>
        public void Update<T>(String key, T value)
        {
            if (!_stateStorage.ContainsKey(key))
                throw new KeyNotFoundException(RM.Get_Error_KeyNotFound(key));

            _stateStorage[key] = value;
        }

        /// <summary>
        /// Indicates whether an item exists in storage.
        /// </summary>
        /// <param name="key">
        /// Unique identifier of the item.
        /// </param>
        public bool Contains(String key)
        {
            return _stateStorage.ContainsKey(key);
        }
    }
}