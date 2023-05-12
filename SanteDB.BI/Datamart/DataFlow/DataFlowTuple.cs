using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SanteDB.BI.Datamart.DataFlow
{
    /// <summary>
    /// Represents a data tuple
    /// </summary>
    internal class DataFlowStreamTuple : IDictionary<String, Object>
    {

        // Backing dictionary
        private readonly IDictionary<String, Object> m_dictionary;

        /// <inheritdoc/>
        public ICollection<string> Keys => this.m_dictionary.Keys;

        /// <inheritdoc/>
        public ICollection<object> Values => this.m_dictionary.Values;

        /// <inheritdoc/>
        public int Count => this.m_dictionary.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// Default ctor (creates a new output tuple)
        /// </summary>
        public DataFlowStreamTuple()
        {
            this.m_dictionary = new ExpandoObject();
        }

        /// <summary>
        /// Create an input tuple based on the provided data
        /// </summary>
        public DataFlowStreamTuple(IDictionary<String, Object> inputData)
        {
            this.m_dictionary = inputData.ToDictionary(o => o.Key.ToLowerInvariant(), o => o.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Object this[String name]
        {
            get => this.GetData(name);
            set => this.SetData(name, value);
        }

        /// <summary>
        /// Set data in the tuple
        /// </summary>
        public void SetData(string name, object value)
        {
            if (this.m_dictionary.ContainsKey(name.ToLowerInvariant()))
            {
                this.m_dictionary[name.ToLowerInvariant()] = value;
            }
            else
            {
                this.m_dictionary.Add(name.ToLowerInvariant(), value);
            }
        }

        /// <summary>
        /// Get data from the tuple
        /// </summary>
        public object GetData(string name) => this.m_dictionary.TryGetValue(name.ToLowerInvariant(), out var result) ? result : null;

        /// <summary>
        /// Represent as a string
        /// </summary>
        public override string ToString() => String.Join(",", this.m_dictionary.Values);

        /// <inheritdoc/>
        public void Add(string key, object value) => this.SetData(key, value);

        /// <inheritdoc/>
        public bool ContainsKey(string key) => this.m_dictionary.ContainsKey(key.ToLowerInvariant());

        /// <inheritdoc/>
        public bool Remove(string key) => throw new NotSupportedException();

        /// <inheritdoc/>
        public bool TryGetValue(string key, out object value) => this.m_dictionary.TryGetValue(key.ToLowerInvariant(), out value);

        /// <inheritdoc/>
        public void Add(KeyValuePair<string, object> item) => this.SetData(item.Key, item.Value);

        /// <inheritdoc/>
        public void Clear() => throw new NotSupportedException();

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<string, object> item) => this.m_dictionary.Contains(new KeyValuePair<string, object>(item.Key.ToLowerInvariant(), item.Value));

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => this.m_dictionary.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<string, object> item) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.m_dictionary.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
