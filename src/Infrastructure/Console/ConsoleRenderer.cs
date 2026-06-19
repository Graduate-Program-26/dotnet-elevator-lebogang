using System.Text;

public class ConsoleRenderer
{
    private const string Reset = "\x1b[0m";
    private const string Green = "\x1b[32m";
    private const string Red = "\x1b[31m";
    private const string Yellow = "\x1b[33m";
    private const string Cyan = "\x1b[36m";
    private const string White = "\x1b[37m";
    private const string CursorHome = "\x1b[H";

    // ── frame characters ─────────────────────────────────────
    private const char Full = '█';
    private const char Empty = '░';
    private const char VBar = '│';
    private const char HBar = '═';
    private const char TL = '╔';
    private const char TR = '╗';
    private const char BL = '╚';
    private const char BR = '╝';
    private const char LT = '╠';
    private const char RT = '╣';
    private const char TT = '╦';
    private const char BT = '╩';

    private const int FloorColWidth = 8;
    private const int ElevatorColWidth = 10;
    private const int WaitingColWidth = 10;
    private const int CapacityBarWidth = 8;


    public void DrawFrame(SystemStatusDto status, IReadOnlyList<LogEntryDto> recentLogs, string currentInput)
    {
        var frame = BuildFrame(status, recentLogs, currentInput);

        // move cursor to top-left — overwrite same characters in place
        Console.Write(CursorHome);
        Console.Write(frame);
    }

    /// <summary>
    /// Builds the complete frame as a string.
    /// Pure function — no console side effects.
    /// Called directly by tests.
    /// </summary>
    public string BuildFrame(SystemStatusDto status, IReadOnlyList<LogEntryDto> recentLogs, string currentInput)
    {
        var elevatorCount = status.Elevators.Count;
        var frameWidth = CalculateFrameWidth(elevatorCount);

        var sb = new StringBuilder();

        sb.AppendLine(BuildColumnHeaders(status.Elevators, frameWidth));
        sb.AppendLine(BuildShaftGrid(status, frameWidth));
        sb.AppendLine(BuildDivider(frameWidth));
        sb.AppendLine(BuildStatusStrip(status.Elevators, frameWidth));
        sb.AppendLine(BuildDivider(frameWidth));
        sb.AppendLine(BuildLogPanel(recentLogs, frameWidth));
        sb.AppendLine(BuildDivider(frameWidth));
        sb.AppendLine(BuildInputRow(currentInput, frameWidth));
        sb.Append(BuildBottom(frameWidth));

        return sb.ToString();
    }

    private static string BuildHeader(int elevatorCount, int frameWidth)
    {
        var sb = new StringBuilder();
        var time = DateTime.Now.ToString("HH:mm:ss");
        var info = $" {time} │ Elev: {elevatorCount} ";

        sb.AppendLine(BuildTop(frameWidth));
        sb.Append(PadRow(info, frameWidth));

        return sb.ToString();
    }

    private static string BuildColumnHeaders(IReadOnlyList<ElevatorStatusDto> elevators, int frameWidth)
    {
        var sb = new StringBuilder();
        sb.AppendLine(BuildDivider(frameWidth));

        var header = new StringBuilder();
        header.Append($" {"Floor",-6}");

        for(int i = 1; i <= elevators.Count; i++)
        {
            header.Append($" {VBar} elevator {i,-2}");
        }

        header.Append($" {VBar} {"Waiting",-4}");

        sb.Append(PadRow(header.ToString(), frameWidth));
        return sb.ToString();
    }

    /// <summary>
    /// The main building shaft grid — one row per floor, top to bottom.
    /// Shows elevator car at its current floor, waiting passengers on each floor.
    /// </summary>
    private static string BuildShaftGrid(SystemStatusDto status, int frameWidth)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(BuildDivider(frameWidth));

        var maxFloor = status.Floors.Max(floor => floor.FloorNumber);
        var minFloor = status.Floors.Min(floor => floor.FloorNumber);

