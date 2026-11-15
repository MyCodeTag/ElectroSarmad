using System.Collections.Generic;
using System.Data;

namespace Sarmad.Models
{
    public class UserInfo
    {
        public int Si { get; set; }
        public string? Token { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Active { get; set; }
        public string? Knd { get; set; }
    }
    public class FormData
    {
        public DataTable? lastForms { get; set; }
        public DataTable? Pages { get; set; }
        public string? Today { get; set; }
    }

    public class FormSetup
    {
        public int FormId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Header { get; set; }
        public string? Footer { get; set; }
        public string? MSG { get; set; }
        public string? Today { get; set; }
        public string? Script { get; set; }

        public string Base { get; set; }
        public string BaseIcon { get; set; }
        public string BaseArea { get; set; }

        public string Daily { get; set; }
        public string DailyIcon { get; set; }
        public string DailyArea { get; set; }

        public string Report { get; set; }
        public string ReportIcon { get; set; }
        public string ReportArea { get; set; }

        public string Other { get; set; }
        public string OtherIcon { get; set; }
        public string OtherArea { get; set; }

    }

    public class ListForm
    {
        public required string Daste { get; set; }
        public required string Name { get; set; }
        public required string Adres { get; set; }
        public required string Css { get; set; }

    }

    public class LimitDate
    {
        public required string FromDate { get; set; }
        public required string ToDate { get; set; }
    }
    public class JResult
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
    }
}
