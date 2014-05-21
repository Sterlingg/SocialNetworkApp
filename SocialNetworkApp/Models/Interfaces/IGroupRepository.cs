// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-16-2012
// ***********************************************************************
// <copyright file="IGroupRepository.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkApp.Models
{
    public interface IGroupRepository
    {
        IQueryable<Group> GetPopularGroups();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        //A work around
        List<Group> NotPrivateOrOwner(User u);
        IQueryable<Group> SearchOnTerm(string term);
        void MakePublic(int GroupId);
    }
}