using System.IO;

namespace ClipboardAI;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var app = new System.Windows.Application();
        app.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
        var tempPath = args.Length > 0 && File.Exists(args[0]) ? args[0] : null;
        var context = new AppContext(tempPath);
        app.Run();
    }
}
