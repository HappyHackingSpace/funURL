using System.CommandLine;
using Figgle.Fonts;

namespace funURL.CLI.Commands;

/// <summary>
/// The root command for the funURL CLI.
/// </summary>
public class RootCommand : System.CommandLine.RootCommand
{
    private readonly Option<bool> silentOption = new("--silent", "-s") { Description = "Suppress the startup banner", Recursive = true };

    private RootCommand()
        : base("funURL - A Functional URL Swiss Army Knife")
    {
        Options.Add(silentOption);
        Subcommands.Add(ParseCommand.Create());
        Subcommands.Add(ModifyCommand.Create());
        Subcommands.Add(EncodeCommand.Create());
        Subcommands.Add(DecodeCommand.Create());
        Subcommands.Add(DedupeCommand.Create());
    }

    public async Task<ParseResult> Parse(IReadOnlyList<string> args, CancellationToken cancellationToken)
    {
        var parseResult = base.Parse(args, null);

        if (!parseResult.GetValue(silentOption))
        {
            await Console.Out.WriteAsync(FiggleFonts.Standard.Render("funURL").AsMemory(), cancellationToken);
        }

        return parseResult;
    }

    public static RootCommand Create() => new();
}
