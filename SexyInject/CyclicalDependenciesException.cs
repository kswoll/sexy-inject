// <copyright file="CyclicalDependenciesException.cs" company="PlanGrid, Inc.">
//     Copyright (c) 2017 PlanGrid, Inc. All rights reserved.
// </copyright>

using System;

namespace SexyInject
{
    public class CyclicalDependenciesException : Exception
    {
        public CyclicalDependenciesException(string message) : base(message)
        {
        }
    }
}