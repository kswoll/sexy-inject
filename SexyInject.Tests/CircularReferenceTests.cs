// <copyright file="CircularReferenceTests.cs" company="PlanGrid, Inc.">
//     Copyright (c) 2017 PlanGrid, Inc. All rights reserved.
// </copyright>

using System;
using NUnit.Framework;
using SexyInject.Tests.TestClasses;

namespace SexyInject.Tests
{
    [TestFixture]
    public class CircularReferenceTests
    {
        [Test]
        public void CreateA()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();

            try
            {
                registry.Get<CircularReferenceClasses.A>();
                Assert.Fail("Should have thrown CyclicalDependenciesException");
            }
            catch (CyclicalDependenciesException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void CreateD()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();

            try
            {
                registry.Get<CircularReferenceClasses.D>();
                Assert.Fail("Should have thrown CyclicalDependenciesException");
            }
            catch (CyclicalDependenciesException)
            {
                Assert.Pass();
            }
        }
    }
}