namespace Fb2Parser.Model
{
    public enum DocGenerationInstructionType
    {
        require,
        allow,
        deny,
    }

    /// <summary>
    /// Align for table cells
    /// </summary>
    public enum AlignType
    {
        left,
        right,
        center,
    }

    /// <summary>
    /// Align for table cells
    /// </summary>
    public enum VerticalAlignType
    {
        top,
        middle,
        bottom,
    }

    /// <summary>
    /// Modes for document sharing (free|paid for now)
    /// </summary>
    public enum ShareModesType
    {
        free,
        paid,
    }
}
