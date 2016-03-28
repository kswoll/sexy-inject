using System;
using System.Reflection;

namespace SexyInject.Emit
{
    public interface ITokenResolver
    {
        MethodBase AsMethod(int token);
        FieldInfo AsField(int token);
        Type AsType(int token);
        string AsString(int token);
        MemberInfo AsMember(int token);
        byte[] AsSignature(int token);
    }

    public class ModuleScopeTokenResolver : ITokenResolver
    {
        private readonly Type[] methodContext;
        private readonly Module module;
        private readonly Type[] typeContext;

        public ModuleScopeTokenResolver(MethodBase method)
        {
            module = method.Module;
            methodContext = method is ConstructorInfo ? null : method.GetGenericArguments();
            typeContext = method.DeclaringType?.GetGenericArguments();
        }

        public MethodBase AsMethod(int token)
        {
            return module.ResolveMethod(token, typeContext, methodContext);
        }

        public FieldInfo AsField(int token)
        {
            return module.ResolveField(token, typeContext, methodContext);
        }

        public Type AsType(int token)
        {
            return module.ResolveType(token, typeContext, methodContext);
        }

        public MemberInfo AsMember(int token)
        {
            return module.ResolveMember(token, typeContext, methodContext);
        }

        public string AsString(int token)
        {
            return module.ResolveString(token);
        }

        public byte[] AsSignature(int token)
        {
            return module.ResolveSignature(token);
        }
    }
}