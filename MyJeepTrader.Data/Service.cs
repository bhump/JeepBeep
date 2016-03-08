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

        #endregion

        #region Timeline
        public ICollection<LiveStream> GetLiveStream(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var privateSettings = GetSettings(userId);
                var statusList = new List<LiveStream>();

                statusList = (from fl in context.tFriendsLists
                              join su in context.tStatusUpdates on fl.FriendId equals su.Id
                              join up in context.tUserProfiles on su.Id equals up.Id
                              where fl.Id == userId && fl.Accepted == true
                              select new LiveStream
                                  {
                                      Status = su.Status,
                                      UserName = su.AspNetUser.UserName,
                                      IsPost = false,
                                      StatusId = su.StatusId,
                                      Avatar = up.Avatar,
                                      DateCreated = su.DateCreated
                                  }).OrderByDescending(su => su.DateCreated).ToList();

                return statusList;

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
                                        IsPost = false,
                                        StatusId = su.StatusId,
                                        Avatar = up.Avatar,
                                        DateCreated = su.DateCreated
                                    }).OrderByDescending(su => su.DateCreated).ToList();

                return publicStatus;

            }
        }

        public ICollection<LivePost> GetLivePosts()
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var posts = (from p in context.tPosts
                             join up in context.tUserProfiles on p.Id equals up.Id
                             select new LivePost
                                 {
                                     UserName = p.AspNetUser.UserName,
                                     PostDescription = p.PostDescription,
                                     IsPost = true,
                                     PostId = p.PostId,
                                     Avatar = up.Avatar,
                                     DateCreated = p.DateCreated
                                 }).OrderByDescending(p => p.DateCreated).ToList();

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

        #region Friends/Follw
        public string AddFriend(string userId, string friendId)
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

                    return "Friend Added Succesfully!";
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

                return "Error";
            }
        }

        public List<FriendsList> GetFriends(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var friends = (from f in context.tFriendsLists
                               join up in context.tUserProfiles on f.Id equals up.Id
                               join u in context.AspNetUsers on f.Id equals u.Id
                               where f.Id == userId && f.Accepted == true
                               select new FriendsList
                               {
                                   UserId = u.Id,
                                   UserProfileId = up.UserProfileId,
                                   UserName = u.UserName,
                                   FirstName = up.FirstName,
                                   LastName = up.LastName,
                                   Email = u.Email,
                                   Pending = f.Pending,
                                   Accepted = f.Accepted
                               }).ToList();

                return friends;
            }
        }

        public List<FriendsList> GetPendingFriends(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var friends = (from f in context.tFriendsLists
                               join up in context.tUserProfiles on f.FriendId equals up.Id
                               join u in context.AspNetUsers on f.FriendId equals u.Id
                               where f.Id == userId && f.Pending == true
                               select new FriendsList
                               {
                                   UserId = f.FriendId,
                                   UserProfileId = up.UserProfileId,
                                   UserName = u.UserName,
                                   FirstName = up.FirstName,
                                   LastName = up.LastName,
                                   Email = u.Email,
                                   Pending = f.Pending,
                                   Accepted = f.Accepted
                               }).ToList();

                return friends;
            }
        }

        public List<FriendsList> GetFollowerFriends(string userId)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var friends = (from f in context.tFriendsLists
                               join up in context.tUserProfiles on f.Id equals up.Id
                               join u in context.AspNetUsers on f.Id equals u.Id
                               where f.FriendId == userId && f.Pending == true
                               select new FriendsList
                               {
                                   UserId = u.Id,
                                   UserProfileId = up.UserProfileId,
                                   UserName = u.UserName,
                                   FirstName = up.FirstName,
                                   LastName = up.LastName,
                                   Email = u.Email,
                                   Pending = f.Pending,
                                   Accepted = f.Accepted
                               }).ToList();

                return friends;
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
                var count = (from tc in context.tTestingCodes where tc.Code == code select tc.Count).First();

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

        public void UpdateSettings(string userId, bool postOnly, bool statusOnly)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var settings = context.tSettings.Where(s => s.Id == userId).FirstOrDefault();
                settings.Posts = postOnly;
                settings.Status = statusOnly;

                context.SaveChanges();
            }
        }

        public void UpdateSettings(string userId, bool privateOnly)
        {
            using (dboMyJeepTraderEntities context = new dboMyJeepTraderEntities())
            {
                var settings = context.tSettings.Where(s => s.Id == userId).FirstOrDefault();
                settings.PrivateStatus = privateOnly;

                context.SaveChanges();
            }
        }
        #endregion

    }// public class service
} // namespace
