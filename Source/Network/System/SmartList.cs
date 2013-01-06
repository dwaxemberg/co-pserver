using System;

namespace System.Collections.Generic
{
    public class SmartQueue<T>
    {
        private class ParentAndChild
        {
            //This is a parent
            public T Value;
            public ParentAndChild Child;
            public ParentAndChild Parent;
        }

        private ParentAndChild FirstParent;

        private ParentAndChild LastChild;

        public int Count
        {
            get;
            set;
        }

        public void Enqueue(T value)
        {
            if (FirstParent == null)
            {
                FirstParent = new SmartQueue<T>.ParentAndChild();
                FirstParent.Value = value;
                FirstParent.Child = null;
            }
            else if (FirstParent.Child == null)
            {
                FirstParent.Child = new SmartQueue<T>.ParentAndChild();
                FirstParent.Child.Value = value;
                FirstParent.Child.Child = null;
                FirstParent.Child.Parent = FirstParent.Child;
                LastChild = FirstParent.Child;
            }
            else
            {
                LastChild.Child = new SmartQueue<T>.ParentAndChild();
                LastChild.Child.Value = value;
                LastChild.Child.Child = null;
                LastChild.Child.Parent = LastChild;
                LastChild = LastChild.Child;
            }
            Count++;
        }
        public T Dequeue()
        {
            if (!FirstParent.Value.Equals(default(T)))
            {
                var ret = FirstParent.Value;
                if (FirstParent.Child != null)
                {
                    FirstParent = FirstParent.Child;
                    FirstParent.Parent = null;
                }
                Count--;
                return ret;
            }
            return default(T);
        }
        public void Remove(T value)
        {
            var Child = FindChildWithValue(value);
            if (Child != null)
            {
                var childschild = Child.Child;
                if (childschild == null)
                    Child.Parent.Child = null;
                else
                {
                    childschild.Parent = Child.Parent;
                    Child.Parent.Child = childschild;
                }
                Count--;
            }
        }
        public bool Contains(T value)
        {
            ParentAndChild p = FirstParent;

            if (p == null)
                return false;

            if (p.Value.Equals(value))
                return true;

            while (p.Child != null)
            {
                p = p.Child;
                if (p.Value.Equals(value))
                    return true;
            }
            return false;
        }
        private ParentAndChild FindChildWithValue(T value)
        {
            ParentAndChild p = FirstParent;

            if (p.Value.Equals(value))
                return p;

            while (p.Child != null)
            {
                p = p.Child;
                if (p.Value.Equals(value))
                    return p;
            }
            return null;
        }
        public void Clear()
        {
            Count = 0;
            FirstParent = null;
        }
    }
}