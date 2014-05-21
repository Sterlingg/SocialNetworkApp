using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialNetworkApp.Models;

namespace TheTests.Mock
{
    class MockPostRepository : IPostRepository
    {

        public IQueryable<Post> FindAllPosts()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Post> FindNearbyPosts(decimal lat, decimal lon)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Post> FindPostByGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        public Post GetPost(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(Post post)
        {
            throw new NotImplementedException();
        }

        public void Delete(Post post)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
