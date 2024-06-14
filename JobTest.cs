using System.Diagnostics;

namespace RemoteJob;

public class JobTest
{
    public static async Task ExecuteProcessAsync(Func<string, Task<string>> callback)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "run -p 3000:3000 svenwal/jsonplaceholder", // Comando a ser executado
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += async (sender, e) =>
        {
            if (e.Data != null)
            {
                await callback(sender.ToString() + " || " + e.Data);
            }
        };

        process.ErrorDataReceived += async (sender, e) =>
        {
            if (e.Data != null)
            {
                await callback(sender.ToString() + " || " + e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
    }
}