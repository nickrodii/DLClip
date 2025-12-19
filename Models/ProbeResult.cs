using System;
using System.Collections.Generic;
using System.Text;

namespace DLClip.Models
{
    internal class ProbeResult
    {
        public bool Success { get; set; }
        public bool TimedOut { get; set; }
        public int? ExitCode { get; set; }
        public string StdOut { get; set; }
        public string StdErr { get; set; }
        public string ErrorMessage { get; set; }

        public ProbeResult()
        {
            bool success = false;
            bool timedOut = false;
            int? exitCode = null;
            string stdOut = "";
            string stdErr = "";
            string errorMessage = "";
        }
    }
}
