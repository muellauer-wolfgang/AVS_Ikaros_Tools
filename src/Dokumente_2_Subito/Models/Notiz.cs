using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dokumente_2_Subito.Infrastructure;

namespace Dokumente_2_Subito.Models
{
  public class Notiz : IEquatable<Notiz>, IComparable<Notiz>
  {
    public DateTime? Datum {  get; set; }
    public NotizArt Art { get; set; }
    public string Kürzel {  get; set; }
    public string Kurztext { get; set; }
    public string Text {  get; set; }
    public string FileName { get; set; }  
    public override string ToString()
    {
      return $"Am:{Datum:yyyy-MM-dd} Art:{Art} K:{Kürzel} KT:{Kurztext}"; 
    }


    public string AsHtmlTableRow()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("<tr>");
      //Datum
      sb.Append("<td>");
      if (this.Datum.HasValue) { sb.Append(this.Datum.Value.ToString("yyyy&#x2011;MM&#x2011;dd")); }
      sb.AppendLine("</td>");
      //Kürzel
      sb.Append("<td>");
      if (!string.IsNullOrEmpty(this.Kürzel)) { sb.Append(this.Kürzel); }
      sb.AppendLine("</td>");
      //Kurztext
      sb.Append("<td>");
      if (!string.IsNullOrEmpty(this.Kurztext)) { sb.Append(this.Kurztext); }
      sb.AppendLine("</td>");
      //Text
      sb.Append("<td>");
      if (!string.IsNullOrEmpty(this.Text)) { sb.Append(this.Text.ToHtml()); }
      sb.AppendLine("</td>");
      //FileName
      sb.Append("<td>");
      if (!string.IsNullOrEmpty(this.FileName)) { sb.Append(this.FileName); }
      sb.AppendLine("</td>");

      sb.AppendLine("</tr>");
      return sb.ToString() ;
    }

    public bool Equals(Notiz rhs)
    {
      if (this.Datum.HasValue && rhs.Datum.HasValue) {
        if (this.Datum.Value != rhs.Datum.Value) { return false; }
      }
      if (this.Datum.HasValue ^ rhs.Datum.HasValue ) { return false; }

      if (this.Art != rhs.Art) { return false; }

      if (!string.IsNullOrEmpty(this.Kurztext) && !string.IsNullOrEmpty(rhs.Kurztext)) {
        if (!this.Kurztext.Equals(rhs.Kurztext)) { return false; }
      }

      if (string.IsNullOrEmpty(this.Kurztext) ^ string.IsNullOrEmpty(rhs.Kurztext)) {
        return false;
      }

      if (!string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(rhs.Text)) {
        if (!this.Text.Equals(rhs.Text)) { return false; }
      }

      if (string.IsNullOrEmpty(this.Text) ^ string.IsNullOrEmpty(rhs.Text)) {
        return false;
      }

      return true;
    }

    public int CompareTo(Notiz rhs)
    {
      if (this.Equals(rhs)) { return 0; }
      if (this.Datum.HasValue && rhs.Datum.HasValue) {
        return (this.Datum.Value.CompareTo(rhs.Datum.Value));
      }
      if (!string.IsNullOrEmpty(this.Kurztext) && !string.IsNullOrEmpty(rhs.Kurztext)) {
        return (this.Kurztext.CompareTo(rhs.Kurztext));
      }
      if (!string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(rhs.Text)) {
        return (this.Text.CompareTo(rhs.Text));
      }
      return 0;
    }


  } //end   public class Notiz

  public enum NotizArt
  {
    Unbefiniert,
    Gläubiger,
    Schuldner,
    Akt,
    Bemerkung,
    Datei
  };

} //end namespace Dokumente_2_Subito.Models

