namespace ClipboardAI;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var app = new System.Windows.Application();
        app.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
        var context = new AppContext();
        app.Run();
    }
}
