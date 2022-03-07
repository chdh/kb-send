using Console                = System.Console;
using Environment            = System.Environment;
using Exception              = System.Exception;
using File                   = System.IO.File;
using IntPtr                 = System.IntPtr;
using Math                   = System.Math;
using Thread                 = System.Threading.Thread;

internal class MainClass {

private static string        kbText;                       // text to be sent to the keyboard
private static IntPtr        targetWindow;                 // window handle of target window

private static void ListWindow (IntPtr wh) {
   var isVisible = WinUtils.IsWindowVisible(wh);
   var isForeground = wh == WinUtils.GetForegroundWindow();
   var title = WinUtils.GetWindowText(wh);
   if (!isForeground && (!isVisible || title == null)) {
      return; }
   var exeFileName = WinUtils.getWindowExeFileName(wh);
   if (isForeground) {
      Console.WriteLine("** Foreground Window **"); }
   if (!isVisible) {
      Console.WriteLine("** invisible **"); }
   Console.WriteLine("Title: " + (title ?? "(no title)"));
   Console.WriteLine("File: " + (exeFileName ?? "(undefined)"));
   Console.WriteLine(); }

private static void ListWindows() {
   var wh = WinUtils.GetTopWindow();
   while (wh != default) {
      ListWindow(wh);
      wh = WinUtils.GetNextWindow(wh); }}

private static void GetKbText() {
   if (CommandLine.inputTextOpt) {
      kbText = CommandLine.inputTextArg; }
    else if (CommandLine.clipboardOpt) {
      kbText = NativeClipboard.getText(); }
    else if (CommandLine.inputFileOpt) {
      kbText = File.ReadAllText(CommandLine.inputFileArg); }
    else {
      throw new SimpleException("No input text specified."); }
   if (kbText == null || kbText.Length == 0) {
      throw new SimpleException("Undefined or empty input text."); }}

private static void ActivateTargetWindow() {
   var oldForegroundWindow = WinUtils.GetForegroundWindow();
   if (oldForegroundWindow == targetWindow) {
      return; }
   if (!WinUtils.SetForegroundWindow(targetWindow)) {
      throw new SimpleException("Unable to activate target window."); }
   int startTime = Environment.TickCount;
   while (true) {
      var wh = WinUtils.GetForegroundWindow();
      if (wh == targetWindow) {
         break; }
      int elapsedMs = Environment.TickCount - startTime;
      if (elapsedMs > 5000) {
         throw new SimpleException("Timeout while waiting for target window to become active."); }
      Thread.Sleep(50); }
   Thread.Sleep(50); }

private static void PrepareTargetWindow() {
   if (CommandLine.currentWindowOpt) {
      targetWindow = WinUtils.GetForegroundWindow(); }
    else if (CommandLine.nextWindowOpt) {
      targetWindow = WinUtils.FindNextSuitableFocusWindow();
      if (targetWindow == default) {
         throw new SimpleException("Unable to find next window."); }
      ActivateTargetWindow(); }
    else if (CommandLine.windowTitleOpt) {
      targetWindow = WinUtils.FindWindowByTitle(CommandLine.windowTitleArg, "-windowTitle");
         // Windows with "-windowTitle" in the title are excluded because cmd.exe adds the command line string the window title of the command prompt window.
      if (targetWindow == default) {
         throw new SimpleException("Unable to find window by title."); }
      ActivateTargetWindow(); }
    else if (CommandLine.windowExeOpt) {
      targetWindow = WinUtils.FindWindowByExeFile(CommandLine.windowExeArg);
      if (targetWindow == default) {
         throw new SimpleException("Unable to find window by EXE file name."); }
      ActivateTargetWindow(); }
    else {
       Utils.Assert(false); }
   if (targetWindow == default) {
      throw new SimpleException("Unable to determine target window."); }}

private static void verifyFocustWindow() {
   if (WinUtils.GetForegroundWindow() != targetWindow) {
      throw new SimpleException("Target window has lost focus."); }}

private static void sendLine (string s) {
   const int chunkSize = 10;
   int p = 0;
   while (p < s.Length) {
      verifyFocustWindow();
      int len = Math.Min(s.Length - p, chunkSize);
      NativeKeyboard.SendChars(s.Substring(p, len));
      p += len;
      if (CommandLine.charDelay > 0) {
         Thread.Sleep((int)Math.Ceiling(len * CommandLine.charDelay)); }}}

private static void SendKbText() {
   int eolCount = 0;
   int kbTextPos = 0;
   while (kbTextPos < kbText.Length) {
      string line = Utils.scanTextLine(kbText, ref kbTextPos);
      sendLine(line);
      if (kbTextPos >= kbText.Length) {
         break; }
      verifyFocustWindow();
      NativeKeyboard.SendKey(NativeKeyboard.VK_ENTER);
      if (CommandLine.sendHomeKeyOpt && line.Length > 0 && line[0] == ' ') {
         NativeKeyboard.SendKey(NativeKeyboard.VK_HOME); }
      eolCount++;
      Console.Write(".");
      if (CommandLine.lineDelay > 0) {
         verifyFocustWindow();
         Thread.Sleep((int)Math.Ceiling(CommandLine.lineDelay)); }}
   if (eolCount > 0) {
      Console.WriteLine(); }}

private static void Main2() {
   CommandLine.Init();
   if (CommandLine.helpOpt) {
      CommandLine.DisplayHelp();
      return; }
   if (CommandLine.listWindowsOpt) {
      ListWindows();
      return; }
   GetKbText();
   PrepareTargetWindow();
   SendKbText(); }

public static int Main() {
   try {
      Main2(); }
    catch (SimpleException e) {
      Console.WriteLine("\nError: " + e.Message);
      return 99; }
    catch (Exception e) {
      Console.WriteLine("\nError: " + e.ToString());
      return 99; }
   return 0; }

}