        for (int floor = maxFloor; floor >= minFloor; floor--)
        {
            var row = new StringBuilder();
            row.Append($" {floor,4}   ");

            foreach (var elevator in status.Elevators)
            {
                row.Append(VBar);

                if (elevator.CurrentFloor == floor)
                {
                    var car = BuildElevatorCar(elevator);
                    row.Append($" {car} ");
                }
                else
                {
                    // empty shaft column
                    row.Append(new string(' ', ElevatorColWidth));
                }
            }

            // waiting passengers column
            row.Append(VBar);
            var floorStatus = status.Floors.FirstOrDefault(f => f.FloorNumber == floor);

            if (floorStatus?.WaitingPassengerCount > 0)
            {
                row.Append($" {Yellow} {floorStatus.WaitingPassengerCount}{Reset,-4}");
            }
            else
            {
                row.Append(new string(' ', WaitingColWidth));
            }


            stringBuilder.AppendLine(PadRow(row.ToString(), frameWidth));
        }

        return stringBuilder.ToString();
    }

    private static string BuildStatusStrip(IReadOnlyList<ElevatorStatusDto> elevators, int frameWidth)
    {
        var stringBuilder = new StringBuilder();

        foreach (var elevator in elevators)
        {
            var dirSymbol = DirectionSymbol(elevator.Direction);
            var stateColor = StateColor(elevator.State);
            var bar = CapacityBar(elevator.CurrentCapacity, elevator.MaxCapacity);
            var pax = $"{elevator.CurrentCapacity}/{elevator.MaxCapacity}";

            var line = $"Floor:{elevator.CurrentFloor,-3} " +
                       $"{dirSymbol}  " +
                       $"{stateColor}{elevator.State,-12}{Reset}" +
                       $"{pax,-6}" +
                       $"{bar}";

            stringBuilder.AppendLine(PadRow(line, frameWidth));
        }

        return stringBuilder.ToString();
    }


    private static string BuildLogPanel(IReadOnlyList<LogEntryDto> logs, int frameWidth)
    {
        var sb = new StringBuilder();
        var recent = logs.TakeLast(4);

        foreach (var entry in recent)
        {
            var line = $" {Cyan}LOG{Reset}  " +
                       $"{entry.Timestamp:HH:mm:ss}  " +
                       $"{entry.Message}";

            sb.AppendLine(PadRow(line, frameWidth));
        }

        // fill remaining rows 
        for (int i = logs.Count; i < 4; i++)
        {
            sb.AppendLine(PadRow("", frameWidth));
        }


        return sb.ToString();
    }


    private static string BuildInputRow(string input, int frameWidth)
        => PadRow($" > {input}_", frameWidth);


    private static string BuildElevatorCar(ElevatorStatusDto elevator)
    {
        var dir = DirectionSymbol(elevator.Direction);
        var color = StateColor(elevator.State);


        return $"{color}[{dir}]{Reset}";
    }


    private static string BuildTop(int width)
        => $"{TL}{new string(HBar, width - 2)}{TR}";

    private static string BuildBottom(int width)
        => $"{BL}{new string(HBar, width - 2)}{BR}";

    private static string BuildDivider(int width)
        => $"{LT}{new string(HBar, width - 2)}{RT}";


    private static string PadRow(string content, int frameWidth)
    {
        var visibleLength = StripAnsi(content).Length;
        var padding = Math.Max(0, frameWidth - visibleLength - 3);
        return $"{VBar}{content}{new string(' ', padding)}{VBar}";
    }

    private static int CalculateFrameWidth(int elevatorCount)
        => FloorColWidth + (elevatorCount * (ElevatorColWidth + 1)) + WaitingColWidth + 4;


    private static string DirectionSymbol(ElevatorDirection dir) => dir switch
    {
        ElevatorDirection.Up => "↑",
        ElevatorDirection.Down => "↓",
        ElevatorDirection.Stationary => "■",
        _ => "?"
    };

    private static string StateColor(ElevatorState state) => state switch
    {
        ElevatorState.Traveling => Green,
        ElevatorState.OutOfService => Red,
        ElevatorState.Idle => White,
        _ => White
    };


    private static string CapacityBar(int current, int max)
    {
        if (max == 0) return new string(Empty, CapacityBarWidth);

        var filled = (int)Math.Round((double)current / max * CapacityBarWidth);
        var isFull = current >= max;
        var fillChar = isFull ? $"{Red}{Full}{Reset}" : $"{Full}";

        return string.Concat(Enumerable.Range(0, CapacityBarWidth).Select(i => i < filled ? fillChar : $"{Empty}"));
    }

    private static string StripAnsi(string input)
        => System.Text.RegularExpressions.Regex.Replace(input, @"\x1b\[[0-9;]*m", "");
}