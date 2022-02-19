using Exception          = System.Exception;

// Used when only the error message is relevant and the stack trace should not be displayed.
internal class SimpleException : Exception {
   public SimpleException (string msg) : base(msg) {}}

//------------------------------------------------------------------------------

internal class Utils {

public static void Assert (bool cond) {
   if (!cond) {
      throw new Exception("Program logic error. Assertion failed."); }}

public static string scanTextLine (string text, ref int p) {
   int p0 = p;
   while (p < text.Length && text[p] != 0x0D && text[p] != 0x0A) {
      p++; }
   int p1 = p;
   if (p < text.Length && text[p] == 0x0D) {
      p++; }
   if (p < text.Length && text[p] == 0x0A) {
      p++; }
   return text.Substring(p0, p1 - p0); }

}
