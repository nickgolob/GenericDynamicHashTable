using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTable
{
    public class RedBlackTree<K, V> where K : IComparable
    {
        #region [ internal declarations ]

        public class Node
        {
            #region [ attributes ]

            public K key;
            public V value;
            public Node left;      // reference to left child
            public Node right;     // reference to right child
            public Node parent;    // reference to parent node
            public Boolean red;    // color for red-black components

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

            #region [ methods ]

            public void quickSet()
            {
                this.left = null;
                this.right = null;
                this.parent = null;
                this.red = false;
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
        private void rotate(Node fulcrum, bool left)
        {
            Node replacement;
            if (left)
            {
                replacement = fulcrum.right;
                fulcrum.right = replacement.left;
                if (replacement.left != null)
                    replacement.left.parent = fulcrum;
                replacement.parent = fulcrum.parent;
                if (fulcrum.parent != this.root)
                {
                    if (fulcrum.parent.left == fulcrum)
                        fulcrum.parent.left = replacement;
                    else
                        fulcrum.parent.right = replacement;
                }
                else
                {
                    this.root = replacement;
                }
                replacement.left = fulcrum;
                fulcrum.parent = replacement;
            }
            else
            {
                replacement = fulcrum.left;
                fulcrum.left = replacement.right;
                if (replacement.right != null)
                    replacement.right.parent = fulcrum;
                replacement.parent = fulcrum.parent;
                if (fulcrum.parent != this.root)
                {
                    if (fulcrum.parent.right == fulcrum)
                        fulcrum.parent.right = replacement;
                    else
                        fulcrum.parent.left = replacement;
                }
                else
                {
                    this.root = replacement;
                }
                replacement.right = fulcrum;
                fulcrum.parent = replacement;
            }
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

        #endregion

        #region [ public methods ]

        /// <summary>
        /// inserts target node and restructures the tree
        /// </summary>
        /// <param name="entry">node to be inserted</param>
        public void insert(Node entry)
        {
            Node x, y;
            entry.quickSet();

            #region [ BST insert ]
            for (x = this.root, y = null;
                x != null;
                y = x, x = (x.key.CompareTo(entry.key) < 0 ? x.right : x.left)) ;
            entry.parent = y;
            if (y == null)
                this.root = entry;
            else if (y.key.CompareTo(entry.key) < 0)
                y.right = entry;
            else
                y.left = entry;
            entry.red = true;
            #endregion

            #region [ RB restructure ]
            x = entry;
            while ((x != this.root) && (x.parent.red))
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
                            this.rotate(x, true);
                        }
                        x.parent.red = false;
                        x.parent.parent.red = true;
                        this.rotate(x.parent.parent, false);
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
                            this.rotate(x, false);
                        }
                        x.parent.red = false;
                        x.parent.parent.red = true;
                        this.rotate(x.parent.parent, true);
                    }
                }
            }
            this.root.red = false;
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

                #region [BST delete]
                y = target;
                bool origColor = y.red;
                if (target.left == null)
                {
                    x = target.right;
                    this.transplant(target, target.right);
                }
                else if (target.right == null)
                {
                    x = target.left;
                    this.transplant(target, target.left);
                }
                else
                {
                    y = this.getMin(target.right);
                    origColor = y.red;
                    x = y.right;
                    if (y.parent == target)
                    {
                        x.parent = y;
                    }
                    else
                    {
                        this.transplant(y, y.right);
                        y.right = target.right;
                        y.right.parent = y;
                    }
                    this.transplant(target, y);
                    y.left = target.left;
                    y.left.parent = y;
                    y.red = target.red;
                }
                #endregion
                
                #region [ RB restructure ]
                if (!origColor)
                {
                    while ((x != this.root) && (!x.red))
                    {
                        if (x == x.parent.left)
                        {
                            y = x.parent.right;
                            if (y.red)
                            {
                                y.red = false;
                                x.parent.red = true;
                                this.rotate(x.parent, true);
                                y = x.parent.right;
                            }
                            if ((!y.left.red) && (!y.right.red))
                            {
                                y.red = true;
                                x = x.parent;
                            }
                            else
                            {
                                if (!y.right.red)
                                {
                                    y.left.red = false;
                                    y.red = true;
                                    this.rotate(y, false);
                                    y = x.parent.right;
                                }
                                y.red = x.parent.red;
                                x.parent.red = false;
                                y.right.red = false;
                                this.rotate(x.parent, true);
                                x = this.root;
                            }
                        }
                        else
                        {
                            y = x.parent.left;
                            if (y.red)
                            {
                                y.red = false;
                                x.parent.red = true;
                                this.rotate(x.parent, false);
                                y = x.parent.left;
                            }
                            if ((!y.right.red) && (!y.left.red))
                            {
                                y.red = true;
                                x = x.parent;
                            }
                            else
                            {
                                if (!y.left.red)
                                {
                                    y.right.red = false;
                                    y.red = true;
                                    this.rotate(y, true);
                                    y = x.parent.left;
                                }
                                y.red = x.parent.red;
                                x.parent.red = false;
                                y.left.red = false;
                                this.rotate(x.parent, false);
                                x = this.root;
                            }
                        }
                    }
                    x.red = false;
                }
                #endregion
            }
        public void delete(K key)
        {
            this.remove(this.search(key));
        }

        #endregion
    }
}
