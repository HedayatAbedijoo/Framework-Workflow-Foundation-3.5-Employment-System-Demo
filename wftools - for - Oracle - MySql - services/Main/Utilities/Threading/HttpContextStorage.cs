using System.Web;

namespace WFTools.Utilities.Threading
{
    /// <summary>
    /// Implements <see cref="IThreadStorage"/> by using <see cref="HttpContext.Items"/>.
    /// </summary>
    /// <remarks>Copied from Spring.NET v1.1 (http://www.springframework.net/</remarks>
    public class HttpContextStorage : IThreadStorage
    {
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
            return HttpContext.Current.Items[name];
        }

        /// <summary>
        /// Stores a given object and associates it with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name with which to associate the new item.</param>
        /// <param name="value">The object to store in the current thread's context.</param>
        public void SetData(string name, object value)
        {
            HttpContext.Current.Items[name] = value;
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
            HttpContext.Current.Items.Remove(name);
        }

        /// <summary>
        /// Indicates whether the underlying storage mechanism is currently available.
        /// </summary>
        public bool IsAvailable()
        {
            return HttpContext.Current != null;
        }
    }
}