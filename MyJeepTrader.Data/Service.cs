using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqKit;

namespace MyJeepTrader.Data
{
    public class Service
    {

        private dboMyJeepTraderEntities _context;

        public Service()
        {
            _context = new dboMyJeepTraderEntities();
        }


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

        public object GetAvatarImage(int id)
        {
            //var avatar = _context.tUserProfiles.FirstOrDefault(a => a.AccountId == id);
            return null;
        }

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
                //updateProfile.Avatar = avatar;
                updateProfile.Description = description;
                updateProfile.Facebook = facebook;
                updateProfile.Twitter = twitter;
                updateProfile.Ello = ello;
                updateProfile.GooglePlus = google;
                updateProfile.Website = website;

                context.SaveChanges();
            }
        }

        public bool CheckForProfile(string userId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();
            var noProfile = (from up in context.tUserProfiles where up.Id == userId select up).Any();
            return noProfile;
        }

        public tUserProfile GetProfileInfo(string userId)
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();
            return context.tUserProfiles.Where(up => up.Id == userId).FirstOrDefault();
        }

        public IList<tModel> GetAllModels()
        {
            dboMyJeepTraderEntities context = new dboMyJeepTraderEntities();
            return (from m in context.tModels select m).ToList();
        }

        public void CreateJeepProfile(string userId, string manufactuer, string make, string model, byte[] jeepImage, string jeepDescription, bool defaultJeep)
        {
            using (_context)
            {
                tVehicleProfile jeep = new tVehicleProfile
                {
                    Manufacturer = manufactuer,
                    Make = make,
                    Model = model,
                    Image = jeepImage,
                    Description = jeepDescription,
                    DefaultJeep = defaultJeep
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

    }// public class service
} // namespace
