/*
* The MIT License (MIT)
* 
* Copyright (c) 2012-2014 Fredrik Holmstrom (fredrik.johan.holmstrom@gmail.com)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace FunctionalDataStructures {
    public class LinkedList<T> : IEnumerable<T>, IEquatable<LinkedList<T>> where T : IEquatable<T> {
        readonly T _head;
        readonly int _count;
        readonly LinkedList<T> _tail;

        public int Count {
            get {
                return this._count;
            }
        }

        public T Head {
            get {
                if (this._count == 0)
                    throw new InvalidOperationException("List is empty");

                return this._head;
            }
        }

        public LinkedList<T> Tail {
            get {
                if (this._count == 0)
                    throw new InvalidOperationException("List is empty");

                return this._tail;
            }
        }

        LinkedList (T head, LinkedList<T> tail) {
            this._head = head;
            this._tail = tail;
            this._count = tail.Count + 1;
        }

        public LinkedList () {
            this._count = 0;
            this._head = default(T);
            this._tail = default(LinkedList<T>);
        }

        public LinkedList<T> Cons (T head) {
            return new LinkedList<T>(head, this);
        }

        public void Split (out T head, out LinkedList<T> tail) {
            head = this.Head;
            tail = this.Tail;
        }

        public bool TrySplit (out T head, out LinkedList<T> tail) {
            if (_count == 0) {
                head = default(T);
                tail = default(LinkedList<T>);
                return false;
            }

            head = this.Head;
            tail = this.Tail;
            return true;
        }

        public IEnumerator<T> GetEnumerator () {
            LinkedList<T> list = this;

            while (list.Count > 0) {
                yield return list.Head;
                list = list.Tail;
            }
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator();
        }

        public bool Equals (LinkedList<T> other) {
            LinkedList<T> a = this;
            LinkedList<T> b = other;

            if (ReferenceEquals(a, b))
                return true;

            if (a.Count != b.Count)
                return false;

            while (a.Count > 0 && b.Count > 0) {
                if (a.Head.Equals(b.Head) == false)
                    return false;

                a = a.Tail;
                b = b.Tail;
            }

            return a.Count == 0 && b.Count == 0;
        }
    }
}
