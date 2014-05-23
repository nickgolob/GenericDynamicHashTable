﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTable
{
    public class HashTable<K,V> where K : IComparable
    {
        #region [ internal declarations ]

        private class node // red black tree sub-root
        {
            #region [ attributes ]

            public K key;
            public V value;
            public node left;      // reference to left child
            public node right;     // reference to right child
            public node parent;    // reference to parent node
            public Boolean red;    // color for red-black components

            #endregion

            #region [ constructors ]

            public node(K key, V value)
            {
                this.key = key;
                this.value = value;
                this.left = null;
                this.right = null;
                this.red = false;
            }

            #endregion

            #region [ private utilities ]

            /// <summary>
            /// disconnect minimim node (leaf) from subtree
            /// </summary>
            /// <param name="parent"> this is a reference to the parent </param>
            /// <returns>
            /// returns the disconnected node
            /// </returns>
            private node popMin(node parent)
            {
                if (this.left == null) {
                    parent.left = this.right;
                    return this;
                }
                else
                {
                    return this.left.popMin(this);
                }
            }

            #endregion

            #region [ public methods ]

            public void insert(node entry)
            {
                if (this.key.CompareTo(entry.key) < 0)
                {
                    if (this.right == null)
                    {
                        this.right = entry;
                    }
                    else
                    {
                        this.right.insert(entry);
                    }
                }
                else
                {
                    if (this.left == null)
                    {
                        this.left = entry;
                    }
                    else
                    {
                        this.left.insert(entry);
                    }
                }
                entry.red = true;
            }

            public node search(K key) 
            {
                if (this.key.CompareTo(key) == 0)
                {
                    return this;
                }
                else if (this.key.CompareTo(key) < 0)
                {
                    if (this.right == null)
                    {
                        throw new Exception("key not found");
                    }
                    else
                    {
                        return this.right.search(key);
                    }
                }
                else
                {
                    if (this.left == null)
                    {
                        throw new Exception("key not found");
                    }
                    else
                    {
                        return this.left.search(key);
                    }
                }     
            }

            /// <summary>
            /// deletes the targeted node, and restructures the tree
            /// </summary>
            /// <param name="key"> key of node targeted for removal </param>
            /// <returns>
            /// the (possibly new) root of the tree
            /// </returns>
            /// <remarks>
            /// - initial replacement is the minimal node in the right subtree
            ///     if node has two children
            /// - this routine assumes 'key' is present, and will crash otherwise
            /// </remarks>
            public node delete(K key)
            {
                if (this.key.CompareTo(key) == 0)
                {
                    if ((this.right != null) && (this.left != null))
                    {
                        node replacement = this.popMin(this);
                        replacement.left = this.left;
                        replacement.right = this.right;
                        return replacement;
                    }
                    else if ((this.right == null) && (this.left != null))
                    {
                        return this.left;
                    }
                    else if ((this.right != null) && (this.left == null))
                    {
                        return this.right;
                    }
                    else
                    {
                        return null;
                    }    
                }
                else if (this.key.CompareTo(key) < 0)
                {
                    this.right = this.right.delete(key);
                    return this;
                }
                else
                {
                    this.left = this.left.delete(key);
                    return this;
                }
            }

            #endregion
        }

        //private class redBlackTree
        //{
        //    public node root;

        //    public void insert()
        //    {

        //    }

        //    public V 

        //}




        private const int DEFAULT_INITIAL_SIZE = 1;
        private const float DEFAULT_MAX_LOAD = 2.0F;


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
