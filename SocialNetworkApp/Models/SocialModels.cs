// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-16-2012
// ***********************************************************************
// <copyright file="SocialModels.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SocialNetworkApp.Models
{
    public class SocialContext : DbContext, ISocialContext
    {
        public SocialContext()
            : base("DefaultConnection")
        {
        }

        public IDbSet<Post> Posts { get; set; }
        public IDbSet<Location> Locations { get; set; }
        public IDbSet<Group> Groups { get; set; }
        public IDbSet<User> Users { get; set; }
        //public DbSet<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Location>().Property(a => a.Latitude).HasPrecision(35, 15);
            modelBuilder.Entity<Location>().Property(a => a.Longitude).HasPrecision(35, 15);
            modelBuilder.Entity<User>()
                .HasMany(c => c.Groups2).WithMany(i => i.Users2)
                .Map(t => t.MapLeftKey("GroupID")
                .MapRightKey("UserID")
                .ToTable("Subscription"));
            modelBuilder.Entity<User>()
                .HasMany(c => c.Groups1).WithMany(i => i.Users1)
                .Map(t => t.MapLeftKey("GroupID")
                    .MapRightKey("AllowedUserID")
                    .ToTable("AllowedSubscriber"));
            modelBuilder.Entity<User>()
                .HasMany(c => c.Groups).WithMany(i => i.Users)
                .Map(t => t.MapLeftKey("GroupID")
                    .MapRightKey("OwnerUserID")
                    .ToTable("GroupOwner"));
            modelBuilder.Entity<User>()
                .HasMany(c => c.Posts1).WithMany(i => i.Users)
                .Map(t => t.MapLeftKey("BlockedPostID")
                    .MapRightKey("BlockerUserID")
                    .ToTable("BlockedPost"));
            modelBuilder.Entity<User>()
                .HasMany(c => c.User1).WithMany(i => i.Users)
                .Map(t => t.MapLeftKey("BlockedUserID")
                    .MapRightKey("BlockerUserID")
                    .ToTable("BlockedUser"));
        }
    }

    public class Post
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PostID { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public DateTime CreationDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Post visible until:")]
        public Nullable<DateTime> EndDate { get; set; }

        [Display(Name = "Seen from a distance of: (in meters)")]
        public Nullable<decimal> VisibleProximity { get; set; }

        [ForeignKey("Group")]
        [Display(Name = "Post in group:")]
        public Nullable<int> GroupID { get; set; }
        public virtual Group Group { get; set; }

        public Nullable<int> LocationID { get; set; }
        public virtual Location Location { get; set; }
        // User refers to the user that created this post

        [ForeignKey("User")]
        public Nullable<int> UserID { get; set; }
        public virtual User User { get; set; }
        // Users refers to the users who have blocked this post
        public virtual ICollection<User> Users { get; set; }
    }

    public class Group
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GroupID { get; set; }
        [Display(Name = "Name")]
        [Required]
        public string GroupName { get; set; }
        [Display(Name = "Make group private?")]
        public bool IsPrivate { get; set; }
        [Display(Name = "Description")]
        public string GroupDescription { get; set; }

        //Posts associated with this group
        [Display(Name = "Posted to this group")]
        [InverseProperty("Group")]
        public virtual ICollection<Post> Posts { get; set; }
        //Users refers to the user(s) who created/own this group (private group)
        [Display(Name = "Group owners")]
        public virtual ICollection<User> Users { get; set; }
        //Users1 refers to the user(s) who are invited to join this group (private group)
        [Display(Name = "Invited users")]
        public virtual ICollection<User> Users1 { get; set; }
        //Users2 refers to the user(s) who are subscribed to this group
        [Display(Name = "Subscribed Users")]
        public virtual ICollection<User> Users2 { get; set; }

        [NotMapped]
        public int PostCount
        {
            get
            {
                if (Posts != null)
                {
                    return Posts.Count;
                }
                else
                    return 0;
            }
        }

        [NotMapped]
        public int SubscriberCount
        {
            get
            {
                if (Users2 != null)
                {
                    return Users2.Count;
                }
                else
                    return 0;
            }
        }
    }

    public class User
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        public System.DateTime JoinDate { get; set; }
        public string ProfileDescription { get; set; }
        [Required]
        [ForeignKey("UserProfile")]
        public int UserProfileId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        // Posts refers to posts created by this user
        [Display(Name = "Your Posts")]
        public virtual ICollection<Post> Posts { get; set; }
        // Groups refers to groups created by this user
        [Display(Name = "Your Groups")]
        public virtual ICollection<Group> Groups { get; set; }
        // Posts1 refers to posts blocked by this user
        [Display(Name = "Your Blocked Posts")]
        public virtual ICollection<Post> Posts1 { get; set; }
        // Groups1 refers to private groups this user is allowed to subscribe to
        [Display(Name = "Groups that have invited you")]
        public virtual ICollection<Group> Groups1 { get; set; }
        // Groups2 refers to groups this user is subscribed to
        [Display(Name = "Your subscriptions")]
        public virtual ICollection<Group> Groups2 { get; set; }
        // User1 refers to blocked users; users are blocking you
        [Display(Name = "Blocked Users")]
        public virtual ICollection<User> User1 { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    public class Location
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LocationID { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string LocationName { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }


    public class PostLocationModel
    {
        public Post Tpost { get; set; }
        public Location Tlocation { get; set; }

    }

    // Model for _PopularGroups
    // Find most popular groups and check subscriptions of current user
    public class PopularGroupsModel
    {
        public virtual ICollection<Group> Groups { get; set; }
        public virtual User CurrentUser { get; set; }
    }

    // Model for _SearchGroups
    // Search on search term, to retrieve groups.
    public class SearchGroupsModel
    {
        public virtual ICollection<Group> Groups { get; set; }
        [MinLength(3, ErrorMessage = "Search term must at least 3 characters.")]
        public string SearchTerm { get; set; }
    }

    // Model for _ViewGroupData
    // Showing information for a group based on the current user.
    public class ViewGroupDataModel
    {
        public virtual User CurrentUser { get; set; }
        public virtual Group Group { get; set; }
    }

    // Model for _EditGroupData
    public class EditGroupDataModel
    {
        public virtual Group Group { get; set; }
        //used to scroll through group posts
        public int Offset { get; set; }
        //used when deleting a post by an id
        [Range(0, int.MaxValue, ErrorMessage = "Post ID must be greater than 0")]
        public int DeletingPostId { get; set; }
    }

    // Model for _ViewYourGroups
    // Shows subscriptions, owenrships and invitations for the current user
    public class ViewYourGroupsModel
    {
        public virtual ICollection<Group> Subscriptions { get; set; }
        public virtual ICollection<Group> Ownerships { get; set; }
        public virtual ICollection<Group> Invitations { get; set; }
        public virtual User CurrentUser { get; set; }

    }

    // Model for _ViewProfile
    // Shows profile information for the current user including posts written,
    // users blocked, and blocked posts.
    public class ProfileModel
    {
        public virtual User CurrentUser { get; set; }
        public virtual ICollection<Post> WrittenPosts { get; set; }
        public virtual ICollection<User> Blocks { get; set; }
        public virtual ICollection<Post> BlockedPosts { get; set; }
        public int Offset { get; set; }
        public string UserToBlock { get; set; }
    }


}