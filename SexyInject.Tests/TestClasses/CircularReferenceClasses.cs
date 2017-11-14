// <copyright file="CircularReferenceClasses.cs" company="PlanGrid, Inc.">
//     Copyright (c) 2017 PlanGrid, Inc. All rights reserved.
// </copyright>
namespace SexyInject.Tests.TestClasses
{
    public class CircularReferenceClasses
    {
        public class A
        {
            public A(B b) { }
        }

        public class B
        {
            public B(C c) { }
        }

        public class C
        {
            public C(A a) { }
        }

        public class D
        {
            public D(D d) { }
        }
    }
}