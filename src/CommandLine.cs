// Command line parameters

using Console                = System.Console;
using Environment            = System.Environment;

internal class CommandLine {

// Input text
public static bool           inputTextOpt;
public static string         inputTextArg;
public static bool           clipboardOpt;
public static bool           inputFileOpt;
public static string         inputFileArg;

// Target window
public static bool           nextWindowOpt;
public static bool           currentWindowOpt;
public static bool           windowTitleOpt;
public static string         windowTitleArg;
public static bool           windowExeOpt;
public static string         windowExeArg;

// Misc. options
public static double         charDelay;
public static double         lineDelay;
public static bool           sendHomeKeyOpt;
public static bool           listWindowsOpt;
public static bool           helpOpt;

public static void DisplayHelp() {
   Console.WriteLine();
   Console.WriteLine("kbSend - Keyboard send");
   Console.WriteLine("----------------------");
   Console.WriteLine();
   Console.WriteLine("This program injects keystrokes into the Windows keyboard input stream.");
   Console.WriteLine();
   Console.WriteLine("Author: Christian d'Heureuse, Inventec Informatik AG, Switzerland");
   Console.WriteLine("   www.source-code.biz, www.inventec.ch");
   Console.WriteLine();
   Console.WriteLine("Command line parameters:");
   Console.WriteLine();
   Console.WriteLine("=== Input text ===");
   Console.WriteLine();
   Console.WriteLine("-text \"...\"");
   Console.WriteLine("   Text string passed as a parameter.");
   Console.WriteLine("-clipboard");
   Console.WriteLine("   Text is read from the clipboard.");
   Console.WriteLine("-file <fileName>");
   Console.WriteLine("   Text is read from a file.");
   Console.WriteLine();
   Console.WriteLine("=== Target window ===");
   Console.WriteLine();
   Console.WriteLine("-nextWindow");
   Console.WriteLine("   Moves the focus to the next window.");
   Console.WriteLine("   Windows Explorer windows are ignored.");
   Console.WriteLine("   This is the default.");
   Console.WriteLine("-currentWindow");
   Console.WriteLine("   Keeps the current focus window.");
   Console.WriteLine("-windowTitle \"title substring\"");
   Console.WriteLine("   Moves the focus to the window with the specified string in the title.");
   Console.WriteLine("   Case-insensitive substring search is used to compare the window titles.");
   Console.WriteLine("-windowExe \"exe file path substring\"");
   Console.WriteLine("   Moves the focus to the window with the specified string in the EXE file path.");
   Console.WriteLine("   Case-insensitive substring search is used to compare the EXE file paths.");
   Console.WriteLine();
   Console.WriteLine("=== Misc. options ===");
   Console.WriteLine();
   Console.WriteLine("-charDelay <ms>");
   Console.WriteLine("   Delay per character in milliseconds.");
   Console.WriteLine("   Default is 1.");
   Console.WriteLine("-lineDelay <ms>");
   Console.WriteLine("   Delay per line in milliseconds.");
   Console.WriteLine("   Default is 25.");
   Console.WriteLine("-sendHomeKey");
   Console.WriteLine("   Sends HOME after ENTER when the previous line started with blanks.");
   Console.WriteLine("   This is necessary when line indent is inherited from the line above.");
   Console.WriteLine("-listWindows");
   Console.WriteLine("   Lists window titles and EXE file names of the desktop windows.");
   Console.WriteLine("-help");
   Console.WriteLine("   Displays this help text.");
   Console.WriteLine(); }

public static void Init() {
   parseArgs();
   completeArgs(); }

private static void parseArgs() {
   string[] args = Environment.GetCommandLineArgs();
   if (args.Length <= 1) {
      helpOpt = true;
      return; }
   charDelay = 1;
   lineDelay = 25;
   int argp = 1;
   while (argp < args.Length) {
      string t = args[argp++];
      switch (t) {
         case "-text": {
            inputTextOpt = true;
            inputTextArg = GetArg(args, ref argp);
            break; }
         case "-clipboard": {
            clipboardOpt = true;
            break; }
         case "-file": {
            inputFileOpt = true;
            inputFileArg = GetArg(args, ref argp);
            break; }
         case "-nextWindow": {
            nextWindowOpt = true;
            break; }
         case "-currentWindow": {
            currentWindowOpt = true;
            break; }
         case "-windowTitle": {
            windowTitleOpt = true;
            windowTitleArg = GetArg(args, ref argp);
            break; }
         case "-windowExe": {
            windowExeOpt = true;
            windowExeArg = GetArg(args, ref argp);
            break; }
         case "-charDelay": {
            charDelay = GetArgDouble(args, ref argp);
            break; }
         case "-lineDelay": {
            lineDelay = GetArgDouble(args, ref argp);
            break; }
         case "-sendHomeKey": {
            sendHomeKeyOpt = true;
            break; }
         case "-listWindows": {
            listWindowsOpt = true;
            break; }
         case "-help": case "-h": case "-?": {
            helpOpt = true;
            break; }
         default: {
            throw new SimpleException("Invalid command line argument \"" + t + "\"."); }}}}

private static void completeArgs() {
   if (!nextWindowOpt && !currentWindowOpt && !windowTitleOpt && !windowExeOpt) {
      nextWindowOpt = true; }}

private static string GetArg (string[] args, ref int argp) {
   if (argp >= args.Length) {
      throw new SimpleException("End of line reached when a command line argument was expected."); }
   string s = args[argp++].Trim();
   return (s.Length == 0) ? null : s; }

private static int GetArgInt (string[] args, ref int argp) {
   return int.Parse(GetArg(args, ref argp)); }

private static double GetArgDouble (string[] args, ref int argp) {
   return double.Parse(GetArg(args, ref argp)); }

}
