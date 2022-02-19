# kbSend - Send text through the keyboard

This is a Windows command-line utility program, written in C#.
It transfers text data through the keyboard input channel.
It is used when the clipboard cannot be used to paste text or for simple automation scripts.

Example for sending a string to the Remote Desktop Client (mstsc.exe):

<pre>kbsend -text "abc" -windowTitle "Remote Desktop Connection"</pre>

Example for copying text from the clipboard to the Citrix Client:

<pre>kbsend -clipboard -windowExe "\CDViewer.exe" -charDelay 12 -lineDelay 125</pre>

## Parameters

### Input text

<pre>
-text "..."
   Text string passed as a parameter.
-clipboard
   Text is read from the clipboard.
-file &lt;fileName&gt;
   Text is read from a file.
</pre>

### Target window

<pre>
-nextWindow
   Moves the focus to the next window.
   Windows Explorer windows are ignored.
   This is the default.
-currentWindow
   Keeps the current focus window.
-windowTitle "title substring"
   Moves the focus to the window with the specified string in the title.
   Case-insensitive substring search is used to compare the window titles.
-windowExe "exe file path substring"
   Moves the focus to the window with the specified string in the EXE file path.
   Case-insensitive substring search is used to compare the EXE file paths.
</pre>

### Misc. options

<pre>
-charDelay &lt;ms&gt;
   Delay per character in milliseconds.
   Default is 1.
-lineDelay &lt;ms&gt;
   Delay per line in milliseconds.
   Default is 25.
-sendHomeKey
   Sends HOME after ENTER when the previous line started with blanks.
   This is necessary when line indent is inherited from the line above.
-listWindows
   Lists window titles and EXE file names of the desktop windows.
-help
   Displays this help text.
</pre>
