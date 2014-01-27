using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace SocialNetworkApp.Models
{
    public interface ISocialContext
    {
      IDbSet<Post> Posts { get; }
      IDbSet<Location> Locations { get;  }
      IDbSet<Group> Groups { get; }
      IDbSet<User> Users { get; }

      int SaveChanges();
    }      
}
