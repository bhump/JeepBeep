using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using MyJeepTrader.Data.Models;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

namespace MyJeepTrader.Data
{
    public class Service
    {

        private readonly dboMyJeepTraderEntities _context;

        public Service()
        {
            _context = new dboMyJeepTraderEntities();
        }

        #region Posts
        public IEnumerable<tPost> GetAllPosts()
        {
            return (from p in _context.tPosts select p).ToList();
        }

        public IEnumerable<tPost> SearchAllPosts(params string[] keywords)
        {
            var predicate = PredicateBuilder.False<tPost>();

            foreach (string keyword in keywords)
            {
                string temp = keyword;
                predicate = predicate.Or(p => p.PostTitle.Contains(temp));
                predicate = predicate.Or(p => p.PostDescription.Contains(temp));
                predicate = predicate.Or(p => p.PartBrand.Contains(temp));
                predicate = predicate.Or(p => p.PartType.Contains(temp));
                predicate = predicate.Or(p => p.AspNetUser.UserName.Contains(temp));
                predicate = predicate.Or(p => p.tMake.Make.Contains(temp));
                predicate = predicate.Or(p => p.tPostType.Type.Contains(temp));
                predicate = predicate.Or(p => p.tYear.Year.Contains(temp));
                predicate = predicate.Or(p => p.tModelPostControls.Any(m => m.tModel.Model == temp));
                predicate = predicate.Or(p => p.AspNetUser.UserName.Contains(temp));
            }

            return _context.tPosts.AsExpandable().Where(predicate);
        }

        public SearchedPosts GetPostByPostId(int postId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var post = (from p in context.tPosts
                            where p.PostId == postId
                            select new SearchedPosts
                                {
                                    PostTitle = p.PostTitle,
                                    PostDescription = p.PostDescription,
                                    PartBrand = p.PartBrand,
                                    PartType = p.PartType,
                                    PartName = p.PartName,
                                    Active = p.Active,
                                    UserName = p.AspNetUser.UserName,
                                    Year = p.tYear.Year,
                                    DateCreated = p.DateCreated,
                                    PostType = p.tPostType.Type,
                                    State = p.tState.StateCode,
                                    City = p.tCity.City,
                                    Make = p.tMake.Make
                                }).First();

                return post;
            }
        }

