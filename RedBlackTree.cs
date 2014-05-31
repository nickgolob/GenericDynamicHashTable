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
        public const bool RED = true, BLACK = false, LEFT = true, RIGHT = false;

        public class Node
        {
            #region [ attributes ]
            public K key;
            public V value;
            public Node left, right, parent;
            public Boolean color;
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
            public Node grandparent()
            {
                if ((this != null) && (this.parent != null))
                    return this.parent.parent;
                else
                    return null;
            }
            public Node uncle()
            {
                Node grandparent = this.grandparent();
                if (grandparent == null)
                    return null;
                else if (this.parent == grandparent.left)
                    return grandparent.right;
                else
                    return grandparent.left;
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
        /// two nodes trade places in the tree, and are recolored to
        /// each others colors (to preserve structure).
        /// </summary>
        private void trade(Node target, Node replacement)
        {
            // trade colors
            bool tempColor = target.color;
            target.color = replacement.color;
            replacement.color = tempColor;

            // trade left childs
            Node temp = target.left;
            target.left = replacement.left;
            replacement.left = temp;
            if (replacement.left != null)
                replacement.left.parent = replacement;
            if (target.left != null)
                target.left.parent = target;

            // trade right childs
            temp = target.right;
            target.right = replacement.right;
            replacement.right = temp;
            if (replacement.right != null)
                replacement.right.parent = replacement;
            if (target.right != null)
                target.right.parent = target;

            // trade parents
            temp = target.parent;
            target.parent = replacement.parent;
            replacement.parent = temp;
            if (replacement.parent == null)
                this.root = replacement;
            else if (replacement.parent.left == target)
                replacement.parent.left = replacement;
            else
                replacement.parent.right = replacement;
            if (target.parent == null)
                this.root = target;
            else if (target.parent.left == replacement)
                target.parent.left = target;
            else
                target.parent.right = target;
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
                y = x.uncle();
                if ((y != null) && (y.color == RED))
                {
                    x.parent.color = BLACK;
                    y.color = BLACK;
                    x = x.grandparent();
                    x.color = RED;
                }
                else
                {
                    y = x.grandparent();
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
        public bool exists(K key)
        {
            try
            {
                this.search(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// removes the targeted node from tree, and restructures the tree
        /// </summary>
        /// <param name="target">node to be removed (reference required)</param>
        public void remove(Node target)
        {
            Node x, y, z;
            bool side = false;

            #region [ BST deletion ]
            // make target have only one child
            if ((target.left != null) && (target.right != null))
            {
                this.trade(target, this.getMin(target.right));
                x = target.right;
            }
            else if (target.right != null)
                x = target.right;
            else
                x = target.left;
            // replace with (possible null child)
            y = target.parent;
            if (y == null)
                this.root = x;
            else
            {
                if (target == y.left)
                    side = LEFT;
                else
                    side = RIGHT;
                this.transplant(target, x);
            }
            #endregion

            #region [ RB restructure ]
            if (target.color == BLACK)
            { // restructuring required
                while ((x != this.root) && ((x == null) || (x.color == BLACK)))
                {
                    if (side == LEFT) // x is the left child
                    {
                        z = y.right;
                        if ((z != null) && (z.color == RED))
                        {
                            z.color = BLACK;
                            y.color = RED;
                            this.rotate(y, LEFT);
                            z = y.right;
                        }
                        if (((z.left == null) || (z.left.color == BLACK)) &&
                            ((z.right == null) || (z.right.color == BLACK)))
                        {
                            z.color = RED;
                            x = y;
                        }
                        else
                        {
                            if ((z.right == null) || (z.right.color == BLACK))
                            {
                                z.left.color = BLACK;
                                z.color = RED;
                                this.rotate(z, RIGHT);
                                z = y.right;
                            }
                            z.color = y.color;
                            y.color = BLACK;
                            z.right.color = BLACK;
                            this.rotate(y, LEFT);
                            x = this.root;
                        }
                    }
                    else // rights and lefts reversed:
                    {
                        z = y.left;
                        if ((z != null) && (z.color == RED))
                        {
                            z.color = BLACK;
                            y.color = RED;
                            this.rotate(y, RIGHT);
                            z = y.left;
                        }
                        if (((z.right == null) || (z.right.color == BLACK)) &&
                            ((z.left == null) || (z.left.color == BLACK)))
                        {
                            z.color = RED;
                            x = y;
                        }
                        else
                        {
                            if ((z.left == null) || (z.left.color == BLACK))
                            {
                                z.right.color = BLACK;
                                z.color = RED;
                                this.rotate(z, LEFT);
                                z = y.left;
                            }
                            z.color = y.color;
                            y.color = BLACK;
                            z.left.color = BLACK;
                            this.rotate(y, RIGHT);
                            x = this.root;
                        }
                    }
                }
                x.color = BLACK;
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
