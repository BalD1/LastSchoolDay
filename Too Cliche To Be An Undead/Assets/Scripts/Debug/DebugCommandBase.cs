using System;

public class DebugCommandBase
{
    private string commandID;
    private string commandDescription;
    private string commandFormat;

    public string CommandID { get => commandID; }
    public string CommandDescription { get => commandDescription; }
    public string CommandFormat { get => commandFormat; }

    public DebugCommandBase(string _id, string _description, string _format)
    {
        commandID = _id;
        commandDescription = _description;
        commandFormat = _format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string _id, string _description, string _format, Action _command)
           : base(_id, _description, _format)
    {
        this.command = _command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> command;

    public DebugCommand(string _id, string _description, string _format, Action<T1> _command)
           : base(_id, _description, _format) => this.command = _command;

    public void Invoke(T1 val) => command?.Invoke(val);
}

public class DebugCommand<T1,T2> : DebugCommandBase
{
    private Action<T1, T2> command;

    public DebugCommand(string _id, string _description, string _format, Action<T1, T2> _command)
       : base(_id, _description, _format) => this.command = _command;

    public void Invoke(T1 val_1, T2 val_2) => command?.Invoke(val_1, val_2);
}
