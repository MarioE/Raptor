using System.Runtime.InteropServices;

namespace Raptor
{
    internal static class NativeMethods
    {
        [DllImport("kernel32", BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern bool CreateSymbolicLink([MarshalAs(UnmanagedType.LPStr)] string lpSymlinkFileName,
            [MarshalAs(UnmanagedType.LPStr)] string lpTargetFileName, int dwFlags);
    }
}
