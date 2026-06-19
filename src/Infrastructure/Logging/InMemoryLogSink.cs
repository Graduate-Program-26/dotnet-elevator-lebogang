
using System.Collections.Concurrent;
using Serilog.Core;
using Serilog.Events;

public class InMemoryLogSink : ILogEventSink
{
    private readonly ConcurrentQueue<LogEntryDto> _entries = new();
    private const int MaxEntries = 20;

    public void Emit(LogEvent logEvent)
    {
        _entries.Enqueue(new LogEntryDto(logEvent.Timestamp.DateTime,logEvent.RenderMessage()));

        // keep only the most recent entries
        while (_entries.Count > MaxEntries)
        {
              _entries.TryDequeue(out _);
        }
          
    }

    /// <summary>Returns the most recent N log entries, oldest first.</summary>
    public IReadOnlyList<LogEntryDto> GetRecent(int count)
        => _entries.TakeLast(count).ToList();
}