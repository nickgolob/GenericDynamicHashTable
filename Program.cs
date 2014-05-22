using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTable
{
    public class HashTable<K,V> where K : IComparable
    {
        #region [ internal declarations ]

        private const int DEFAULT_INITIAL_SIZE = 1;
        private const float DEFAULT_MAX_LOAD = 2.0F;

        private class node
        {
            public K key;
            public V value;
            public node left;      // pointer to left child
            public node right;     // pointer to right child
            public Boolean red;    // color for red-black components

            public node(K key, V value)
            {
                this.key = key;
                this.value = value;
                this.left = null;
                this.right = null;
                this.red = false;
            }
        }

        #endregion

        #region [ private attributes ]

        private int entries;
        private int maxSize;
        private node[] table;   
        private Func<K, int> map;   // hash function

        // optional parameters
        private float loadFactor;       // max entries:maxSize ratio before split
        private bool duplicateKeys;  // flag representing allowance of duplicate keys

        #endregion

        #region [ constructors ]

        public HashTable(
            Func<K, int> map, 
            float loadFactor = DEFAULT_MAX_LOAD, 
            int maxSize = DEFAULT_INITIAL_SIZE, 
            bool duplicateKeys = true)
        {
            this.entries = 0;
            this.maxSize = maxSize;
            this.table = new node[maxSize];
            this.map = map;
            this.loadFactor = loadFactor;
            this.duplicateKeys = duplicateKeys;
        }

        #endregion

        #region [ private utilities ]

        private void expand()
        {
            this.maxSize *= 2;
            node[] newTable = new node[maxSize];
            
        }

        #endregion

        #region [ public methods ]

        public void insert(K key, V value)
        {
            int tableIndex = this.map(key) % this.maxSize;
            node entry = new node(key, value);

            if (this.table[tableIndex] == null)
            {
                this.table[tableIndex] = entry;
            }
            else
            {
                node scanner;
                for (scanner = this.table[tableIndex]; 
                    scanner.left != null; 
                    scanner = scanner.left)
                {
                    if (this.duplicateKeys && scanner.key.CompareTo(key) == 0)
                    {
                        throw new Exception("duplicate key");
                    }
                }
                scanner.left = entry;
            }

            this.entries += 1;

            if (this.entries / this.maxSize > this.loadFactor)
            {
                this.expand();
            }
        }

        public V search(K key)
        {
            int tableIndex = this.map(key) % this.maxSize;

            return this.table[tableIndex].value;
        }

        public void delete(K Key)
        {

        }

        #endregion

    }




    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
