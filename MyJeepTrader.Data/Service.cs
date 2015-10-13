using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
