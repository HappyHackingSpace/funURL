using funURL.CLI.Commands;

using var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cancellationTokenSource.Cancel();
};

var rootCommand = RootCommand.Create();

var parseResult = await rootCommand.Parse(args, cancellationToken);

return await parseResult.InvokeAsync(cancellationToken: cancellationToken);
