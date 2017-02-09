using System;
using System.Diagnostics;
using System.Reflection;
using Util.Concurrency;

namespace Util.Log {
  public static class Log {
    private const string INFORMATION = "INFO ";
    private const string WARNING = "WARN ";
    private const string ERROR = "ERROR";
    private const string DEBUG = "DEBUG";


    private const string LOG_FILE = "./log.txt";

    // TODO: cancellation?
    private static readonly TaskQueue Bq = new TaskQueue();
    private static readonly TraceSource Ts = new TraceSource(AppDomain.CurrentDomain.FriendlyName);

    static Log() {
      var twtl = new TextWriterTraceListener(LOG_FILE) {
        Name="TextLogger"
      };

      var ctl = new ConsoleTraceListener(false);

      Ts.Listeners.Clear();
      Ts.Listeners.Add(twtl);
      Ts.Listeners.Add(ctl);
      Bq.Start();
      //Trace.WriteLine("The first line to be in the logfile and on the console.");
    }

    private static void WriteLine(string type, string msg, MethodBase mb) {
      foreach (TraceListener listener in Ts.Listeners) {
        listener.WriteLine(
          $"[{type} {DateTime.Now.ToString("HH:mm:ss")} {Ts.Name}:{mb?.ReflectedType?.FullName}::{mb?.Name??"?"}] {msg}\n");
      }
    }

    public static void Info() {
      var m = new StackFrame(1).GetMethod();
      Bq.Execute(() => WriteLine(INFORMATION, "", m));
    }

    public static void Info(string msg) {
      var m = new StackFrame(1).GetMethod();
      Bq.Execute(() => WriteLine(INFORMATION, msg, m));
    }

    public static void Warn(string msg) {
      var m = new StackFrame(1).GetMethod();
      Bq.Execute(() => WriteLine(WARNING, msg, m));
    }

    public static void Warn(string msg, Exception e) {
      var m = new StackFrame(1).GetMethod();
      Bq.Execute(() => WriteLine(WARNING, msg+"\n"+e.StackTrace, m));
    }

    public static void Err(string msg) {
      var m = new StackFrame(1).GetMethod();
      Bq.Execute(() => WriteLine(ERROR, msg, m));
    }

    public static void Err(string msg, Exception e) {
      var m = new StackFrame(1).GetMethod();
      Bq.Execute(() => WriteLine(ERROR, msg+"\n"+e.StackTrace, m));
    }

    [Conditional("DEBUG")]
    public static void Debug(string msg) {
      var m = new StackFrame(1).GetMethod();
      Bq.Execute(() => WriteLine(DEBUG, msg, m));
    }
  }
}