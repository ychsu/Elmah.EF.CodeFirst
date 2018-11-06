namespace Elmah.EF.CodeFirst
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ELMAH_Error
    {
        public Guid ErrorId { get; set; }

        [StringLength(60)]
        public string Application { get; set; }

        [StringLength(50)]
        public string Host { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        [StringLength(60)]
        public string Source { get; set; }

        [StringLength(500)]
        public string Message { get; set; }

        [StringLength(50)]
        public string User { get; set; }

        public int StatusCode { get; set; }

        public DateTime TimeUtc { get; set; }

        public int Sequence { get; set; }

        public string AllXml { get; set; }
    }
}