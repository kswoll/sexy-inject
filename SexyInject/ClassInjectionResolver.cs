﻿using System;

namespace SexyInject
{
    public class ClassInjectionResolver : IResolver
    {
        private readonly Func<ResolveContext, Type, object> lambda;


        public bool TryResolve(ResolveContext context, Type targetType, out object result)
        {
            throw new NotImplementedException();
        }
    }
}