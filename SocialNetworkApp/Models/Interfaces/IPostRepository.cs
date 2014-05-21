// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-16-2012
// ***********************************************************************
// <copyright file="IPostRepository.cs" company="">
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
    public interface IPostRepository
    {
        IQueryable<Post> FindAllPosts();
        IQueryable<Post> FindNearbyPosts(decimal lat, decimal lon);
        IQueryable<Post> FindPostByGroup(int groupId);

        Post GetPost(int id);

        void Add(Post post);
        void Delete(Post post);

        void Save();
    }
}