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
    public static partial class Enumerable {
        public static Map<K, V> ToMap<K, V> (this IEnumerable<KeyValuePair<K, V>> enumerable)
            where K : IEquatable<K>, IComparable<K>
            where V : IEquatable<V> {
            Map<K, V> map = new Map<K, V>();

            foreach (KeyValuePair<K, V> kvp in enumerable) {
                map = map.Update(kvp.Key, kvp.Value);
            }

            return map;
        }

        public static Map<K, V> ToMap<K, V> (this IEnumerable<V> enumerable, Func<V, K> keySelector)
            where K : IEquatable<K>, IComparable<K>
            where V : IEquatable<V> {
            Map<K, V> map = new Map<K, V>();

            foreach (V val in enumerable) {
                map = map.Update(keySelector(val), val);
            }

            return map;
        }

        public static LinkedList<T> ToLinkedList<T> (this IEnumerable<T> enumerable) where T : IEquatable<T> {
            LinkedList<T> list = new LinkedList<T>();

            foreach (T value in enumerable) {
                list = list.Cons(value);
            }

            return list;
        }
    }
}
