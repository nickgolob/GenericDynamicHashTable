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

        private class Node // red black tree sub-root
        {
            #region [ attributes ]

            private K key;
            private V value;
            private Node left;      // reference to left child
            private Node right;     // reference to right child
            private Node parent;    // reference to parent node
            private Boolean red;    // color for red-black components

            #endregion

            #region [ constructors ]

            public Node(K key, V value)
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
            /// quickly resets internal attributes
            /// </summary>
            private void quickSet()
            {
                this.left = null;
                this.right = null;
                this.parent = null;
                this.red = false;
            }

            /// <summary>
            /// hooks up a node as the left or right child of the subject node.
            /// </summary>
            /// <param name="newChild"> the new node to be connected </param>
            /// <param name="left"> denotes wether newChild is to be left or right child </param>
            /// <remark> this accepts null parameters as newChild </remark>
            private void quickLink(Node newChild, bool left)
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

            private Node getMin()
            {
                Node x = this;
                while (x.left != null)
                    x = x.left;
                return x;
            }

            /// <summary>
            /// disconnect minimim node (leaf) from subtree
            /// </summary>
            /// <returns>
            /// returns the disconnected node
            /// </returns>
            /// <remarks>
            /// This now iterates instead of using tail recursion
            /// </remarks>
            private Node popMin()
            {
                Node temp;
                for (temp = this; temp.left != null; temp = temp.left) ;
                temp.parent.quickLink(temp.right, true);
                return temp;
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
                Node replacement;
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

            /// <summary>
            /// replaces the subtree rooted at target with the subtree rooted at replacement
            /// </summary>
            /// <returns>
            /// The root of the tree
            /// </returns>
            private static Node transplant(Node root, Node target, Node replacement)
            {
                if (target == root)
                    return replacement;
                else if (target == target.parent.left)
                    target.parent.left = replacement;
                else
                    target.parent.right = replacement;
                if (replacement != null)
                    replacement.parent = target.parent;
                return root;
            }

            #endregion

            #region [ public methods ]
            
            /// <summary>
            /// inserts target node into the specified tree, and restructures the tree
            /// </summary>
            /// <param name="root">root of tree</param>
            /// <param name="entry">node to be inserted</param>
            /// <returns> root of tree </returns>
            public static Node insert(Node root, Node entry)
            {
                entry.quickSet();
                Node x, y;

                #region [ BST insert ]
                for (x = root, y = null; 
                    x != null; 
                    y = x, x = (x.key.CompareTo(entry.key) < 0 ? x.right : x.left));
                entry.parent = y;
                if (y == null)
                    return entry;
                else if (y.key.CompareTo(entry.key) < 0)
                    y.right = entry;
                else
                    y.left = entry;
                entry.red = true;
                #endregion

                #region [ RB restructure ]
                x = entry;
                while (x.parent.red)
                {
                    if (x.parent == x.parent.parent.left)
                    {
                        y = x.parent.parent.right;
                        if (y.red)
                        {
                            x.parent.red = false;
                            y.red = false;
                            x.parent.parent.red = true;
                            x = x.parent.parent;
                        }
                        else
                        {
                            if (x == x.parent.right)
                            {
                                x = x.parent;
                                x.rotate(true);
                            }
                            x.parent.red = false;
                            x.parent.parent.red = true;
                            x.parent.parent.rotate(false);
                        }
                    }
                    else
                    {
                        y = x.parent.parent.left;
                        if (y.red)
                        {
                            x.parent.red = false;
                            y.red = false;
                            x.parent.parent.red = true;
                            x = x.parent.parent;
                        }
                        else
                        {
                            if (x == x.parent.left)
                            {
                                x = x.parent;
                                x.rotate(false);
                            }
                            x.parent.red = false;
                            x.parent.parent.red = true;
                            x.parent.parent.rotate(true);
                        }
                    }
                }
                #endregion

                return root;
            }

            /// <summary>
            /// finds the node with the matching key
            /// </summary>
            /// <param name="root">root of tree</param>
            /// <param name="key">the key to be matched</param>
            /// <returns>the entire node</returns>
            /// <exception cref="key not found">is thrown if key is not present</exception>
            public static Node search(Node root, K key) 
            {
                Node target;
                for (target = root; target != null; )
                {
                    if (target.key.CompareTo(key) == 0)
                        return target;
                    else if (target.key.CompareTo(key) < 0)
                        target = target.right;
                    else
                        target = target.left;
                }
                throw new Exception("key not found");
            }

            /// <summary>
            /// removed the targeted node, and restructures the tree
            /// </summary>
            /// <param name="root">root of tree</param>
            /// <param name="target">node to be removed (reference required)</param>
            /// <returns>
            /// the (possibly new) root of the tree
            /// </returns>
            /// <remarks>
            /// </remarks>
            public static Node remove(Node root, Node target)
            {
                Node x, y;

                // delete
                y = target;
                bool origColor = y.red;
                if (target.left == null)
                {
                    x = target.right;
                    root = Node.transplant(root, target, target.right);
                }
                else if (target.right == null)
                {
                    x = target.left;
                    root = Node.transplant(root, target, target.left);
                }
                else
                {
                    y = target.right.getMin();
                    origColor = y.red;
                    x = y.right;
                    if (y.parent == target)
                    {
                        x.parent = y;
                    }
                    else
                    {
                        root = Node.transplant(root, y, y.right);
                        y.right = target.right;
                        y.right.parent = y;
                    }
                    root = Node.transplant(root, target, y);
                    y.left = target.left;
                    y.left.parent = y;
                    y.red = target.red;
                }

                // restructure
                if (!origColor)
                {
                    while ((x != root) && (!x.red))
                    {
                        if (x == x.parent.left)
                        {
                            y = x.parent.right;
                            if (y.red)
                            {
                                y.red = false;
                                x.parent.red = true;
                                x.parent.rotate(true);
                            }
                        }
                    }
                }
                    

                return root;

            }

            #endregion
        }


        private const int DEFAULT_INITIAL_SIZE = 1;
        private const float DEFAULT_MAX_LOAD = 2.0F;


        #endregion

        #region [ private attributes ]

        private int entries;
        private int maxSize;
        private Node[] table;   
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
            this.table = new Node[maxSize];
            this.map = map;
            this.loadFactor = loadFactor;
            this.duplicateKeys = duplicateKeys;
        }

        #endregion

        #region [ private utilities ]

        private void expand()
        {
            this.maxSize *= 2;
            Node[] newTable = new Node[maxSize];
            
        }

        #endregion

        #region [ public methods ]

        public void insert(K key, V value)
        {
            
        }

        public void search(K key)
        {
            
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

        }
    }
}
