using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dokumente_2_Subito.Infrastructure
{
  public static class String_Extension_2_HTML
  {
    public static string ToHtml(this string s)
    {
      s = HttpUtility.HtmlEncode(s);
      string[] paragraphs = s.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
      StringBuilder sb = new StringBuilder();
      foreach (string par in paragraphs) {
        sb.AppendLine("<p>");
        string p = par.Replace(Environment.NewLine, "<br />\r\n");
        sb.AppendLine(p);
        sb.AppendLine("</p>");
      }
      return sb.ToString();

    } //end   public static string ToHtml(this string s, bool nofollow)

  } //end   public static class String_Extension_2_HTML

} //end namespace Dokumente_2_Subito.Infrastructure

