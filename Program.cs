////////////////////////////////////
// 
// Dynamic Hash Table data structure
// written by Nick Golob, 4/14/2014
// 
////////////////////////////////////

namespace HashTable
{
    using System;

    public class HashTable<K, V> where K : IComparable
    {
        #region [ internal declarations ]

        private const int DEFAULT_INITIAL_SIZE = 1;
        private const int DEFAULT_RESIZE_FACTOR = 2;
        private const float DEFAULT_MAX_LOAD = 1.0F;

        private const bool CURRENT = true, PREVIOUS = false;

        private class bucket
        {
            #region [ attributes ]
            public int entries;
            public RedBlackTree<K, V> chain;
            public bucket link;
            #endregion

            #region [ constructors ]
            public bucket()
            {
                this.entries = 0;
                this.chain = new RedBlackTree<K, V>();
                this.link = null;
            }
            #endregion

            #region [ methods ]

            #endregion
        }

        #endregion

        #region [ private attributes ]

        /* hash function */
        private Func<K, int, int> map;

        /* tables and sizes */
        private bucket[] currentTable, prevTable;
        private int currentEntries, prevEntries, currentMaxSize, prevMaxSize;

        /* constants */
        private int resizeFactor;
        private float loadFactor;
        private bool duplicateKeys;
        private int initialSize;

        /* incremental resizing attributes */
        private int prevThreshold;  // initial number of elements in prev table
        private int resizeIndex;    // current bucket index for prev table scan
        private bucket resizeHead;  // reference to bucket at head of chain
        private bucket resizeLast;  // reference to last bucked in chain

        /* single entry cache, for rapid exists/search calls
         * and displacement across tables */
        private RedBlackTree<K, V>.Node cache;  // reference to cached node
        private bool cacheTable;                // denotes table chached element is in

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
            if ((loadFactor <= 0) || (initialSize < 0))
                throw new Exception("bad parameters");

            this.map = map;

            this.initialSize = initialSize;
            this.currentTable = new bucket[initialSize];
            this.currentEntries = 0;
            this.prevEntries = 0;
            this.currentMaxSize = initialSize;

            this.resizeFactor = DEFAULT_RESIZE_FACTOR;
            this.loadFactor = loadFactor;

            this.duplicateKeys = duplicateKeys;

            this.resizeIndex = 0;
            this.resizeHead = null;
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
            : this((key, n) => map(key), loadFactor, initialSize, duplicateKeys) { }

        #endregion

        #region [ private utilities ]

        /// <summary>
        /// applies hash function to a key, returns bucket
        /// </summary>
        /// <param name="table">
        /// a boolean denoting which table should be mapped to.
        /// true designates the current table, false designates the
        /// previous table.
        /// </param>
        /// <param name="create">
        /// boolean designating whether or not a bucket should be instantiated
        /// if hash maps to a null bucket. (i.e insertion should be true)
        /// </param>
        private bucket hash(K key, bool table, bool create)
        {
            if (table == CURRENT)
            {
                int index = this.map(key, this.currentMaxSize) % this.currentMaxSize;
                if (create && (this.currentTable[index] == null))
                    this.currentTable[index] = new bucket();
                return this.currentTable[index];
            }
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
            this.prevThreshold = this.currentEntries;

            this.prevEntries = this.currentEntries;
            this.prevMaxSize = this.currentMaxSize;
            this.prevTable = this.currentTable;
            this.currentEntries = 0;

            if (expand)
                this.currentMaxSize = this.prevMaxSize * this.resizeFactor;
            else
                this.currentMaxSize = (int)Math.Ceiling((double)this.prevMaxSize / this.resizeFactor);

            this.currentTable = new bucket[currentMaxSize];

            this.resizeIndex = 0;
            this.resizeHead = null;
            this.resizeLast = null;
        }

