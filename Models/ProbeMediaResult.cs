using System;
using System.Collections.Generic;
using System.Text;

namespace DLClip.Models
{
    internal class ProbeMediaResult
    {
        public int? DurationSeconds { get; set; }
        public string ContainerFormat { get; set; } = "";
        public bool HasVideo { get; set; }

        // (for size estimation)

        public int? AudioBitrateKbps { get; set; }
        public bool HasAudio { get; set; }


        // video only
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? VideoCodec { get; set; }
        public int? VideoBitrateKbps { get; set; }
        public double? Fps { get; set; }

        // url only
        public string[]? PossibleFormats { get; set; } // plan to change type to a custom type

        // probe result pass/fail
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorTitle { get; set; }
    }
}
