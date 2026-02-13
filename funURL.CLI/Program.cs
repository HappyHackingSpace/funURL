using System.CommandLine;
using funURL.CLI.Commands;

using var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cancellationTokenSource.Cancel();
};

var rootCommand = new RootCommand("funURL - URL manipulation toolkit")
{
    ParseCommand.Create(),
    ModifyCommand.Create(),
    EncodeCommand.Create(),
    DecodeCommand.Create(),
    DedupeCommand.Create(),
};

return await rootCommand.Parse(args).InvokeAsync(cancellationToken: cancellationTokenSource.Token);
