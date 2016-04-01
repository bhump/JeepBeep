﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyJeepTrader.Data.Models;

namespace MyJeepTrader.Web.ViewModels
{
    public class PopularPostsAndProfilesViewModel
    {
        public List<UserProfiles> PopularProfiles { get; private set; }

        public List<FriendsList> UsersFriends { get; private set; }

        public List<FriendsList> PendingFriends { get; private set; }

        public PopularPostsAndProfilesViewModel(List<UserProfiles> pp, List<FriendsList> uf, List<FriendsList> pf)
        {
            //this.PopularProfiles = pp.Where(p => !uf.Any(u => u.UserName == p.UserName) || !pf.Any(f => f.UserName == p.UserName)).ToList();

            var newPopular = new List<UserProfiles>();
            var popular = new List<UserProfiles>();
            newPopular = pp.Where(p => !uf.Any(u => u.UserName == p.UserName)).ToList();

            popular = newPopular.Where(np => !pf.Any(fp => fp.UserName == np.UserName)).ToList();

            this.PopularProfiles = popular;
        }
    }
}