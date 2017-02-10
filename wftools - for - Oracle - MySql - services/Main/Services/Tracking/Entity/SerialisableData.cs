using System;
using System.Diagnostics.CodeAnalysis;

namespace WFTools.Services.Tracking.Entity
{
    /// <summary>
    /// Representation of a serialised version of some data.
    /// </summary>
    public class SerialisableData
    {
        private Object _unserialisedData;
        /// <summary>
        /// Gets/sets the unserialised data.
        /// </summary>
        public Object UnserialisedData
        {
            get { return _unserialisedData; }
            set { _unserialisedData = value; }
        }

        private Byte[] _serialisedData;
        /// <summary>
        /// Gets/sets the serialised representation of the <see cref="UnserialisedData" /> property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", 
            Justification = "byte [] contains serialised data, cannot be represented as a collection")]
        public Byte[] SerialisedData
        {
            get { return _serialisedData; }
            set { _serialisedData = value; }
        }

        private String _stringData;
        /// <summary>
        /// Gets/sets the string representation of the <see cref="UnserialisedData" /> property.
        /// </summary>
        public String StringData
        {
            get { return _stringData; }
            set { _stringData = value; }
        }

        private Type _type;
        /// <summary>
        /// Gets/sets the <see cref="Type" /> of the <see cref="UnserialisedData" /> property.
        /// </summary>
        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private Boolean _nonSerialisable;
        /// <summary>
        /// Gets/sets whether the <see cref="UnserialisedData" /> property
        /// is serialisable.
        /// </summary>
        public Boolean NonSerialisable
        {
            get { return _nonSerialisable; }
            set { _nonSerialisable = value; }
        }
    }
}
