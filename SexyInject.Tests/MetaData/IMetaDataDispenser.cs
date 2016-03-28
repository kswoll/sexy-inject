using System;
using System.Runtime.InteropServices;

namespace SexyInject.Tests.MetaData
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("809c652e-7396-11d2-9771-00a0c9b4d50c")]
    [ComImport]
    public interface IMetaDataDispenser
    {
        uint DefineScope([In] ref Guid rclsid, [In] uint dwCreateFlags, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

        uint OpenScope([MarshalAs(UnmanagedType.LPWStr), In] string szScope, [In] int dwOpenFlags, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppIUnk);

        uint OpenScopeOnMemory([In] IntPtr pData, [In] uint cbData, [In] uint dwOpenFlags, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);
    }
}