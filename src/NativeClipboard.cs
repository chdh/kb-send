using Encoding               = System.Text.Encoding;
using IntPtr                 = System.IntPtr;
using Win32Exception         = System.ComponentModel.Win32Exception;
using System.Runtime.InteropServices;

internal class NativeClipboard {

[DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)]
   private static extern bool OpenClipboard(IntPtr hWndNewOwner);
[DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)]
   private static extern bool CloseClipboard();
[DllImport("User32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)]
   private static extern bool IsClipboardFormatAvailable(uint format);
[DllImport("User32.dll", SetLastError = true)]
   private static extern IntPtr GetClipboardData(uint uFormat);
[DllImport("kernel32.dll", SetLastError = true)]
   private static extern IntPtr GlobalLock(IntPtr hMem);
[DllImport("kernel32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)]
   private static extern bool GlobalUnlock(IntPtr hMem);
[DllImport("Kernel32.dll", SetLastError = true)]
   private static extern int GlobalSize(IntPtr hMem);

public static string getText() {
   const uint CF_UNICODETEXT = 13;
   bool clipboardIsOpen = false;
   IntPtr objHandle = default;
   IntPtr objPtr = default;
   try {
      if (!IsClipboardFormatAvailable(CF_UNICODETEXT)) {
         throw new Win32Exception(); }
      if (!OpenClipboard(default)) {
         throw new Win32Exception(); }
      clipboardIsOpen = true;
      objHandle = GetClipboardData(CF_UNICODETEXT);
      if (objHandle == default) {
         throw new Win32Exception(); }
      objPtr = GlobalLock(objHandle);
      if (objPtr == default) {
         throw new Win32Exception(); }
      var objSize = GlobalSize(objHandle);
      if (objSize == 0) {
         throw new Win32Exception(); }
      var buf = new byte[objSize];
      Marshal.Copy(objPtr, buf, 0, objSize);
      var len = objSize;
      if (len >= 2 && buf[len - 1] == 0 && buf[len - 2] == 0) {
         len -= 2; }
      return Encoding.Unicode.GetString(buf, 0, len); }
    finally {
      if (objPtr != default) {
         GlobalUnlock(objPtr); }
      if (clipboardIsOpen) {
         CloseClipboard(); }}}

}
