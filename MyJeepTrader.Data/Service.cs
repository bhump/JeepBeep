using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using MyJeepTrader.Data.Models;
using System.IO;
using System.Data.Entity;

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
        /// <summary>
        /// Gets all the posts.
        /// </summary>
        /// <returns>List of posts.</returns>
        public IEnumerable<tPost> GetAllPosts()
        {
            return (from p in _context.tPosts select p).ToList();
        }

        /// <summary>
        /// Searches all posts. Making use of predicate builder and linqkit, this method searches for whatever fields
        /// you want to include and combines the results.
        /// http://www.albahari.com/nutshell/predicatebuilder.aspx
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <returns></returns>
        public IEnumerable<tPost> SearchAllPosts(params string[] keywords)
        {
            var predicate = PredicateBuilder.False<tPost>();

            foreach (string keyword in keywords)
            {
                string temp = keyword;
                predicate = predicate.Or(p => p.PostTitle.Contains(temp));
                predicate = predicate.Or(p => p.PostDescription.Contains(temp));
                //TODO What else do we want to search for?
            }

            return _context.tPosts.AsExpandable().Where(predicate);
        }

        /// <summary>
        /// Creates the new post.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns></returns>
        public int CreateNewPost(tPost post)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();
            context.tPosts.Add(post);
            context.SaveChanges();

            return post.PostId;
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

        public tSubscription GetSubscription(int? subscriptionId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var subscription = context.tSubscriptions.Where(s => s.SubscriptionId == subscriptionId).First();

                return subscription;
            }
        }

        public void CreateMembership(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                tMembership membership = new tMembership
                    {
                        Id = userId,
                        MemberSince = DateTime.Now.Date,
                        AutoRenew = false,
                        Renewed = false,
                        Expired = false,
                        ExpirationDate = DateTime.Now.Date,
                        SubscriptionId = context.tSubscriptions.Where(s => s.Subscription == "Free").Select(s => s.SubscriptionId).FirstOrDefault()
                    };
                context.tMemberships.Add(membership);
                context.SaveChanges();
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

        public void CreateProfile(string userId, string firstName, string lastName, DateTime birthDate, byte[] avatar, string description, string facebook, string twitter, string ello, string google, string website)
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
                        Website = website
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
        public void UpdateProfile(string userId, string firstName, string lastName, DateTime birthDate, byte[] avatar, string description, string facebook, string twitter, string ello, string google, string website)
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

        public List<tYear> GetAllYears()
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();

            return (from y in context.tYears select y).ToList();
        }
        #endregion

        #region Mailbox And Instant Chat
        public void CreateMessage(string toUser, string fromUser, string subject, string message, bool isIm)
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


    }// public class service
} // namespace
