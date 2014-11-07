using System.Collections.Generic;

namespace ATool.Common
{
    class MultiDictionary<R,C,V>
    {
        private readonly HashSet<R> _rows = new HashSet<R>();
        private readonly HashSet<C> _columns = new HashSet<C>();
        private readonly Dictionary<R,Dictionary<C,V>> _dictionary = new Dictionary<R, Dictionary<C, V>>();

        public HashSet<R> Rows { get { return _rows; } }
        public HashSet<C> Columns { get { return _columns; } }
        public Dictionary<R, Dictionary<C, V>> Dic { get { return _dictionary; } }

        public void Put(R row, C column, V vaule)
        {
            if (!Rows.Contains(row))
            {
                Rows.Add(row);
                var columnDictionary = new Dictionary<C, V>();
                foreach (var innerColumn in Columns)
                {
                    columnDictionary.Add(innerColumn, default(V));
                }
                _dictionary.Add(row, columnDictionary);
            }
            if (!Columns.Contains(column))
            {
                Columns.Add(column);
                foreach (var innerRow in Rows)
                {
                    _dictionary[innerRow].Add(column, default(V));
                }
            }
            _dictionary[row][column] = vaule;
        }

        public Dictionary<C, V> this[R row]
        {
            get
            {
                if (_dictionary.ContainsKey(row))
                {
                    return _dictionary[row];
                }
                return null;
            }
        }

        public void Clear()
        {
            _rows.Clear();
            _columns.Clear();
            _dictionary.Clear();
        }

    }
}
