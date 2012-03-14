using System;
using System.Collections.Generic;

namespace Bespoke.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Node<T> : IEnumerable<T> where T: class
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Node<T> Parent
        {
            get
            {
                return mParent;
            }
            internal set
            {
                mParent = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Node<T> Root
        {
            get
            {
                if (mParent == null)
                {
                    return this;
                }
                else
                {
                    return mParent.Root;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public NodeCollection<T> Children
        {
            get
            {
                return mChildren;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T Data
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public Node(T data)
        {
            mChildren = new NodeCollection<T>(this);
            mData = data;
        }

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsAncestorOf(Node<T> node)
        {
            if (mChildren.Contains(node))
            {
                return true;
            }

            foreach (Node<T> child in mChildren)
            {
                if (child.IsAncestorOf(node))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsDescendantOf(Node<T> node)
        {
            if (mParent == null)
            {
                return false;
            }

            if (node == mParent)
            {
                return true;
            }

            return mParent.IsDescendantOf(node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool SharesHierarchyWith(Node<T> node)
        {
            if (node == this || IsAncestorOf(node) || IsDescendantOf(node))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetDepthFirstEnumerator()
        {
            yield return mData;
            foreach (Node<T> child in mChildren)
            {
                IEnumerator<T> childEnumerator = child.GetDepthFirstEnumerator();
                while (childEnumerator.MoveNext())
                {
                    yield return childEnumerator.Current;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetBreadthFirstEnumerator()
        {
            Queue<Node<T>> queue = new Queue<Node<T>>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                Node<T> node = queue.Dequeue();
                foreach (Node<T> child in node.mChildren)
                {
                    queue.Enqueue(child);
                }

                yield return node.mData;
            }
        }

        /// <summary>
        /// Gets the default enumerator (breadth-first)
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return GetDepthFirstEnumerator();
        }

        /// <summary>
        /// Gets the default enumerator (breadth-first)
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetDepthFirstEnumerator();
        }

        #endregion

        private Node<T> mParent;
        private NodeCollection<T> mChildren;
        private T mData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NodeCollection<T> : IEnumerable<Node<T>> where T : class
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Node<T> Owner
        {
            get
            {
                return mOwner;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return mList.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Node<T> this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public NodeCollection(Node<T> owner)
        {
            Assert.ParamIsNotNull("owner", owner);

            mOwner = owner;
            mList = new List<Node<T>>();
        }

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(Node<T> item)
        {
            if (mOwner.SharesHierarchyWith(item))
            {
                throw new InvalidOperationException("Cannot add a node that is already a member of the hierarchy.");
            }

            mList.Add(item);
            item.Parent = mOwner;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public bool Remove(Node<T> item)
        {
            return mList.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public bool Contains(Node<T> item)
        {
            return mList.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            foreach (Node<T> node in this)
            {
                node.Parent = null;
            }

            mList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Node<T>> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        #endregion

        private Node<T> mOwner;
        private List<Node<T>> mList;
    }
}
