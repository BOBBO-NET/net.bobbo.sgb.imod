using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BobboNet.SGB.IMod
{
    /// <summary>
    /// The result of a single approval operation.
    /// </summary>
    public enum ApproveEventResult
    {
        Error = 0,
        Approve,
        Cancel
    }
}