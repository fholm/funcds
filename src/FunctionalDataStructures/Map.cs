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
using System.Collections.Generic;

namespace FunctionalDataStructures {
    public sealed class Map<K, V> : IEquatable<Map<K, V>>, IEnumerable<KeyValuePair<K, V>>
        where K : IEquatable<K>, IComparable<K>
        where V : IEquatable<V> {

        static readonly Map<K, V> _empty = new Map<K, V>();

        readonly K _key;
        readonly V _value;
        readonly int _count;
        readonly int _height;
        readonly Map<K, V> _left;
        readonly Map<K, V> _right;

        K Key {
            get {
                if (_count == 0)
                    throw new InvalidOperationException("Map is empty");

                return _key;
            }
        }

        V Value {
            get {
                if (_count == 0)
                    throw new InvalidOperationException("Map is empty");

                return _value;
            }
        }

        Map<K, V> Left {
            get {
                if (_count == 0)
                    throw new InvalidOperationException("Map is empty");

                return _left;
            }
        }

        Map<K, V> Right {
            get {
                if (_count == 0)
                    throw new InvalidOperationException("Map is empty");

                return _right;
            }
        }

        int Balance {
            get {
                if (_count == 0)
                    return 0;

                return Left._height - Right._height;
            }
        }

        public int Count {
            get { return _count; }
        }

        public V this[K key] {
            get {
                return this.Find(key);
            }
        }

        public Map () {
            _key = default(K);
            _value = default(V);
            _count = 0;
            _height = 0;
            _left = default(Map<K, V>);
            _right = default(Map<K, V>);
        }

        Map (K key, V value)
            : this(key, value, _empty, _empty) {
        }

        Map (K key, V value, Map<K, V> left, Map<K, V> right) {
            _key = key;
            _value = value;
            _left = left;
            _right = right;
            _height = Math.Max(left._height, right._height) + 1;
            _count = left.Count + right.Count + 1;
        }

        public Map<K, V> Add (K key, V value) {
            if (_count == 0)
                return new Map<K, V>(key, value, _empty, _empty);

            int cmp = key.CompareTo(this.Key);

            if (cmp < 0) return Rebalance(new Map<K, V>(this.Key, this.Value, this.Left.Add(key, value), this.Right));
            if (cmp > 0) return Rebalance(new Map<K, V>(this.Key, this.Value, this.Left, this.Right.Add(key, value)));

            throw new InvalidOperationException("Key already exists");
        }

        public Map<K, V> Update (K key, V value) {
            if (_count == 0)
                return new Map<K, V>(key, value, _empty, _empty);

            int cmp = key.CompareTo(this._key);

            if (cmp < 0) return Rebalance(new Map<K, V>(this.Key, this.Value, this.Left.Update(key, value), this.Right));
            if (cmp > 0) return Rebalance(new Map<K, V>(this.Key, this.Value, this.Left, this.Right.Update(key, value)));

            return new Map<K, V>(key, value, this.Left, this.Right);
        }

        public Map<K, V> Remove (K key) {
            if (_count == 0)
                throw new KeyNotFoundException();

            int cmp = key.CompareTo(this.Key);
            if (cmp < 0) return Rebalance(new Map<K, V>(this.Key, this.Value, this.Left.Remove(key), this.Right));
            if (cmp > 0) return Rebalance(new Map<K, V>(this.Key, this.Value, this.Left, this.Right.Remove(key)));

            // no right child
            if (this.Right.Count == 0) {
                // no children, return empty
                if (this.Left.Count == 0) {
                    return _empty;
                }

                // only left child, return it
                return Rebalance(this.Left);
            } else {
                // only right child, return it
                if (this.Left.Count == 0) {
                    return Rebalance(this.Right);
                }

                // both left and right child
                Map<K, V> replacement = this.Right;

                // while the replacement has a smaller child
                while (replacement.Left.Count > 0) {
                    replacement = replacement.Left;
                }

                // remove replacement from right subtree
                Map<K, V> right = this.Right.Remove(replacement.Key);

                // create new node with replacement key+value, our old left and the new right tree (without the replacement node)
                return Rebalance(new Map<K, V>(replacement.Key, replacement.Value, this.Left, right));
            }
        }

        public V Find (K key) {
            Map<K, V> map = this.Search(key);

            if (map.Count == 0) {
                throw new KeyNotFoundException();
            }

            return map.Value;
        }

        public bool TryFind (K key, out V value) {
            Map<K, V> map = this.Search(key);

            if (map.Count == 0) {
                value = default(V);
                return false;

            } else {
                value = map.Value;
                return true;
            }
        }

        public bool Equals (Map<K, V> other) {
            Map<K, V> a = this;
            Map<K, V> b = other;

            if (ReferenceEquals(a, b))
                return true;

            if (a.Count != b.Count)
                return false;

            var ae = a.GetEnumerator();
            var be = a.GetEnumerator();

            while (ae.MoveNext() && be.MoveNext()) {
                var ak = ae.Current;
                var bk = be.Current;

                if (ak.Key.Equals(bk.Key) == false)
                    return false;

                if (ak.Value.Equals(bk.Value) == false)
                    return false;
            }

            return true;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator () {
            var stack = new Stack<Map<K, V>>();
            stack.Push(this);

            while (stack.Count > 0) {
                var map = stack.Pop();

                if (map.Count == 0)
                    continue;

                if (map.Left.Count == 0) {
                    yield return new KeyValuePair<K, V>(map.Key, map.Value);
                    stack.Push(map.Right);

                } else {
                    stack.Push(map.Right);
                    stack.Push(new Map<K, V>(map.Key, map.Value));
                    stack.Push(map.Left);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
            return GetEnumerator();
        }

        Map<K, V> Search (K key) {
            if (_count == 0)
                return this;

            int cmp = key.CompareTo(this.Key);
            if (cmp < 0) return this.Left.Search(key);
            if (cmp > 0) return this.Right.Search(key);

            return this;
        }

        static Map<K, V> RotateLeft (Map<K, V> map) {
            return new Map<K, V>(
                // new root
                map.Right.Key,
                map.Right.Value,

                // new left
                new Map<K, V>(map.Key, map.Value, map.Left, map.Right.Left),

                // new right
                map.Right.Right
            );
        }

        static Map<K, V> RotateRight (Map<K, V> map) {
            return new Map<K, V>(
                // new root
                map.Left.Key,
                map.Left.Value,

                // new left
                map.Left.Left,

                // new right
                new Map<K, V>(map.Key, map.Value, map.Left.Right, map.Right)
            );
        }

        static Map<K, V> Rebalance (Map<K, V> map) {
            int b = map.Balance;

            if (b < -2)
                throw new Exception();

            if (b > 2)
                throw new Exception();

            // left
            if (b == 2) {

                // left - right
                if (map.Left.Balance == -1) {
                    map = new Map<K, V>(map.Key, map.Value, RotateLeft(map.Left), map.Right);
                }

                // left - left
                return RotateRight(map);
            }

            // right
            if (b == -2) {

                // right - left
                if (map.Right.Balance == 1) {
                    map = new Map<K, V>(map.Key, map.Value, map.Left, RotateRight(map.Right));
                }

                // right - right
                return RotateLeft(map);
            }


            return map;
        }
    }
}
