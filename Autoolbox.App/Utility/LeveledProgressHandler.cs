using Spectre.Console;

namespace Autoolbox.App.Utility;

public static class LeveledProgressHandler
{
    public static async Task Handle(int levels, Func<int, ProgressTask, Task> process)
    {
        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new ElapsedTimeColumn())
            .StartAsync(async ctx =>
            {
                var tasks = new ProgressTask[levels];

                for (var index = 0; index < tasks.Length; index++)
                {
                    tasks[index] = ctx.AddTask($"[green]Level {index+1}[/]", false);
                }

                for (var level = 0; level < levels; level++)
                {
                    var task = tasks[level];
                    task.StartTask();

                    await process(level, task);

                    task.StopTask();
                }
            });
    }
}