        /// <summary>
        /// performes the displacement from old to new table in an
        /// incrmemental resize. This will involve table scanning
        /// and bucket linking, as well as actually movement of
        /// previous entries to new table.
        /// </summary>
        private void displace()
        {
            for (int i = 0;
                this.prevEntries > 0 && i < Math.Ceiling((double)2 / (this.resizeFactor - 1));
                i++)
            {
                if (this.resizeHead != null)
                {
                    /* node displacement */
                    RedBlackTree<K, V>.Node target = this.resizeHead.chain.root;

                    /* removal */
                    this.resizeHead.chain.remove(target);
                    this.resizeHead.entries -= 1;
                    if (resizeHead.entries == 0)
                        this.resizeHead = this.resizeHead.link;
                    this.prevEntries -= 1;

                    /* re-insertion */
                    bucket dest = this.hash(target.key, CURRENT, true);
                    dest.chain.insert(target);
                    dest.entries += 1;
                    this.currentEntries += 1;
                }
                else
                {
                    /* table scan / bucket linking */
                    for (int j = 0;
                        this.resizeIndex < this.prevMaxSize &&
                        j < Math.Ceiling((double)this.prevMaxSize / this.prevThreshold);
                        j++, this.resizeIndex++)
                    {
                        if (this.prevTable[this.resizeIndex].entries <= 0) 
                            continue;
                        if (this.resizeHead == null)
                            this.resizeHead = this.prevTable[this.resizeIndex];
                        else
                            this.resizeLast.link = this.prevTable[this.resizeIndex];
                        this.resizeLast = this.prevTable[this.resizeIndex];
                    }
                }
            }
        }

        #endregion

        #region [ public methods ]

        /// <summary>
        /// insert a new entry in the hash table
        /// </summary>
        public void insert(K key, V value)
        {

            /* expand table if needed */
            if ((double)(this.currentEntries + 1) / this.currentMaxSize > this.loadFactor)
                this.resize(true);

            /* insert element */
            bucket target = this.hash(key, CURRENT, true);
            target.chain.insert(key, value);
            target.entries += 1;
            this.currentEntries += 1;

            /* wipe cache */
            this.cache = null;

            /* incremental structuring */
            this.displace();
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
            /* check cache */
            if (key.CompareTo(this.cache.key) == 0)
                return this.cache.value;

            /* search for it, cache it if it exists */
            if (!this.exists(key))
                throw new Exception("key not found");

            /* return the cached result */
            return this.cache.value;
        }

        /// <summary>
        /// return whether or not an element is in the table.
        /// </summary>
        /// <remarks>
        /// If it exists, it will cache the Node, and the current table info.
        /// Another call to exists will overwrite cache.
        /// </remarks>
        public bool exists(K key)
        {
            RedBlackTree<K, V>.Node result;

            /* check if it exists */
            try
            {
                result = this.hash(key, CURRENT, false).chain.search(key);
                this.cacheTable = CURRENT;
                this.cache = result;
            }
            catch
            {
                if (this.prevEntries > 0)
                    try
                    {
                        result = this.hash(key, PREVIOUS, false).chain.search(key);
                        this.cacheTable = PREVIOUS;
                        this.cache = result;
                        return true;
                    }
                    catch
                    {
                    }
                return false;
            }

            return true;
        }

        /// <summary>
        /// delete a node from the table
        /// </summary>
        /// <exception cref="key not found">
        /// thrown if key is not in table
        /// </exception>
        public void delete(K key)
        {
            /* find entry */
            if (!this.exists(key))
                throw new Exception("key not found");

            /* remove entry */
            bucket target = this.hash(key, this.cacheTable, false);
            target.entries -= 1;
            target.chain.remove(this.cache);

            /* wipe cache */
            this.cache = null;

            /* contract table if needed */
            if ((this.currentEntries - 1) < this.currentMaxSize * this.loadFactor / Math.Pow(this.resizeFactor, 2) &&
                (this.currentMaxSize / this.resizeFactor > this.initialSize))
                this.resize(false);

            /* incremental structuring */
            this.displace();
        }

        #endregion

    }

}
