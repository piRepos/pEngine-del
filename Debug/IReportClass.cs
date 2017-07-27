using System;

namespace pEngine.Debug
{
    public interface IReportClass : IDisposable
    {
        /// <summary>
        /// Links the report class to the debugger.
        /// </summary>
        void Link(DebugModule debugInstance);
    }
}
