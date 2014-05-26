﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTable
{
    public class RedBlackTree<K, V> where K : IComparable
    {
        #region [ internal declarations ]

        public const bool RED = true, BLACK = false, LEFT = true, RIGHT = false;

        public class Node
        {
            #region [ attributes ]

            public K key;
            public V value;
            public Node left;      // reference to left child
            public Node right;     // reference to right child
            public Node parent;    // reference to parent node
            public Boolean color;    // color for red-black components

            #endregion

            #region [ constructors ]

            public Node(K key, V value)
            {
                this.key = key;
                this.value = value;
                this.left = null;
                this.right = null;
                this.parent = null;
                this.color = BLACK;
            }

            #endregion

            #region [ methods ]

            public void quickSet()
            {
                this.left = null;
                this.right = null;
                this.parent = null;
                this.color = BLACK;
            }

            #endregion
        }

        #endregion


        #region [ attributes ]

        private Node root;

        #endregion

        #region [ constructors ]

        public RedBlackTree()
        {
            this.root = null;
        }

        #endregion

        #region [ private utilities ]

        /// <summary>
        /// gets the minimum element of a specified subtree
        /// </summary>
        /// <returns>the left-most element</returns>
        private Node getMin(Node subTree)
        {
            Node x = subTree;
            while (x.left != null)
                x = x.left;
            return x;
        }

        /// <summary>
        /// performs a rotation about the target node.
        /// </summary>
        /// <param name="left"> denotes type of rotation </param>
        /// <remark> 
        /// - left rotations assume right child is not null
        /// - right rotations assume left child is not null
        ///</remark>
        private void rotate(Node fulcrum, bool direction)
        {
            Node replacement, temp;
            if (direction == LEFT)
            {
                replacement = fulcrum.right;
                temp = replacement.left;
                replacement.left = fulcrum;
                fulcrum.right = temp;
            }
            else
            {
                replacement = fulcrum.left;
                temp = replacement.right;
                replacement.right = fulcrum;
                fulcrum.left = temp;
            }
            if (temp != null)
                temp.parent = fulcrum;
            if (fulcrum == this.root)
                this.root = replacement;
            else if (fulcrum.parent.left == fulcrum)
                fulcrum.parent.left = replacement;
            else
                fulcrum.parent.right = replacement;
            replacement.parent = fulcrum.parent;
            fulcrum.parent = replacement;
        }

        /// <summary>
        /// replaces the subtree rooted at target with the subtree rooted at replacement
        /// </summary>
        private void transplant(Node target, Node replacement)
        {
            if (target == this.root)
                this.root = replacement;
            else if (target == target.parent.left)
                target.parent.left = replacement;
            else
                target.parent.right = replacement;
            if (replacement != null)
                replacement.parent = target.parent;
        }

        /// <summary>
        /// get grandparent of a node
        /// </summary>
        /// <param name="x">node to find grandparent of</param>
        /// <returns>the grandparent if exists, null otherwise</returns>
        private Node grandparent(Node x)
        {
            if ((x != null) && (x.parent != null))
                return x.parent.parent;
            else
                return null;
        }

        /// <summary>
        /// get uncle of a node
        /// </summary>
        /// <param name="x">node to find uncle of</param>
        /// <returns>the uncle if exists, null otherwise</returns>
        private Node uncle(Node x)
        {
            Node grandparent = this.grandparent(x);
            if (grandparent == null)
                return null;
            else if (x.parent == grandparent.left)
                return grandparent.right;
            else
                return grandparent.left;
        }

        /// <summary>
        /// get sibling of a node
        /// </summary>
        /// <param name="x">node to find sibling of</param>
        /// <returns>the sibling if exists, null otherwise</returns>
        private Node sibling(Node x)
        {
            if (x == this.root)
                return null;
            else if (x == x.parent.left)
                return x.parent.right;
            else
                return x.parent.left;
        }

        #endregion

        #region [ public methods ]

        /// <summary>
        /// inserts target node and restructures the tree
        /// </summary>
        /// <param name="entry">node to be inserted</param>
        public void insert(Node entry)
        {
            Node x, y = null;
            entry.quickSet();

            #region [ BST insert ]
            x = this.root;
            while (x != null) {
                y = x;
                if (x.key.CompareTo(entry.key) < 0)
                    x = x.right;
                else
                    x = x.left;
            }
            entry.parent = y;
            if (y == null)
                this.root = entry;
            else if (y.key.CompareTo(entry.key) < 0)
                y.right = entry;
            else
                y.left = entry;
            entry.color = RED;
            #endregion

            #region [ RB restructure ]
            x = entry;
            while ((x != this.root) && (x.parent.color == RED))
            {
                y = this.uncle(x);
                if ((y != null) && (y.color == RED))
                {
                    x.parent.color = BLACK;
                    y.color = BLACK;
                    x = this.grandparent(x);
                    x.color = RED;
                }
                else
                {
                    y = this.grandparent(x);
                    if ((x == x.parent.right) && (x.parent == y.left))
                    {
                        this.rotate(x.parent, LEFT);
                        x = x.left;
                    }
                    else if ((x == x.parent.left) && (x.parent == y.right))
                    {
                        this.rotate(x.parent, RIGHT);
                        x = x.right;
                    }
                    x.parent.color = BLACK;
                    y.color = RED;
                    if (x == x.parent.left)
                        this.rotate(y, RIGHT);
                    else
                        this.rotate(y, LEFT);
                    x = y.parent;
                }
            }
            this.root.color = BLACK;


            //x = entry;
            //while ((x != this.root) && (x.parent.red))
            //{
            //    if (x.parent == x.parent.parent.left)
            //    {
            //        y = x.parent.parent.right;
            //        if (y.red)
            //        {
            //            x.parent.red = false;
            //            y.red = false;
            //            x.parent.parent.red = true;
            //            x = x.parent.parent;
            //        }
            //        else
            //        {
            //            if (x == x.parent.right)
            //            {
            //                x = x.parent;
            //                this.rotate(x, true);
            //            }
            //            x.parent.red = false;
            //            x.parent.parent.red = true;
            //            this.rotate(x.parent.parent, false);
            //        }
            //    }
            //    else
            //    {
            //        y = x.parent.parent.left;
            //        if (y.red)
            //        {
            //            x.parent.red = false;
            //            y.red = false;
            //            x.parent.parent.red = true;
            //            x = x.parent.parent;
            //        }
            //        else
            //        {
            //            if (x == x.parent.left)
            //            {
            //                x = x.parent;
            //                this.rotate(x, false);
            //            }
            //            x.parent.red = false;
            //            x.parent.parent.red = true;
            //            this.rotate(x.parent.parent, true);
            //        }
            //    }
            //}
            //this.root.red = false;
            #endregion

        }
        public void insert(K key, V value)
        {
            this.insert(new Node(key, value));
        }

        /// <summary>
        /// finds the node with the matching key
        /// </summary>
        /// <param name="key">the key to be matched</param>
        /// <returns>the node with the matching key</returns>
        /// <exception cref="key not found">is thrown if key is not present</exception>
        public Node search(K key)
        {
            Node target;
            for (target = this.root; target != null; )
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
        /// removes the targeted node from tree, and restructures the tree
        /// </summary>
        /// <param name="target">node to be removed (reference required)</param>
        public void remove(Node target)
        {
            Node x, y;





            //#region [BST delete]
            //y = target;
            //bool origColor = y.color;
            //if (target.left == null)
            //{
            //    x = target.right;
            //    this.transplant(target, target.right);
            //}
            //else if (target.right == null)
            //{
            //    x = target.left;
            //    this.transplant(target, target.left);
            //}
            //else
            //{
            //    y = this.getMin(target.right);
            //    origColor = y.color;
            //    x = y.right;
            //    if (y.parent == target)
            //    {
            //        x.parent = y;
            //    }
            //    else
            //    {
            //        this.transplant(y, y.right);
            //        y.right = target.right;
            //        y.right.parent = y;
            //    }
            //    this.transplant(target, y);
            //    y.left = target.left;
            //    y.left.parent = y;
            //    y.color = target.color;
            //}
            //#endregion
                
            //#region [ RB restructure ]
            //if (!origColor)
            //{
            //    while ((x != this.root) && (!x.color))
            //    {
            //        if (x == x.parent.left)
            //        {
            //            y = x.parent.right;
            //            if (y.color)
            //            {
            //                y.color = false;
            //                x.parent.color = true;
            //                this.rotate(x.parent, true);
            //                y = x.parent.right;
            //            }
            //            if ((!y.left.color) && (!y.right.color))
            //            {
            //                y.color = true;
            //                x = x.parent;
            //            }
            //            else
            //            {
            //                if (!y.right.color)
            //                {
            //                    y.left.color = false;
            //                    y.color = true;
            //                    this.rotate(y, false);
            //                    y = x.parent.right;
            //                }
            //                y.color = x.parent.color;
            //                x.parent.color = false;
            //                y.right.color = false;
            //                this.rotate(x.parent, true);
            //                x = this.root;
            //            }
            //        }
            //        else
            //        {
            //            y = x.parent.left;
            //            if (y.color)
            //            {
            //                y.color = false;
            //                x.parent.color = true;
            //                this.rotate(x.parent, false);
            //                y = x.parent.left;
            //            }
            //            if ((!y.right.color) && (!y.left.color))
            //            {
            //                y.color = true;
            //                x = x.parent;
            //            }
            //            else
            //            {
            //                if (!y.left.color)
            //                {
            //                    y.right.color = false;
            //                    y.color = true;
            //                    this.rotate(y, true);
            //                    y = x.parent.left;
            //                }
            //                y.color = x.parent.color;
            //                x.parent.color = false;
            //                y.left.color = false;
            //                this.rotate(x.parent, false);
            //                x = this.root;
            //            }
            //        }
            //    }
            //    x.color = false;
            //}
            //#endregion

        }
        public void delete(K key)
        {
            this.remove(this.search(key));
        }

        #endregion
    }
}
