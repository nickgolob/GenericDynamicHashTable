using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTable
{
    public class HashTable<K, V> where K : IComparable
    {
        #region [ internal declarations ]

        private const int DEFAULT_INITIAL_SIZE = 1;
        private const int DEFAULT_RESIZE_FACTOR = 2;
        private const float DEFAULT_MAX_LOAD = 2.0F;

        private const bool A = true, B = false;

        private class bucket
        {
            #region [ attributes ]
            public int entries;
            public RedBlackTree<K, V> chain;
            public bucket marker;
            #endregion

            #region [ constructors ]
            public bucket()
            {
                this.entries = 0;
                this.chain = new RedBlackTree<K, V>();
                this.marker = null;
            }
            #endregion

            #region [ methods ]

            #endregion
        }

        #endregion

        #region [ private attributes ]

        private Func<K, int, int> map;

        private bucket[] currentTable, prevTable;
        private int currentEntries, prevEntries, currentMaxSize, prevMaxSize;

        private int resizeFactor;
        private float loadFactor;
        private bool duplicateKeys;

        private bucket resizeLink;

        #endregion

        #region [ constructors ]

        /// <summary>
        /// the constructor for the hash table
        /// </summary>
        /// <param name="map">
        ///  - This is the hash function.
        ///  - The function maps the key of an input as the first parameter, 
        ///     and the size of the hash table as the second parameter, to a
        ///     slot in the hash table. 
        ///  - The output will be mapped modulo size of the table, so bounds
        ///     need not necessarily be considered in function definition.
        ///  - The unconventional second parameter (the size of the hash
        ///     table) enable more control over the distribution of hashes
        ///     for the dynamic table.    
        /// </param>
        /// <param name="loadFactor">
        ///  - load factor is an optional parameter denoting the
        ///     max entries:maxSize ratio before the table resizes.
        /// </param>
        /// <param name="initialSize">
        ///  - initialSize is an optional parameter denoting initial size
        ///     of the table.
        /// </param>
        /// <param name="duplicateKeys">
        ///  - duplicate keys is an optional paramerer indicating whether
        ///     duplicate keys are to be accepted.
        ///  - behavior of the hash table, if duplicate keys are to be
        ///     accepted will be to return the first found is searched,
        ///     and delete the first found if deleted.
        /// </param>
        public HashTable(
            Func<K, int, int> map, 
            float loadFactor = DEFAULT_MAX_LOAD, 
            int initialSize = DEFAULT_INITIAL_SIZE, 
            bool duplicateKeys = false)
        {
            this.map = map;

            this.currentTable = new bucket[initialSize];
            this.currentEntries = 0;
            this.prevEntries = 0;
            this.currentMaxSize = initialSize;

            this.resizeFactor = DEFAULT_RESIZE_FACTOR;
            this.loadFactor = loadFactor;

            this.duplicateKeys = duplicateKeys;

            this.resizeLink = null;
        }

        /// <summary>
        /// constructor with hash function ommitting the unconventional
        /// second argument
        /// </summary>
        public HashTable(
            Func<K, int> map,
            float loadFactor = DEFAULT_MAX_LOAD,
            int initialSize = DEFAULT_INITIAL_SIZE,
            bool duplicateKeys = false)
            : this( delegate(K key, int n) { return map(key); },
                    loadFactor, initialSize, duplicateKeys) { }

        #endregion

        #region [ private utilities ]

        /// <summary>
        /// applies hash function to a key, returns bucket
        /// </summary>
        /// <param name="current">
        /// a boolean denoting which table should be mapped to.
        /// true designates the current table, false designates the
        /// previous table.
        /// </param>
        private bucket hash(K key, bool current)
        {
            if (current)
                return this.currentTable[this.map(key, this.currentMaxSize) % this.currentMaxSize];
            else
                return this.prevTable[this.map(key, this.prevMaxSize) % this.prevMaxSize];
        }


        /// <summary>
        /// creates a new hash table
        /// </summary>
        /// <param name="expand">
        /// boolean denoting whether the table should expand.
        /// If false, the table will instead contract
        /// </param>
        /// <remarks>
        /// - the size of the new table will be the previous size
        ///     times or dicided by the resizing factor
        /// - in an EXPANSION:
        ///     this function assumes the old table is empty, as the
        ///     current table will now become the old table, and
        ///     the old table will be deallocated.
        /// - in a CONTRACTION:
        /// 
        /// </remarks>
        private void resize(bool expand)
        {
            if (expand)
            {
                this.prevEntries = this.currentEntries;
                this.prevMaxSize = this.currentMaxSize;
                this.prevTable = this.currentTable;

                this.currentEntries = 0;
                this.currentMaxSize = this.prevMaxSize * this.resizeFactor;
                this.currentTable = new bucket[currentMaxSize];
            }
            else
            {

            }
        }

        #endregion

        #region [ public methods ]

        public void insert(K key, V value)
        {
            // resize table if needed
            if ((this.currentEntries + 1) / this.currentMaxSize > this.loadFactor)
                this.resize(true);

            // insert element
            bucket target = this.hash(key, true);
            target.entries += 1;
            target.chain.insert(key, value);

            if (prevEntries > 0) {

                // scan previous table
                

            }


        }


        /// <summary>
        /// return the value of an element in the table
        /// </summary>
        /// <exception cref="key not found">
        /// is thrown if no element with the specified key
        /// is in the table.
        /// </exception>
        public V search(K key)
        {
            bool firstTable;

            if (this.prevEntries == 0)
                return this.hash(key, true).chain.search(key).value;
            else if (this.currentEntries >= this.prevEntries)
                firstTable = true;
            else
                firstTable = false;

            try
            {
                return this.hash(key, firstTable).chain.search(key).value;
            }
            catch
            {
                return this.hash(key, !firstTable).chain.search(key).value;
            }
        }
        /// <summary>
        /// return whether or not an element is in the table
        /// </summary>
        public bool exists(K key)
        {
            try
            {
                V test = this.search(key);
                return true;
            }
            catch
            {
                return false;
            }
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

            // tests:

            RedBlackTree<int, char> tree = new RedBlackTree<int,char>();

            tree.insert(2, 'B');
            tree.insert(1, 'A');
            tree.insert(3, 'C');
            tree.insert(4, 'D');
            tree.insert(5, 'E');
            tree.insert(6, 'F');
            tree.insert(7, 'G');
            tree.delete(4);
            tree.delete(1);
            tree.delete(7);
            tree.delete(6);
            tree.delete(5);

        }
    }
}
