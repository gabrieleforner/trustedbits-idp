namespace Trustedbits.APIServer;

/// <summary>
/// Application entrypoint
/// </summary>
public class Program
{
    /// <summary>
    /// Main method
    /// </summary>
    /// <param name="args">Launch arguments</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}