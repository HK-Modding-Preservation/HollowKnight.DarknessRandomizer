﻿using System;

namespace DarknessRandomizer.Lib
{
    public class Variant<A, B>
    {
        private readonly bool isFirst;
        private readonly A a;
        private readonly B b;

        private Variant(bool isFirst, A a, B b)
        {
            this.isFirst = isFirst;
            this.a = a;
            this.b = b;
        }

        public Variant(A a) : this(true, a, default) { }
        public Variant(B b) : this(false, default, b) { }

        public A First
        {
            get
            {
                if (!isFirst)
                {
                    throw new ArgumentException("Invalid Variant access");
                }

                return a;
            }
        }

        public B Second
        {
            get
            {
                if (isFirst)
                {
                    throw new ArgumentException("Invalid Variant access");
                }

                return b;
            }
        }

        public bool HasFirst => isFirst;
        public bool HasSecond => !isFirst;
    }
}
