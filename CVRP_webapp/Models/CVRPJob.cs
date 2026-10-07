using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Models
{
#pragma warning disable S101
    public class CVRPJob
#pragma warning restore S101
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public JobStatus JobStatus { get; set; } = JobStatus.Pending;
        public InputData InputData { get; init; } = null!;
        public int MultistartCount { get; init; } = 0;
        public int HistoryCount { get; init; } = 0;
        public string Solver { get; init; } = "";
        public CVRPTaskResult? Result { get; set; }
        public string Error { get; set; } = "";
    }

    public enum JobStatus
    {
        Pending = 0, Running = 1, Completed = 2, Failed = 3
    }

}
