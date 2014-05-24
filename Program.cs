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
                this.parent = null;
                this.red = false;
            }

            #endregion

            #region [ private utilities ]

            /// <summary>
            /// hooks up a node as the left or right child of the subject node.
            /// </summary>
            /// <param name="newChild"> the new node to be connected </param>
            /// <param name="left"> denotes wether newChild is to be left or right child </param>
            /// <remark> this accepts null parameters as newChild </remark>
            private void quickLink(node newChild, bool left)
            {
                if (left) 
                {
                    this.left = newChild;
                }
                else 
                {
                    this.right = newChild;
                }
                if (newChild != null)
                {
                    newChild.parent = this;
                }
            }

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
                    this.parent.quickLink(this.right, true);
                    return this;
                }
                else
                {
                    return this.left.popMin(this);
                }
            }

            /// <summary>
            /// performs a rotation about the subject node.
            /// </summary>
            /// <param name="left"> denotes type of rotation </param>
            /// <remark> 
            /// - left rotations assume right child is not null
            /// - right rotations assume left child is not null
            ///</remark>
            private void rotate(bool left)
            {
                node replacement;
                if (left)
                {
                    replacement = this.right;
                    this.right = replacement.left;
                    if (replacement.left != null)
                        replacement.left.parent = this;
                    replacement.parent = this.parent;
                    if (this.parent != null)
                    {
                        if (this.parent.left == this)
                            this.parent.left = replacement;
                        else
                            this.parent.right = replacement;
                    }
                    replacement.left = this;
                    this.parent = replacement;
                }
                else
                {
                    replacement = this.left;
                    this.left = replacement.right;
                    if (replacement.right != null)
                        replacement.right.parent = this;
                    replacement.parent = this.parent;
                    if (this.parent != null)
                    {
                        if (this.parent.right == this)
                            this.parent.right = replacement;
                        else
                            this.parent.left = replacement;
                    }
                    replacement.right = this;
                    this.parent = replacement;
                }
            }

            #endregion

            #region [ public methods ]
            
            /// <summary>
            /// inserts target node, and restructures the tree
            /// </summary>
            /// <param name="entry">node to be inserted</param>
            /// <remarks>
            /// - being a method, the tree must be non-empty
            /// </remarks>
            public void insert(node entry)
            {
                // standard insert:
                if (this.key.CompareTo(entry.key) < 0)
                {
                    if (this.right == null)
                        this.right = entry;
                    else
                        this.right.insert(entry);
                }
                else
                {
                    if (this.left == null)
                        this.left = entry;
                    else
                        this.left.insert(entry);
                }
                // RB restructuring:
                entry.red = true;
                node temp, temp2;
                temp = entry;
                while (temp.parent.red)
                {
                    if (temp.parent == temp.parent.parent.left)
                    {
                        temp2 = temp.parent.parent.right;
                        if (temp2.red)
                        {
                            temp.parent.red = false;
                            temp2.red = false;
                            temp.parent.parent.red = true;
                            temp = temp.parent.parent;

                        }
                        else if (temp == temp.parent.right) // not done


                    }
                }
            }

            /// <summary>
            /// finds the node with the matching key
            /// </summary>
            /// <param name="key">the key to be matched</param>
            /// <returns>the entire node</returns>
            /// <exception cref="key not found">is thrown if key is not present</exception>
            public node search(K key) 
            {
                node target;

                if (this.key.CompareTo(key) == 0)
                    return this;
                else if (this.key.CompareTo(key) < 0)
                    target = this.right;
                else
                    target = this.left;

                if (target == null)
                    throw new Exception("key not found");
                else
                    return target.search(key);  
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
