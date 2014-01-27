using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// Taken from: 
// http://stackoverflow.com/questions/3764639/the-specified-linq-expression-contains-references-to-queries-that-are-associated

namespace SocialNetworkApp.Models
{
    public class ContextHelper
    {
        //I don't think that commenting that out is going to go well.
          [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
          private SocialContext context;

          [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
          public static SocialContext GetContext()
          {
              if (!HttpContext.Current.Items.Contains("_db_context"))
              {
                  HttpContext.Current.Items.Add("_db_context", new SocialContext());
              }
              return (SocialContext)HttpContext.Current.Items["_db_context"];
          }
    }
}