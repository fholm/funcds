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

namespace FunctionalDataStructures {
    public abstract class Option<T> : IEquatable<Option<T>> where T : IEquatable<T> {
        public static readonly Option<T> None = new _None();
        public static Option<T> Some (T value) { return new _Some(value); }

        public abstract T Value { get; }
        public abstract bool HasValue { get; }
        public abstract bool Equals (Option<T> other);

        Option () {

        }

        public bool TryGetValue (out T value) {
            if (this.HasValue) {
                value = this.Value;
                return true;
            }

            value = default(T);
            return false;
        }

        class _None : Option<T> {
            public override bool HasValue {
                get { return false; }
            }

            public override T Value {
                get { throw new InvalidOperationException(); }
            }

            public override bool Equals (Option<T> other) {
                return other.HasValue == false;
            }
        }

        class _Some : Option<T> {
            readonly T _value;

            public override bool HasValue {
                get { return true; }
            }

            public override T Value {
                get { return _value; }
            }

            public _Some (T v) {
                this._value = v;
            }

            public override bool Equals (Option<T> other) {
                if (other.HasValue) {
                    return this.Value.Equals(other.Value);
                }

                return false;
            }
        }
    }
}
