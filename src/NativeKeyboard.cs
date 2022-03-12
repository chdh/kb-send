using Exception              = System.Exception;
using IntPtr                 = System.IntPtr;
using Math                   = System.Math;
using Win32Exception         = System.ComponentModel.Win32Exception;
using System.Runtime.InteropServices;

internal class NativeKeyboard {

// Key codes:
public  const int  VK_RETURN          = 0x0D;
public  const int  VK_TAB             = 0x09;
public  const int  VK_SHIFT           = 0x10;
public  const int  VK_CONTROL         = 0x11;
public  const int  VK_MENU            = 0x12;
public  const int  VK_HOME            = 0x24;
public  const int  VK_ESCAPE          = 0x1B;
// Non-standard key code constants:
public  const int  VK_ENTER           = VK_RETURN;
public  const int  VK_ALT             = VK_MENU;

private const uint INPUT_KEYBOARD     = 1;
private const uint KEYEVENTF_KEYUP    = 2;
private const uint KEYEVENTF_UNICODE  = 4;
private const uint MAPVK_VK_TO_VSC_EX = 4;

[StructLayout(LayoutKind.Explicit, Size = 40)]
private struct INPUT64 {
   [FieldOffset(0)]  public uint   Type;
   [FieldOffset(8)]  public ushort VirtualKeyCode;
   [FieldOffset(10)] public ushort ScanCode;
   [FieldOffset(12)] public uint   Flags;
   [FieldOffset(16)] public uint   Time;
   [FieldOffset(24)] public IntPtr ExtraInfo; }

[DllImport("User32.dll", EntryPoint="SendInput", SetLastError=true)]
   private static extern uint SendInput64 (uint nInputs, INPUT64[] pInputs, int cbSize);
[DllImport("User32.dll")]
   private static extern IntPtr GetMessageExtraInfo();
[DllImport("User32.dll")]
   private static extern uint MapVirtualKey (uint uCode, uint uMapType);

private static void SendInput (int n, INPUT64[] pInput) {
   Utils.Assert(IntPtr.Size == 8);                         // 32-bit version not yet implemented
   uint tr = SendInput64((uint)n, pInput, Marshal.SizeOf(typeof(INPUT64)));
   if (tr == 0) {
      throw new Win32Exception(); }
   if (tr != n) {
      throw new Win32Exception("SendInput() did not process all events."); }}

private static void SetInput (ref INPUT64 input, ushort vk, ushort c, bool down) {
   input.Type = INPUT_KEYBOARD;
   input.VirtualKeyCode = vk;
   input.ScanCode = (c != 0) ? c : (ushort)MapVirtualKey(vk, MAPVK_VK_TO_VSC_EX);
   input.Flags = (down ? 0 : KEYEVENTF_KEYUP) | (vk == 0 ? KEYEVENTF_UNICODE : 0);
   input.Time = 0;
   input.ExtraInfo = GetMessageExtraInfo(); }

public static void SendChar (ushort c) {
   var buf = new INPUT64[2];
   SetInput(ref buf[0], 0, c, true);
   SetInput(ref buf[1], 0, c, false);
   SendInput(2, buf); }

public static void SendChars (string s) {
   if (s.Length == 0) {
      return; }
   const int maxBlockSize = 128;
   var buf = new INPUT64[2 * Math.Min(s.Length, maxBlockSize)];
   int p = 0;
   while (p < s.Length) {
      int p0 = p;
      while (p < s.Length && p - p0 < maxBlockSize) {
         SetInput(ref buf[2 * (p - p0)    ], 0, s[p], true);
         SetInput(ref buf[2 * (p - p0) + 1], 0, s[p], false);
         p++; }
      SendInput(2 * (p - p0), buf); }}

public static void SendKey (ushort vk, bool down) {
   INPUT64[] pInput = new INPUT64[1];
   SetInput(ref pInput[0], vk, 0, down);
   SendInput(1, pInput); }

public static void SendKey (ushort vk) {
   INPUT64[] pInput = new INPUT64[2];
   SetInput(ref pInput[0], vk, 0, true);
   SetInput(ref pInput[1], vk, 0, false);
   SendInput(2, pInput); }

}
