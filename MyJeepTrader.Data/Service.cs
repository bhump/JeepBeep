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

    }// public class service
} // namespace
