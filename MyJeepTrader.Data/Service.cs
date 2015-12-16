using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using MyJeepTrader.Data.Models;
using System.IO;

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

        public IEnumerable<tUserProfile> GetAllUserProfiles()
        {
            return (from up in _context.tUserProfiles select up).ToList();
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

        public List<UsersPosts> GetAllPostsForUser(int userProfileId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var result = (from p in context.tPosts
                              join pc in context.tPostsControls on p.PostId equals pc.PostId
                              join u in context.AspNetUsers on pc.Id equals u.Id
                              join up in context.tUserProfiles on u.Id equals up.Id
                              join pt in context.tPostTypes on p.PostTypeId equals pt.PostTypeId
                              where up.UserProfileId == userProfileId
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

        public UsersPosts GetUsersMostRecentPost(int userProfileId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var result = (from p in context.tPosts
                              join pc in context.tPostsControls on p.PostId equals pc.PostId
                              join u in context.AspNetUsers on pc.Id equals u.Id
                              join up in context.tUserProfiles on u.Id equals up.Id
                              join pt in context.tPostTypes on p.PostTypeId equals pt.PostTypeId
                              where up.UserProfileId == userProfileId
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
            //using (_context)
            //{
            return (from p in _context.tPostTypes select p).ToList();
            //}
        }
        #endregion

        #region Membership
        public void CreateMembership(string userId)
        {
            using (_context)
            {
                tMembership membership = new tMembership
                    {
                        Id = userId,
                        PremiumMembership = false,
                        BasicMembership = true,
                        MemberSince = DateTime.Now.Date,
                        AutoRenew = false,
                        Renewed = false,
                        Expired = false,
                        ExpirationDate = DateTime.Now.Date,
                    };
                _context.tMemberships.Add(membership);
                _context.SaveChanges();
            }
        }
        #endregion

        #region User Profile
        public byte[] GetAvatarImage(int UserProfileId)
        {
            using (_context)
            {
                var avatar = (from up in _context.tUserProfiles
                              where up.UserProfileId == UserProfileId
                              select up.Avatar).FirstOrDefault();

                return avatar;
            }
        }

        public void CreateProfile(string userId, string firstName, string lastName, DateTime birthDate, byte[] avatar, string description, string facebook, string twitter, string ello, string google, string website)
        {
            using (_context)
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
                _context.tUserProfiles.Add(userProfile);
                _context.SaveChanges();
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

        public tUserProfile GetProfileInfoByProfileId(int profileId)
        {
            using (_context)
            {
                return _context.tUserProfiles.Where(up => up.UserProfileId == profileId).First();
            }
        }

        public long? GetViewCount(int UserProfileId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var count = (from up in context.tUserProfiles where up.UserProfileId == UserProfileId select up.ViewCount).FirstOrDefault();

                return count;
            }
        }

        public void UpdateViewCount(int UserProfileId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var updateProfile = context.tUserProfiles.Where(up => up.UserProfileId == UserProfileId).FirstOrDefault();
                updateProfile.ViewCount = GetViewCount(UserProfileId) + 1;
                context.SaveChanges();
            }
        }
        #endregion

        #region Jeep Profile
        public byte[] GetJeepImage(int JeepProfileId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var image = (from jp in context.tVehicleProfiles
                             where jp.VehicleProfileId == JeepProfileId
                             select jp.Image).FirstOrDefault();

                return image;
            }
        }

        public void CreatePrimaryJeepProfile(string userId, string manufactuer, string make, string model, short year, byte[] jeepImage, string jeepDescription, bool primaryJeep)
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

        public tVehicleProfile GetPrimaryJeepInfo(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                return (from vp in context.tVehicleProfiles
                        join vpc in context.tVehicleProfileControls on vp.VehicleProfileId equals vpc.VehicleProfileId
                        where vpc.Id == userId
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
                                 where vpc.Id == userId
                                 select vp).Any();

            return noPrimaryJeep;
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

        #region Mailbox
        public void CreateMessage(string toUser, string fromUserId, string subject, string message)
        {
            using (_context)
            {
                string toUserId = (from u in _context.AspNetUsers where u.UserName == toUser select u.Id).FirstOrDefault();

                tMessage mess = new tMessage
                {
                    Subject = subject,
                    Message = message,
                    DateSent = DateTime.Now
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
                                    where mc.FromUserId == userId
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
                             where mc.ToUserId == userId
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
        #endregion

    }// public class service
} // namespace