        public List<byte[]> GetPostImages(int postId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var images = (from i in context.tImages where i.PostId == postId select i.Image).ToList();

                return images;
            }
        }

        public int CreateNewPost(tPost post)
        {
            try
            {
                dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();
                post.DateCreated = DateTime.Now;
                post.ViewCount = 0;
                context.tPosts.Add(post);
                context.SaveChanges();

                return post.PostId;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                return 0;
            }
        }

        public void AddImage(byte[] imageData, int newPostId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();
            tImage imageToInsert = new tImage();
            imageToInsert.Image = imageData;
            imageToInsert.PostId = newPostId;
            context.tImages.Add(imageToInsert);
            context.SaveChanges();
        }

        public List<UsersPosts> GetAllPostsForUser(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var result = (from p in context.tPosts
                              join pt in context.tPostTypes on p.PostTypeId equals pt.PostTypeId
                              join u in context.tUserProfiles on p.Id equals u.Id
                              where p.AspNetUser.UserName == userName
                              select new UsersPosts
                              {
                                  PostId = p.PostId,
                                  PostDescription = p.PostDescription,
                                  Active = p.Active,
                                  IsVehicle = p.IsVehicle,
                                  PostTitle = p.PostTitle,
                                  PostType = pt.Type,
                                  DateCreated = p.DateCreated,
                                  UserName = p.AspNetUser.UserName,
                                  Avatar = u.Avatar
                              }).ToList();

                return result;
            }
        }

        public UsersPosts GetUsersMostRecentPost(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var result = (from p in context.tPosts
                              join pt in context.tPostTypes on p.PostTypeId equals pt.PostTypeId
                              where p.AspNetUser.UserName == userName
                              select new UsersPosts
                              {
                                  PostId = p.PostId,
                                  PostDescription = p.PostDescription,
                                  Active = p.Active,
                                  IsVehicle = p.IsVehicle,
                                  PostTitle = p.PostTitle,
                                  PostType = pt.Type,
                                  DateCreated = p.DateCreated
                              }).OrderByDescending(p => p.DateCreated).FirstOrDefault();

                return result;
            }
        }

        public List<tPostType> GetAllPostTypes()
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();
            return (from p in context.tPostTypes select p).ToList();
        }

        public void AddModelPost(int modelId, int newPostId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();
            tModelPostControl entity = new tModelPostControl
            {
                ModelId = modelId,
                PostId = newPostId
            };

            context.tModelPostControls.Add(entity);
            context.SaveChanges();

        }

        public List<tState> GetAllStates()
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();

            return (from s in context.tStates select s).ToList();
        }

        public List<CityDropDown> GetCityByStateId(int stateId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();

            var cities = context.tCities.Where(c => c.StateId == stateId).Select(c => new CityDropDown { CityId = c.CityId, City = c.City }).OrderBy(c => c.City).ToList();

            return cities;
        }

        public string GetStateById(int stateId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();

            var state = (from s in context.tStates where s.StateId == stateId select s.State).First();

            return state;
        }

        public string GetYearById(int yearId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();

            var year = (from y in context.tYears where y.YearId == yearId select y.Year).First();

            return year;
        }

        public string GetModelById(int modelId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();

            var model = (from m in context.tModels where m.ModelId == modelId select m.Model).First();

            return model;
        }

        public long UpdatePostViewCount(int postId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var post = (from p in context.tPosts where p.PostId == postId select p).FirstOrDefault();
                post.ViewCount = post.ViewCount + 1;
                context.SaveChanges();

                return post.ViewCount;
            }
        }

        public List<UsersPosts> GetPopularUserPost(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var result = (from p in context.tPosts
                              join pt in context.tPostTypes on p.PostTypeId equals pt.PostTypeId
                              join up in context.tUserProfiles on p.Id equals up.Id
                              where p.ViewCount > 10 && p.Id != userId
                              select new UsersPosts
                              {
                                  Avatar = up.Avatar,
                                  UserName = p.AspNetUser.UserName,
                                  PostId = p.PostId,
                                  PostDescription = p.PostDescription,
                                  Active = p.Active,
                                  IsVehicle = p.IsVehicle,
                                  PostTitle = p.PostTitle,
                                  PostType = pt.Type,
                                  DateCreated = p.DateCreated
                              }).OrderByDescending(p => p.DateCreated).ToList();

                return result;
            }
        }

        public List<UsersPosts> GetPopularUserPost()
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var result = (from p in context.tPosts
                              join pt in context.tPostTypes on p.PostTypeId equals pt.PostTypeId
                              join up in context.tUserProfiles on p.Id equals up.Id
                              where p.ViewCount > 10
                              select new UsersPosts
                              {
                                  Avatar = up.Avatar,
                                  UserName = p.AspNetUser.UserName,
                                  PostId = p.PostId,
                                  PostDescription = p.PostDescription,
                                  Active = p.Active,
                                  IsVehicle = p.IsVehicle,
                                  PostTitle = p.PostTitle,
                                  PostType = pt.Type,
                                  DateCreated = p.DateCreated,
                                  Year = p.tYear.Year,
                                  City = p.tCity.City,
                                  State = p.tState.State,
                                  Model = p.tModelPostControls.Select(m => m.tModel.Model).ToList(),
                                  PartType = p.PartType
                              }).OrderByDescending(p => p.DateCreated).Take(3).ToList();

                return result;
            }
        }

        #endregion

        #region Timeline
        public ICollection<LiveStream> GetLiveStream(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var privateSettings = GetSettings(userId);
                var statusList = new List<LiveStream>();
                var ownStatusList = new List<LiveStream>();
                var imagesList = new List<byte[]>();
                var ownImageList = new List<byte[]>();

                statusList = (from fl in context.tFriendsLists
                              join su in context.tStatusUpdates on fl.FriendId equals su.Id
                              join up in context.tUserProfiles on su.Id equals up.Id
                              where fl.Id == userId && fl.Accepted == true && fl.Block == false
                              select new LiveStream
                                  {
                                      Status = su.Status,
                                      UserName = su.AspNetUser.UserName,
                                      UserId = su.AspNetUser.Id,
                                      IsPost = false,
                                      StatusId = su.StatusId,
                                      Avatar = up.Avatar,
                                      DateCreated = su.DateCreated,
                                      LikeCount = su.LikeCount,
                                      DislikeCount = su.DislikeCount,
                                  }).ToList();

                ownStatusList = (from su in context.tStatusUpdates
                                 join up in context.tUserProfiles on su.Id equals up.Id
                                 where su.Id == userId
                                 select new LiveStream
                                 {
                                     Status = su.Status,
                                     UserName = su.AspNetUser.UserName,
                                     UserId = su.AspNetUser.Id,
                                     IsPost = false,
                                     StatusId = su.StatusId,
                                     Avatar = up.Avatar,
                                     DateCreated = su.DateCreated,
                                     LikeCount = su.LikeCount,
                                     DislikeCount = su.DislikeCount,
                                 }).ToList();

                foreach (var own in ownStatusList)
                {
                    var commentCount = GetCommentCount(own.StatusId);
                    own.CommentCount = commentCount;
                    statusList.Add(own);
                }

                foreach (var status in statusList)
                {
                    var commentCount = GetCommentCount(status.StatusId);
                    status.CommentCount = commentCount;
                    if (context.tStatusMedias.Where(x => x.StatusId == status.StatusId).Count() != 0)
                    {
                        imagesList = (from i in context.tStatusMedias where i.StatusId == status.StatusId select i.Image).ToList();
                        status.Images = imagesList;
                    }
                }

                return statusList.OrderByDescending(su => su.DateCreated).ToList();
            }
        }

        public ICollection<LiveStream> GetPublicStream()
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {

                var publicStatus = (from su in context.tStatusUpdates
                                    join s in context.tSettings on su.Id equals s.Id
                                    join up in context.tUserProfiles on su.Id equals up.Id
                                    where s.PrivateStatus == false
                                    select new LiveStream
                                    {
                                        Status = su.Status,
                                        UserName = su.AspNetUser.UserName,
                                        UserId = su.AspNetUser.Id,
                                        IsPost = false,
                                        StatusId = su.StatusId,
                                        Avatar = up.Avatar,
                                        DateCreated = su.DateCreated,
                                        LikeCount = su.LikeCount,
                                        DislikeCount = su.DislikeCount
                                    }).OrderByDescending(su => su.DateCreated).ToList();

                foreach (var status in publicStatus)
                {
                    if (context.tStatusMedias.Where(x => x.StatusId == status.StatusId).Count() != 0)
                    {
                        var imagesList = (from i in context.tStatusMedias where i.StatusId == status.StatusId select i.Image).ToList();
                        status.Images = imagesList;
                    }
                }

                return publicStatus;
            }
        }

        public ICollection<LivePost> GetLivePosts()
        {
            var images = new List<byte[]>();

            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var posts = (from p in context.tPosts
                             join up in context.tUserProfiles on p.Id equals up.Id
                             select new LivePost
                                 {
                                     UserName = p.AspNetUser.UserName,
                                     UserId = p.AspNetUser.Id,
                                     PostDescription = p.PostDescription,
                                     IsPost = true,
                                     PostId = p.PostId,
                                     Avatar = up.Avatar,
                                     DateCreated = p.DateCreated,
                                     LikeCount = 0,
                                     DislikeCount = 0
                                 }).OrderByDescending(p => p.DateCreated).ToList();

                foreach (var post in posts)
                {
                    if (context.tImages.Where(x => x.PostId == post.PostId).Count() != 0)
                    {
                        images = (from i in context.tImages where i.PostId == post.PostId select i.Image).ToList();
                        post.Images = images;
                    }
                }

                return posts;
            }
        }

        public async Task<int> CreateStatusAsync(string userId, string status)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                tStatusUpdate statusUpdate = new tStatusUpdate
                {
                    DateCreated = DateTime.Now,
                    LikeCount = 0,
                    DislikeCount = 0,
                    Status = status,
                    Id = userId
                };
                context.tStatusUpdates.Add(statusUpdate);
                context.SaveChanges();

                Task<int> t = new Task<int>(() =>
                {
                    return statusUpdate.StatusId;

                });

                t.Start();
                return await t;
            }
        }

        public List<LiveStream> GetStatusForUser(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var status = (from s in context.tStatusUpdates
                              join u in context.tUserProfiles on s.Id equals u.Id
                              where s.AspNetUser.UserName == userName
                              select new LiveStream
                              {
                                  Status = s.Status,
                                  UserName = s.AspNetUser.UserName,
                                  UserId = s.AspNetUser.Id,
                                  IsPost = false,
                                  StatusId = s.StatusId,
                                  Avatar = u.Avatar,
                                  DateCreated = s.DateCreated,
                                  LikeCount = s.LikeCount,
                                  DislikeCount = s.DislikeCount,
                              }).ToList();

                return status;
            }
        }

        public List<string> BlockedUsers(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var friendId = (from u in context.AspNetUsers where u.UserName == userName select u.Id).First();

                var blockedUsers = (from fl in context.tFriendsLists
                                    join s in context.tSettings on fl.AspNetUser.UserName equals s.AspNetUser.UserName
                                    where fl.FriendId == friendId && fl.Block == true
                                    select fl.AspNetUser.UserName).ToList();

                return blockedUsers;
            }
        }

        public List<string> AllowedUsers(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var allowedUsers = new List<string>();

                var friendId = (from u in context.AspNetUsers where u.UserName == userName select u.Id).First();

                allowedUsers = (from fl in context.tFriendsLists
                                join s in context.tSettings on fl.AspNetUser.UserName equals s.AspNetUser.UserName
                                where fl.FriendId == friendId && fl.Accepted == true
                                select fl.AspNetUser.UserName).ToList();

                allowedUsers.Add(userName);

                return allowedUsers;
            }
        }

        public long UpdateLikeCount(int statusId, string userId)
        {
            try
            {
                using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
                {
                    var getLikedBy = (from sc in context.tStatusControls where sc.LikedBy == userId && sc.StatusId == statusId select sc).FirstOrDefault();

                    if (getLikedBy == null)
                    {
                        var status = (from s in context.tStatusUpdates where s.StatusId == statusId select s).First();
                        status.LikeCount = status.LikeCount + 1;

                        tStatusControl statusControl = new tStatusControl
                        {
                            StatusId = statusId,
                            LikedBy = userId,
                            Active = true
                        };

                        context.tStatusControls.Add(statusControl);
                        context.SaveChanges();

                        if (context.tStatusUpdates.Where(s => s.StatusId == statusId).Select(u => u.Id).FirstOrDefault() != userId)
                            CreateNotification(userId, "0", 0, 0, 0, statusControl.StatusControlId, 0);

                        return status.LikeCount;
                    }
                    else if (getLikedBy.Active == false)
                    {
                        var status = (from s in context.tStatusUpdates where s.StatusId == statusId select s).First();
                        status.LikeCount = status.LikeCount + 1;

                        var toActivate = (from sc in context.tStatusControls where sc.LikedBy == userId && sc.StatusId == statusId && sc.Active == false select sc).First();
                        toActivate.Active = true;
                        context.SaveChanges();

                        if (context.tStatusUpdates.Where(s => s.StatusId == statusId).Select(u => u.Id).FirstOrDefault() != userId)
                            CreateNotification(userId, "0", 0, 0, 0, toActivate.StatusControlId, 0);

                        return status.LikeCount;
                    }
                    else
                    {
                        var status = (from s in context.tStatusUpdates where s.StatusId == statusId select s).First();
                        status.LikeCount = status.LikeCount - 1;

                        var toDelete = (from sc in context.tStatusControls where sc.LikedBy == userId && sc.StatusId == statusId && sc.Active == true select sc).First();
                        toDelete.Active = false;
                        context.SaveChanges();

                        return status.LikeCount;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                return 0;
            }
        }

        public long UpdateDislikeCount(int statusId, string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var getDislikedBy = (from sc in context.tStatusControls where sc.DisLikedBy == userId && sc.StatusId == statusId select sc).FirstOrDefault();

                if (getDislikedBy == null)
                {
                    var status = (from s in context.tStatusUpdates where s.StatusId == statusId select s).First();
                    status.DislikeCount = status.DislikeCount + 1;

                    tStatusControl statusControl = new tStatusControl
                    {
                        StatusId = statusId,
                        DisLikedBy = userId,
                        Active = true
                    };

                    context.tStatusControls.Add(statusControl);
                    context.SaveChanges();

                    if (context.tStatusUpdates.Where(s => s.StatusId == statusId).Select(u => u.Id).FirstOrDefault() != userId)
                        CreateNotification(userId, "0", 0, 0, 0, statusControl.StatusControlId, 0);

                    return status.DislikeCount;
                }
                else if (getDislikedBy.Active == false)
                {
                    var status = (from s in context.tStatusUpdates where s.StatusId == statusId select s).First();
                    status.DislikeCount = status.DislikeCount + 1;

                    var toActivate = (from sc in context.tStatusControls where sc.DisLikedBy == userId && sc.StatusId == statusId && sc.Active == false select sc).First();
                    toActivate.Active = true;

                    if (context.tStatusUpdates.Where(s => s.StatusId == statusId).Select(u => u.Id).FirstOrDefault() != userId)
                        CreateNotification(userId, "0", 0, 0, 0, toActivate.StatusControlId, 0);

                    context.SaveChanges();

                    return status.DislikeCount;
                }
                else
                {
                    var status = (from s in context.tStatusUpdates where s.StatusId == statusId select s).First();
                    status.DislikeCount = status.DislikeCount - 1;

                    var toDelete = (from sc in context.tStatusControls where sc.DisLikedBy == userId && sc.StatusId == statusId && sc.Active == true select sc).First();
                    toDelete.Active = false;
                    context.SaveChanges();

                    return status.DislikeCount;
                }
            }
        }

        public List<Comments> GetCommentsForStatus(int statusId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var comments = (from c in context.tStatusComments
                                join u in context.AspNetUsers on c.Id equals u.Id
                                where c.StatusId == statusId
                                select new Comments
                                {
                                    CommentId = c.CommentId,
                                    Comment = c.Comment,
                                    UserName = u.UserName,
                                    UserId = u.Id,
                                    DateCreated = c.DateCreated
                                }).OrderByDescending(c => c.DateCreated).ToList();

                return comments;
            }
        }

        public long GetCommentCount(int statusId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var count = (from c in context.tStatusComments where c.StatusId == statusId select c).Count();

                return count;
            }
        }

        public tStatusComment CreateComment(string userId, string statusId, string comment)
        {
            try
            {
                var sId = Convert.ToInt32(statusId);

                using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
                {
                    var newComment = new tStatusComment();
                    newComment.Id = userId;
                    newComment.Comment = comment;
                    newComment.StatusId = sId;
                    newComment.DateCreated = DateTime.Now;

                    context.tStatusComments.Add(newComment);
                    context.SaveChanges();


                    string mentionPattern = @"@\w* ";
                    MatchCollection matches = Regex.Matches(comment, mentionPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    if (matches.Count != 0)
                    {
                        foreach (Match match in matches)
                        {
                            var mentionedUserName = match.ToString().Replace("@", "");
                            CreateMention(mentionedUserName, userId, 0, newComment.CommentId);
                        }
                    }
                    else
                    {
                        CreateNotification(userId, "0", newComment.CommentId, 0, 0, 0, 0);
                    }

                    return newComment;
                }
            }
            catch (Exception ex)
            {
                var failedComment = new tStatusComment();
                failedComment.StatusId = 0;
                failedComment.Comment = "Could not post comment!";

                Console.WriteLine(ex);

                return failedComment;
            }
        }

        public void AddStatusImage(byte[] imageData, int newStatusId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();
            tStatusMedia imageToInsert = new tStatusMedia();
            imageToInsert.Image = imageData;
            imageToInsert.StatusId = newStatusId;
            context.tStatusMedias.Add(imageToInsert);
            context.SaveChanges();
        }

        public List<UserProfiles> GetPopularProfiles(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var max = (from u in context.tUserProfiles
                           where u.ViewCount > 10 && u.Id != userId
                           select new UserProfiles
                           {
                               Avatar = u.Avatar,
                               UserName = u.AspNetUser.UserName,
                               Description = u.Description
                           }).Take(3).ToList();

                return max;
            }
        }

        public int CreateMention(string mentionedUserName, string mentionedByUserId, int statusId, int commentId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var mentionedUserdId = (from u in context.AspNetUsers where u.UserName == mentionedUserName select u.Id).First();

                tMention mention = new tMention
                {
                    MentionedUserId = mentionedUserdId,
                    MentionedByUserId = mentionedByUserId,
                    StatusId = (statusId == 0) ? (int?)null : statusId,
                    CommentId = (commentId == 0) ? (int?)null : commentId,
                    DateCreated = DateTime.Now
                };

                context.tMentions.Add(mention);
                context.SaveChanges();

                CreateNotification(mentionedByUserId, "0", 0, 0, 0, 0, mention.MentionId);

                return mention.MentionId;
            }
        }

        public List<tMention> MentionsByUserId(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var mentions = (from m in context.tMentions
                                join u in context.AspNetUsers on m.MentionedByUserId equals u.Id
                                where m.MentionedUserId == userId
                                select m).ToList();

                return mentions;
            }
        }

        public List<AutoCompleteUsers> GetAutoCompleteUsers()
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var usernames = (from u in context.AspNetUsers
                                 join p in context.tUserProfiles on u.Id equals p.Id
                                 select new AutoCompleteUsers
                                 {
                                     UserName = u.UserName
                                 }).ToList();

                return usernames;
            }
        }
        #endregion

        #region Membership
        public tMembership GetMembership(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var membership = context.tMemberships.Where(m => m.AspNetUser.UserName == userName).FirstOrDefault();

                return membership;
            }
        }

        public tSubscription GetSubscription(int membershipId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var subscription = context.tSubscriptions.Where(s => s.MembershipId == membershipId && s.Active == true).First();

                return subscription;
            }
        }

        public string GetSubscriptionType(int subscriptionTypeId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var subType = context.tSubscriptionTypes.Where(t => t.SubscriptionTypeId == subscriptionTypeId).First();

                return subType.SubscriptionType;
            }
        }

        public void CreateMembership(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                tMembership membership = new tMembership
                    {
                        Id = userId,
                        Active = true,
                        MemberSince = DateTime.Now.Date
                    };
                context.tMemberships.Add(membership);
                context.SaveChanges();
            }
        }

        public void CreateSubscription(int membershipId, DateTime startDate, DateTime expireDate, string subType, string payPalSubscriptionId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                tSubscription subscription = new tSubscription
                {
                    MembershipId = membershipId,
                    Active = true,
                    AutoRenew = true,
                    Expired = false,
                    ExpireDate = expireDate,
                    StartDate = startDate,
                    SubscriptionTypeId = context.tSubscriptionTypes.Where(s => s.SubscriptionType == subType).Select(s => s.SubscriptionTypeId).FirstOrDefault(),
                    PayPalSubscriptionId = payPalSubscriptionId
                };

                context.tSubscriptions.Add(subscription);
                context.SaveChanges();
            }
        }

        public void InactivateSubscription(int subscriptionId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var inactive = context.tSubscriptions.Where(s => s.SubscriptionId == subscriptionId).FirstOrDefault();
                inactive.Active = false;
                context.SaveChanges();
            }
        }

        public void UpdateMembership(string customerId, string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var updateMembership = context.tMemberships.Where(m => m.Id == userId).FirstOrDefault();
                updateMembership.PayPalCustomerId = customerId;

                context.SaveChanges();
            }
        }

        public string GetCustomer(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var customerId = context.tMemberships.Where(c => c.Id == userId).Select(c => c.PayPalCustomerId).First();

                return customerId;
            }
        }
        #endregion

        #region User Profile
        public List<UserProfiles> GetAllUserProfiles()
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var result = (from up in context.tUserProfiles
                              select new UserProfiles
                              {
                                  UserProfileId = up.UserProfileId,
                                  FirstName = up.FirstName,
                                  LastName = up.LastName,
                                  BirthDate = up.BirthDate,
                                  Avatar = up.Avatar,
                                  Description = up.Description,
                                  Facebook = up.Facebook,
                                  Twitter = up.Twitter,
                                  Ello = up.Ello,
                                  GooglePlus = up.GooglePlus,
                                  Website = up.Website,
                                  UserName = up.AspNetUser.UserName,
                                  Instagram = up.Instagram
                              }).ToList();

                return result;
            }
        }

        public byte[] GetAvatarImage(string userName)
        {
            using (_context)
            {
                var avatar = (from up in _context.tUserProfiles
                              where up.AspNetUser.UserName == userName
                              select up.Avatar).FirstOrDefault();

                return avatar;
            }
        }

        public void CreateProfile(string userId, string firstName, string lastName, DateTime birthDate, byte[] avatar, string description, string facebook, string twitter, string ello, string google, string website, string instagram)
        {
            try
            {

                using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
                {
                    tUserProfile userProfile = new tUserProfile
                    {
                        Id = userId,
                        FirstName = firstName,
                        LastName = lastName,
                        BirthDate = birthDate,
                        Avatar = avatar,
                        Description = description,
                        Facebook = facebook,
                        Twitter = twitter,
                        Ello = ello,
                        GooglePlus = google,
                        Website = website,
                        Instagram = instagram
                    };
                    context.tUserProfiles.Add(userProfile);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void CreateProfile(string userId, byte[] avatar)
        {
            try
            {
                using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
                {
                    tUserProfile userProfile = new tUserProfile
                    {
                        Id = userId,
                        Avatar = avatar,
                        ViewCount = 0
                    };
                    context.tUserProfiles.Add(userProfile);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void UpdateProfile(string userId, string firstName, string lastName, DateTime birthDate, byte[] avatar, string description, string facebook, string twitter, string ello, string google, string website, string instagram)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var updateProfile = context.tUserProfiles.Where(up => up.Id == userId).FirstOrDefault();
                updateProfile.FirstName = firstName;
                updateProfile.LastName = lastName;
                updateProfile.BirthDate = birthDate;
                updateProfile.Description = description;
                updateProfile.Facebook = facebook;
                updateProfile.Twitter = twitter;
                updateProfile.Ello = ello;
                updateProfile.GooglePlus = google;
                updateProfile.Website = website;
                updateProfile.Avatar = avatar;
                updateProfile.Instagram = instagram;

                context.SaveChanges();
            }
        }

        public bool CheckForProfile(string userId)
        {
            using (_context)
            {
                var noProfile = (from up in _context.tUserProfiles where up.Id == userId select up).Any();

                return noProfile;
            }
        }

        public tUserProfile GetProfileInfo(string userId)
        {
            using (_context)
            {
                return _context.tUserProfiles.Where(up => up.Id == userId).FirstOrDefault();
            }
        }

        public tUserProfile GetProfileInfoByUserName(string userName)
        {
            using (_context)
            {
                return _context.tUserProfiles.Where(up => up.AspNetUser.UserName == userName).First();
            }
        }

        public long? GetViewCount(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var count = (from up in context.tUserProfiles where up.AspNetUser.UserName == userName select up.ViewCount).FirstOrDefault();

                return count;
            }
        }

        public void UpdateViewCount(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var updateProfile = context.tUserProfiles.Where(up => up.AspNetUser.UserName == userName).FirstOrDefault();
                updateProfile.ViewCount = GetViewCount(userName) + 1;
                context.SaveChanges();
            }
        }
        #endregion

        #region Jeep Profile
        public byte[] GetJeepImage(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var image = (from vp in context.tVehicleProfiles
                             join vpc in context.tVehicleProfileControls on vp.VehicleProfileId equals vpc.VehicleProfileId
                             where vpc.AspNetUser.UserName == userName
                             select vp.Image).FirstOrDefault();

                return image;
            }
        }

        public byte[] GetJeepImageByJeepProfileId(int jeepProfileId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var image = (from vp in context.tVehicleProfiles
                             join vpc in context.tVehicleProfileControls on vp.VehicleProfileId equals vpc.VehicleProfileId
                             where vp.VehicleProfileId == jeepProfileId
                             select vp.Image).FirstOrDefault();

                return image;
            }
        }

        public void CreateJeepProfile(string userId, string manufactuer, string make, string model, short year, byte[] jeepImage, string jeepDescription, bool primaryJeep)
        {
            using (_context)
            {
                tVehicleProfile jeep = new tVehicleProfile
                {
                    Manufacturer = manufactuer,
                    Make = make,
                    Model = model,
                    Year = year,
                    Image = jeepImage,
                    Description = jeepDescription,
                    PrimaryJeep = primaryJeep
                };

                tVehicleProfileControl jeepControl = new tVehicleProfileControl
                {
                    VehicleProfileId = jeep.VehicleProfileId,
                    Id = userId
                };

                _context.tVehicleProfiles.Add(jeep);
                _context.tVehicleProfileControls.Add(jeepControl);
                _context.SaveChanges();
            }
        }

        public void UpdatePrimaryJeepProfile(string userId, string manufactuer, string make, string model, short year, byte[] jeepImage, string jeepDescription, bool primaryJeep)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var updateJeepProfile = (from vp in context.tVehicleProfiles
                                         join vpc in context.tVehicleProfileControls on vp.VehicleProfileId equals vpc.VehicleProfileId
                                         where vpc.Id == userId
                                         && vp.PrimaryJeep == true
                                         select vp).FirstOrDefault();

                updateJeepProfile.Manufacturer = manufactuer;
                updateJeepProfile.Make = make;
                updateJeepProfile.Model = model;
                updateJeepProfile.Year = year;
                updateJeepProfile.Image = jeepImage;
                updateJeepProfile.Description = jeepDescription;
                updateJeepProfile.PrimaryJeep = primaryJeep;

                context.SaveChanges();
            }
        }

        public void ChangePrimaryJeep(string userId, bool primaryJeep)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var changePrimary = (from vp in context.tVehicleProfiles
                                     join vpc in context.tVehicleProfileControls on vp.VehicleProfileId equals vpc.VehicleProfileId
                                     where vpc.Id == userId
                                     && vp.PrimaryJeep == true
                                     select vp).FirstOrDefault();

                changePrimary.PrimaryJeep = primaryJeep;
                context.SaveChanges();
            }
        }

        public tVehicleProfile GetPrimaryJeepInfo(string UserName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                return (from vp in context.tVehicleProfiles
                        join vpc in context.tVehicleProfileControls on vp.VehicleProfileId equals vpc.VehicleProfileId
                        where vpc.AspNetUser.UserName == UserName
                        select vp).FirstOrDefault();
            }

        }

        public tVehicleProfile GetPrimaryJeepInfoByProfileId(int jeepProfileId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                return (from vp in context.tVehicleProfiles
                        join vpc in context.tVehicleProfileControls on vp.VehicleProfileId equals vpc.VehicleProfileId
                        where vpc.VehicleProfileId == jeepProfileId
                        select vp).FirstOrDefault();
            }
        }

        public bool CheckForPrimaryJeep(string userId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();

            var noPrimaryJeep = (from vp in context.tVehicleProfiles
                                 join vpc in context.tVehicleProfileControls on vp.VehicleProfileId equals vpc.VehicleProfileId
                                 where vpc.Id == userId && vp.PrimaryJeep == true
                                 select vp).Any();

            return noPrimaryJeep;
        }

        public List<JeepProfiles> GetAllJeepProfiles(string userId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();

            var allJeeps = (from vp in context.tVehicleProfiles
                            join vpc in context.tVehicleProfileControls on vp.VehicleProfileId equals vpc.VehicleProfileId
                            where vpc.Id == userId
                            select new JeepProfiles
                            {
                                JeepDescription = vp.Description,
                                Make = vp.Make,
                                Model = vp.Model,
                                Image = vp.Image,
                                Manufactuer = vp.Manufacturer,
                                JeepProfileId = vp.VehicleProfileId,
                                PrimaryJeep = vp.PrimaryJeep == true ? true : false,
                                Year = vp.Year.Value
                            }).ToList();

            return allJeeps;
        }
        #endregion

        #region Vehicle Details
        public List<tModel> GetAllModels()
        {
            using (_context)
            {
                return (from m in _context.tModels select m).ToList();
            }
        }

        public List<JeepModels> GetAllJeepModels()
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var models = (from m in context.tModels
                              select new JeepModels
                                  {
                                      Model = m.Model,
                                      ModelId = m.ModelId,
                                      Selected = true
                                  }).ToList();

                return models;
            }
        }

        public List<tYear> GetAllYears()
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();

            return (from y in context.tYears select y).ToList();
        }
        #endregion

        #region Mailbox And Instant Chat
        public int CreateMessage(string toUser, string fromUser, string subject, string message, bool isIm)
        {
            using (_context)
            {
                string toUserId = (from u in _context.AspNetUsers where u.UserName == toUser select u.Id).FirstOrDefault();
                string fromUserId = (from u in _context.AspNetUsers where u.UserName == fromUser select u.Id).FirstOrDefault();

                tMessage mess = new tMessage
                {
                    Subject = subject,
                    Message = message,
                    DateSent = DateTime.Now,
                    IsIM = isIm
                };

                tMessageControl messControl = new tMessageControl
                {
                    ToUserId = toUserId,
                    FromUserId = fromUserId,
                    MessageId = mess.MessageId
                };

                _context.tMessages.Add(mess);
                _context.tMessageControls.Add(messControl);
                _context.SaveChanges();

                CreateNotification(fromUserId, "0", 0, mess.MessageId, 0, 0, 0);


                return mess.MessageId;
            }
        }

        public List<MailMessages> GetSentMessages(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var sentMessages = (from m in context.tMessages
                                    join mc in context.tMessageControls on m.MessageId equals mc.MessageId
                                    join u in context.AspNetUsers on mc.ToUserId equals u.Id
                                    where mc.FromUserId == userId && m.IsIM == false
                                    orderby m.DateSent descending
                                    select new MailMessages
                                    {
                                        Subject = m.Subject,
                                        Message = m.Message,
                                        DateSent = m.DateSent,
                                        DateRead = m.DateRead,
                                        MessageId = m.MessageId,
                                        FromUserId = mc.FromUserId,
                                        To = u.UserName,
                                        ToUserId = mc.ToUserId
                                    }).ToList();

                return sentMessages;
            }
        }

        public List<MailMessages> GetInboxMessages(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var inbox = (from m in context.tMessages
                             join mc in context.tMessageControls on m.MessageId equals mc.MessageId
                             join u in context.AspNetUsers on mc.FromUserId equals u.Id
                             where mc.ToUserId == userId && m.IsIM == false
                             orderby m.DateSent descending
                             select new MailMessages
                             {
                                 Subject = m.Subject,
                                 Message = m.Message,
                                 DateSent = m.DateSent,
                                 DateRead = m.DateRead,
                                 MessageId = m.MessageId,
                                 From = u.UserName,
                                 ToUserId = mc.ToUserId,
                                 FromUserId = mc.FromUserId
                             }).ToList();

                return inbox;
            }
        }

        public void MarkMessageAsRead(int messageId)
        {
            using (_context)
            {
                var getMessage = _context.tMessages.Where(m => m.MessageId == messageId).FirstOrDefault();
                getMessage.DateRead = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void AddConnectedUser(string connectedId, string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var connection = new tConnectedUser();
                connection.ConnectionId = connectedId;
                connection.ConnectionUserName = userName;

                context.tConnectedUsers.Add(connection);
                context.SaveChanges();
            }
        }

        public void RemoveConnectedUser(string connectedId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var delete = context.tConnectedUsers.Where(cu => cu.ConnectionId == connectedId).FirstOrDefault();
                context.Entry(delete).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
        #endregion

        #region Friends/Follow
        public int AddFriend(string userId, string friendId)
        {
            try
            {
                using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
                {
                    tFriendsList friend = new tFriendsList
                    {
                        Id = userId,
                        FriendId = friendId,
                        Pending = true,
                        Accepted = false
                    };
                    context.tFriendsLists.Add(friend);
                    context.SaveChanges();

                    CreateNotification(userId, "0", 0, 0, friend.FriendListId, 0, 0);

                    return friend.FriendListId;
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                return 0;
            }
        }

        public int AddFriend(string userId, string userName, bool fromDetails)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var friendId = context.AspNetUsers.Where(u => u.UserName == userName).Select(u => u.Id).First();

                tFriendsList friend = new tFriendsList
                {
                    Id = userId,
                    FriendId = friendId,
                    Pending = true,
                    Accepted = false
                };
                context.tFriendsLists.Add(friend);
                context.SaveChanges();

                CreateNotification(userId, "0", 0, 0, friend.FriendListId, 0, 0);

                return friend.FriendListId;
            }
        }

        public string AcceptFriend(string friendsListId)
        {
            var id = Convert.ToInt32(friendsListId);

            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var acceptFriend = (from f in context.tFriendsLists where f.FriendListId == id select f).First();

                acceptFriend.Pending = false;
                acceptFriend.Accepted = true;
                context.SaveChanges();

                return "Friend Accepted!";
            }

        }

        public string RejectFriend(string friendsListId)
        {
            var id = Convert.ToInt32(friendsListId);

            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var acceptFriend = (from f in context.tFriendsLists where f.FriendListId == id select f).First();

                acceptFriend.Pending = false;
                acceptFriend.Accepted = false;
                context.SaveChanges();

                return "Friend Rejected!";
            }
        }

        public string RemoveFriend(string friendId, string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var removeFriend = (from f in context.tFriendsLists where f.FriendId == friendId && f.Id == userId select f).First();

                tFriendsList friendRequestToRemove = removeFriend;

                context.tFriendsLists.Remove(friendRequestToRemove);
                context.SaveChanges();

                return "Friend Request Removed!";
            }
        }

        public string RemoveFriend(string friendsListId)
        {
            var id = Convert.ToInt32(friendsListId);

            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var removeFriend = (from f in context.tFriendsLists where f.FriendListId == id select f).First();

                tFriendsList friendRequestToRemove = removeFriend;

                context.tFriendsLists.Remove(friendRequestToRemove);
                context.SaveChanges();

                return "Friend Request Removed!";
            }
        }

        public string BlockFriend(string friendsListId, string blockFriend)
        {
            var id = Convert.ToInt32(friendsListId);
            var block = Convert.ToBoolean(blockFriend);

            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var blocked = (from f in context.tFriendsLists where f.FriendListId == id select f).First();
                blocked.Block = block;
                blocked.Pending = false;
                blocked.Accepted = false;

                context.SaveChanges();

                if (block == true)
                {
                    return "Friend Blocked!";
                }
                else
                {
                    return "Friend Unblocked!";
                }
            }
        }

        public List<FriendsList> GetFriends(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var following = new List<FriendsList>();

                var friends = (from f in context.tFriendsLists
                               join u in context.AspNetUsers on f.FriendId equals u.Id
                               join up in context.tUserProfiles on u.Id equals up.Id
                               where f.Id == userId && f.Pending == false && f.Accepted == true
                               select new { u, f, up }).ToList();

                foreach (var friend in friends)
                {
                    var friendsList = new FriendsList();

                    friendsList.Email = friend.u.Email;
                    friendsList.UserName = friend.u.UserName;
                    friendsList.FirstName = friend.up.FirstName;
                    friendsList.LastName = friend.up.LastName;
                    friendsList.FriendsListId = friend.f.FriendListId;
                    friendsList.Avatar = friend.up.Avatar;
                    friendsList.Blocked = friend.f.Block;
                    friendsList.UserId = userId;
                    friendsList.FriendId = friend.u.Id;

                    following.Add(friendsList);
                }

                return following;
            }
        }

        public FriendsList GetFriendInfo(string userId, string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var friendId = context.AspNetUsers.Where(f => f.UserName == userName).Select(f => f.Id).FirstOrDefault();

                var info = (from f in context.tFriendsLists
                            where f.Id == userId && f.FriendId == friendId
                            select new FriendsList
                            {
                                FriendsListId = f.FriendListId,
                                Blocked = f.Block,
                                Pending = f.Pending,
                                Accepted = f.Accepted
                            }).FirstOrDefault();

                return info;
            }
        }

        public List<FriendsList> GetPendingFriends(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var pendingFriends = new List<FriendsList>();

                var friends = (from f in context.tFriendsLists
                               join u in context.AspNetUsers on f.Id equals u.Id
                               where f.FriendId == userId && f.Pending == true
                               select new { u, f }).ToList();

                foreach (var friend in friends)
                {
                    var friendsList = new FriendsList();

                    var pending = (from up in context.tUserProfiles
                                   where up.Id == friend.u.Id
                                   select
                                       up).First();

                    friendsList.Email = friend.u.Email;
                    friendsList.UserName = friend.u.UserName;
                    friendsList.FirstName = pending.FirstName;
                    friendsList.LastName = pending.LastName;
                    friendsList.FriendsListId = friend.f.FriendListId;
                    friendsList.Avatar = pending.Avatar;
                    friendsList.Blocked = friend.f.Block;

                    pendingFriends.Add(friendsList);
                }

                return pendingFriends;
            }
        }

        public List<FriendsList> GetFollowerFriends(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var followers = new List<FriendsList>();

                var friends = (from f in context.tFriendsLists
                               join u in context.AspNetUsers on f.Id equals u.Id
                               join up in context.tUserProfiles on u.Id equals up.Id
                               where f.FriendId == userId && f.Pending == false && f.Accepted == true
                               select new { u, f, up }).ToList();

                foreach (var friend in friends)
                {
                    var friendsList = new FriendsList();

                    friendsList.Email = friend.u.Email;
                    friendsList.UserName = friend.u.UserName;
                    friendsList.FirstName = friend.up.FirstName;
                    friendsList.LastName = friend.up.LastName;
                    friendsList.FriendsListId = friend.f.FriendListId;
                    friendsList.Avatar = friend.up.Avatar;
                    friendsList.Blocked = friend.f.Block;
                    friendsList.UserId = friend.u.Id;
                    friendsList.FriendId = friend.f.FriendId;

                    followers.Add(friendsList);
                }

                return followers;
            }
        }

        public List<FriendsList> GetUsersPending(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var pendingFriends = new List<FriendsList>();

                var friends = (from f in context.tFriendsLists
                               join u in context.AspNetUsers on f.Id equals u.Id
                               where f.Id == userId && f.Pending == true
                               select new { u, f }).ToList();

                foreach (var friend in friends)
                {
                    var friendsList = new FriendsList();

                    var pending = (from up in context.tUserProfiles
                                   where up.Id == friend.f.FriendId
                                   select
                                       up).First();

                    friendsList.Email = friend.u.Email;
                    friendsList.UserName = pending.AspNetUser.UserName;
                    friendsList.FirstName = pending.FirstName;
                    friendsList.LastName = pending.LastName;
                    friendsList.FriendsListId = friend.f.FriendListId;
                    friendsList.Avatar = pending.Avatar;
                    friendsList.Blocked = friend.f.Block;
                    friendsList.FriendId = friend.f.FriendId;
                    friendsList.UserId = friend.u.Id;

                    pendingFriends.Add(friendsList);
                }

                return pendingFriends;
            }
        }
        #endregion

        #region Admin
        public IEnumerable<AspNetUser> GetAllUsers()
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var users = (from u in context.AspNetUsers select u).ToList();

                return users;
            }
        }

        public AspNetUser SearchUsers(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var user = context.AspNetUsers.Where(u => u.UserName == userName).FirstOrDefault();

                return user;
            }
        }

        public UserInformation GetUserInformation(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var userInfo = (from p in context.tUserProfiles
                                join m in context.tMemberships on p.Id equals m.Id
                                join s in context.tSubscriptions on m.MembershipId equals s.MembershipId
                                join st in context.tSubscriptionTypes on s.SubscriptionTypeId equals st.SubscriptionTypeId
                                where p.AspNetUser.Id == userId
                                select new UserInformation
                                {
                                    FirstName = p.FirstName,
                                    LastName = p.LastName,
                                    BirthDate = p.BirthDate,
                                    SubscriptionActive = s.Active,
                                    StartDate = s.StartDate,
                                    ExpireDate = s.ExpireDate,
                                    Expired = s.Expired,
                                    AutoRenew = s.AutoRenew,
                                    SubscriptionType = st.SubscriptionType,
                                    MemberSince = m.MemberSince,
                                    MembershipId = m.MembershipId,
                                    MembershipActive = m.Active,
                                    PayPalCustomerId = m.PayPalCustomerId,
                                    PayPalSubscriptionId = s.PayPalSubscriptionId
                                }).OrderByDescending(i => i.StartDate).FirstOrDefault();

                return userInfo;
            }
        }
        #endregion

        #region Alpha and Beta Testing
        public List<int?> GetCurrentAlphaTesters(string code)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var count = (from u in context.AspNetUsers where u.tTestingCode.Code == code select u.CodeId).ToList();

                return count;
            }
        }

        public int? GetCodeCount(string code)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var count = (from tc in context.tTestingCodes where tc.Code == code select tc.Count).FirstOrDefault();

                return count;
            }
        }

        public int GetCodeId(string code)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var codeId = (from tc in context.tTestingCodes where tc.Code == code select tc.CodeId).First();

                return codeId;
            }
        }

        public void CreateFeedback(int scale, int overall, string description, string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                tFeedback feedback = new tFeedback
                {
                    Scale = scale,
                    Overall = overall,
                    Description = description,
                    Id = userId
                };

                context.tFeedbacks.Add(feedback);
                context.SaveChanges();
            }
        }
        #endregion

        #region Privacy Settings
        public void CreateSettings(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var settings = new tSetting
                {
                    Id = userId,
                    Discover = true,
                    Status = true,
                    PrivateStatus = true,
                    Friends = true,
                    Posts = true
                };
                context.tSettings.Add(settings);
                context.SaveChanges();
            }
        }

        public tSetting GetSettings(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var settings = context.tSettings.Where(s => s.Id == userId).First();

                return settings;
            }
        }

        public tSetting GetSettingsByUserName(string userName)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var settings = context.tSettings.Where(s => s.AspNetUser.UserName == userName).First();

                return settings;
            }
        }

        public string UpdateSettings(string userId, string postOnly, string statusOnly)
        {
            try
            {
                using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
                {
                    var post = Convert.ToBoolean(postOnly);
                    var status = Convert.ToBoolean(statusOnly);

                    var settings = context.tSettings.Where(s => s.Id == userId).FirstOrDefault();
                    settings.Posts = post;
                    settings.Status = status;

                    context.SaveChanges();

                    return "Settings Saved!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "Settings Could Not Be Saved!";
            }
        }

        public string UpdateSettings(string userId, string privateOnly)
        {
            try
            {
                using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
                {
                    var privateStatus = Convert.ToBoolean(privateOnly);

                    var settings = context.tSettings.Where(s => s.Id == userId).FirstOrDefault();
                    settings.PrivateStatus = privateStatus;

                    context.SaveChanges();

                    return "Settings Saved!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "Settings Could Not Be Saved!";
            }
        }
        #endregion

        #region Notifications
        public int CreateNotification(string fromUserId, string statusId, int commentId, int messageId, int friendListId, int statusControlId, int mentionId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                if (statusId != "0")
                {
                    var sId = Convert.ToInt32(statusId);
                    var userId = (from su in context.tStatusUpdates where su.StatusId == sId select su.Id).First();
                    tNotification notification = new tNotification
                    {
                        Id = userId,
                        FromUserId = fromUserId,
                        StatusId = (sId == 0) ? (int?)null : sId,
                        CommentId = (commentId == 0) ? (int?)null : commentId,
                        MessageId = (messageId == 0) ? (int?)null : messageId,
                        StatusControlId = (statusControlId == 0) ? (int?)null : statusControlId,
                        FriendListId = (friendListId == 0) ? (int?)null : friendListId,
                        DateCreated = DateTime.Now
                    };
                    context.tNotifications.Add(notification);
                    context.SaveChanges();

                    return notification.NotificationId;
                }
                else if (messageId != 0 || commentId != 0 || friendListId != 0 || statusControlId != 0 || mentionId != 0)
                {
                    var sId = 0;
                    var userId = "";
                    if (messageId != 0)
                    {
                        userId = (from m in context.tMessageControls where m.MessageId == messageId select m.ToUserId).First();
                    }
                    else if (friendListId != 0)
                    {
                        userId = (from f in context.tFriendsLists where f.FriendListId == friendListId select f.FriendId).First();
                    }
                    else if (commentId != 0)
                    {
                        userId = (from sc in context.tStatusComments
                                  join s in context.tStatusUpdates on sc.StatusId equals s.StatusId
                                  where sc.CommentId == commentId
                                  select s.Id).First();
                    }
                    else if (statusControlId != 0)
                    {
                        userId = (from sc in context.tStatusControls
                                  join s in context.tStatusUpdates on sc.StatusId equals s.StatusId
                                  where sc.StatusControlId == statusControlId
                                  select s.Id).First();
                    }
                    else if (mentionId != 0)
                    {
                        userId = (from m in context.tMentions where m.MentionId == mentionId select m.MentionedUserId).First();
                    }

                    tNotification notification = new tNotification
                        {
                            Id = userId,
                            FromUserId = fromUserId,
                            StatusId = (sId == 0) ? (int?)null : sId,
                            CommentId = (commentId == 0) ? (int?)null : commentId,
                            MessageId = (messageId == 0) ? (int?)null : messageId,
                            FriendListId = (friendListId == 0) ? (int?)null : friendListId,
                            StatusControlId = (statusControlId == 0) ? (int?)null : statusControlId,
                            MentionId = (mentionId == 0) ? (int?)null : mentionId,
                            DateCreated = DateTime.Now
                        };

                    if (userId != fromUserId)
                    {
                        context.tNotifications.Add(notification);
                        context.SaveChanges();
                    }

                    return notification.NotificationId;
                }
            }

            //if it makes it to here, something bad has happened. 
            return 0;
        }

        public List<Notifications> GetNotifications(string userId)
        {
            var comments = new List<Notifications>();
            var messages = new List<Notifications>();
            var likes = new List<Notifications>();
            var friends = new List<Notifications>();
            var mentionsStatus = new List<Notifications>();
            var mentionsComments = new List<Notifications>();

            messages.Clear();
            likes.Clear();
            friends.Clear();

            var notifications = new List<Notifications>();

            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                comments = (from n in context.tNotifications
                            join c in context.tStatusComments on n.CommentId equals c.CommentId
                            join s in context.tStatusUpdates on c.StatusId equals s.StatusId
                            join u in context.AspNetUsers on n.FromUserId equals u.Id
                            join p in context.tUserProfiles on n.FromUserId equals p.Id
                            where n.Id == userId
                            select new Notifications
                            {
                                Avatar = p.Avatar,
                                FromUserName = u.UserName,
                                NotificationId = n.NotificationId,
                                CommentId = c.CommentId,
                                Comment = c.Comment,
                                Status = s.Status,
                                StatusDate = s.DateCreated,
                                DateCreated = n.DateCreated,
                                DateRead = n.DateRead
                            }).ToList();

                messages = (from n in context.tNotifications
                            join m in context.tMessages on n.MessageId equals m.MessageId
                            join u in context.AspNetUsers on n.FromUserId equals u.Id
                            join p in context.tUserProfiles on n.FromUserId equals p.Id
                            where n.Id == userId && m.IsIM == false
                            select new Notifications
                            {
                                Avatar = p.Avatar,
                                FromUserName = u.UserName,
                                NotificationId = n.NotificationId,
                                MessageId = m.MessageId,
                                Message = m.Message,
                                DateCreated = n.DateCreated,
                                DateRead = n.DateRead
                            }).ToList();

                likes = (from n in context.tNotifications
                         join l in context.tStatusControls on n.StatusControlId equals l.StatusControlId
                         join s in context.tStatusUpdates on l.StatusId equals s.StatusId
                         join u in context.AspNetUsers on n.FromUserId equals u.Id
                         join p in context.tUserProfiles on n.FromUserId equals p.Id
                         where n.Id == userId
                         select new Notifications
                         {
                             Avatar = p.Avatar,
                             FromUserName = u.UserName,
                             NotificationId = n.NotificationId,
                             LikedBy = l.LikedBy,
                             DislikedBy = l.DisLikedBy,
                             Status = s.Status,
                             StatusDate = s.DateCreated,
                             DateCreated = n.DateCreated,
                             DateRead = n.DateRead
                         }).ToList();

                friends = (from n in context.tNotifications
                           join f in context.tFriendsLists on n.FriendListId equals f.FriendListId
                           join u in context.AspNetUsers on n.FromUserId equals u.Id
                           join p in context.tUserProfiles on n.FromUserId equals p.Id
                           where n.Id == userId
                           select new Notifications
                             {
                                 Avatar = p.Avatar,
                                 FromUserName = u.UserName,
                                 NotificationId = n.NotificationId,
                                 FriendListId = f.FriendListId,
                                 FriendPending = f.Pending,
                                 DateCreated = n.DateCreated,
                                 DateRead = n.DateRead
                             }).ToList();

                mentionsStatus = (from n in context.tNotifications
                                  join m in context.tMentions on n.MentionId equals m.MentionId
                                  join s in context.tStatusUpdates on m.StatusId equals s.StatusId
                                  join u in context.AspNetUsers on n.FromUserId equals u.Id
                                  join p in context.tUserProfiles on n.FromUserId equals p.Id
                                  where n.Id == userId
                                  select new Notifications
                                  {
                                      Avatar = p.Avatar,
                                      FromUserName = u.UserName,
                                      NotificationId = n.NotificationId,
                                      MentionId = m.MentionId,
                                      Status = s.Status,
                                      StatusDate = s.DateCreated,
                                      DateCreated = n.DateCreated,
                                      DateRead = n.DateRead
                                  }).ToList();

                mentionsComments = (from n in context.tNotifications
                                    join m in context.tMentions on n.MentionId equals m.MentionId
                                    join c in context.tStatusComments on m.CommentId equals c.CommentId
                                    join s in context.tStatusUpdates on c.StatusId equals s.StatusId
                                    join u in context.AspNetUsers on n.FromUserId equals u.Id
                                    join p in context.tUserProfiles on n.FromUserId equals p.Id
                                    where n.Id == userId
                                    select new Notifications
                                    {
                                        Avatar = p.Avatar,
                                        FromUserName = u.UserName,
                                        NotificationId = n.NotificationId,
                                        MentionedCommentId = m.CommentId,
                                        Comment = c.Comment,
                                        Status = s.Status,
                                        StatusDate = s.DateCreated,
                                        DateCreated = n.DateCreated,
                                        MentionId = m.MentionId,
                                        DateRead = n.DateRead
                                    }).ToList();

                notifications = comments.Concat(messages).Concat(likes).Concat(friends).Concat(mentionsStatus).Concat(mentionsComments).OrderByDescending(x => x.DateCreated).ToList();

            }

            return notifications;
        }

        public int GetNotificationCount(string userId)
        {
            var count = GetNotifications(userId).Where(x => x.DateRead == null).Count();

            return count;
        }

        public string UpdateNotifcation(int notificationId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var notification = (from n in context.tNotifications where n.NotificationId == notificationId select n).First();
                notification.DateRead = DateTime.Now;
                context.SaveChanges();

                return "Read";
            }
        }
        #endregion

    }// public class service
} // namespace
