using Exception          = System.Exception;
using IntPtr             = System.IntPtr;
using Process            = System.Diagnostics.Process;
using StringBuilder      = System.Text.StringBuilder;
using StringComparison   = System.StringComparison;
using System.Runtime.InteropServices;

internal class WinUtils {

private const uint           GW_HWNDNEXT = 2;
private const int            GWL_STYLE = -16;
private const int            WS_VISIBLE = 0x10000000;

[DllImport("user32.dll")]
   private static extern IntPtr GetTopWindow (IntPtr hWnd);
[DllImport("user32.dll", SetLastError=true)]
   private static extern IntPtr GetWindow (IntPtr hWnd, uint uCmd);
[DllImport("user32.dll", SetLastError=true)]
   private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
[DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
   private static extern int GetWindowTextLength (IntPtr hWnd);
[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
   private static extern int GetWindowText (IntPtr hWnd, StringBuilder lpString, int nMaxCount);
[DllImport("User32.dll")]
   private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

// Public extern functions.
[DllImport("user32.dll")]
   public static extern IntPtr GetForegroundWindow();
[DllImport("User32.dll")] [return: MarshalAs(UnmanagedType.Bool)]
   public static extern bool SetForegroundWindow (IntPtr hWnd);

public static IntPtr GetTopWindow() {
   return GetTopWindow(default); }

public static string GetWindowText (IntPtr hWnd) {
   int len = GetWindowTextLength(hWnd);
   if (len <= 0) {
      return null; }
   StringBuilder sb = new StringBuilder(len + 1);
   int len2 = GetWindowText(hWnd, sb, sb.Capacity);
   if (len2 <= 0) {
      return null; }
   return sb.ToString(0, len2); }

public static IntPtr GetNextWindow (IntPtr hWnd) {
   return GetWindow(hWnd, GW_HWNDNEXT); }

public static bool IsWindowVisible (IntPtr hWnd) {
   int style = GetWindowLong(hWnd, GWL_STYLE);
   return (style & WS_VISIBLE) != 0; }

public static uint getWindowProcessId (IntPtr hWnd) {
   uint processId = 0;
   GetWindowThreadProcessId(hWnd, out processId);
   return processId; }

public static string getWindowExeFileName (IntPtr hWnd) {
   try {
      var processId = getWindowProcessId(hWnd);
      if (processId == 0) {
         return null; }
      var process = Process.GetProcessById((int)processId);
      var mainModule = process.MainModule;
      return mainModule.FileName; }
    catch (Exception) {
      return null; }}

public static IntPtr FindNextSuitableFocusWindow() {
   var wh = WinUtils.GetForegroundWindow();
   if (wh == default) {
      return default; }
   while (true) {
      wh = WinUtils.GetNextWindow(wh);
      if (wh == default) {
         return default; }
      if (IsSuitableFocusWindow(wh)) {
         return wh; }}}

private static bool IsSuitableFocusWindow (IntPtr wh) {
   if (!WinUtils.IsWindowVisible(wh)) {
      return false; }
   var exeFileName = WinUtils.getWindowExeFileName(wh);
   if (exeFileName != default && exeFileName.EndsWith("\\Explorer.EXE", StringComparison.OrdinalIgnoreCase)) {
      return false; }
   return true; }

public static IntPtr FindWindowByTitle (string titleSubstring, string excludeSubstring) {
   if (titleSubstring == null || titleSubstring.Length == 0) {
      return default; }
   var wh = WinUtils.GetTopWindow();
   while (wh != default) {
      if (IsWindowVisible(wh)) {
         string title = GetWindowText(wh);
         if (title != null && title.IndexOf(titleSubstring, StringComparison.OrdinalIgnoreCase) >= 0 && title.IndexOf(excludeSubstring, StringComparison.OrdinalIgnoreCase) < 0) {
            return wh; }}
      wh = WinUtils.GetNextWindow(wh); }
   return default;}

public static IntPtr FindWindowByExeFile (string exeFileNameSubstring) {
   if (exeFileNameSubstring == null || exeFileNameSubstring.Length == 0) {
      return default; }
   var wh = WinUtils.GetTopWindow();
   while (wh != default) {
      if (IsWindowVisible(wh) && GetWindowText(wh) != default) {
         string fileName = getWindowExeFileName(wh);
         if (fileName != null && fileName.IndexOf(exeFileNameSubstring, StringComparison.OrdinalIgnoreCase) >= 0) {
            return wh; }}
      wh = WinUtils.GetNextWindow(wh); }
   return default;}

}
