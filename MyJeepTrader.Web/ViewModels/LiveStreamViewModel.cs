using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyJeepTrader.Data;
using MyJeepTrader.Data.Models;

namespace MyJeepTrader.Web.ViewModels
{
    public class LiveStreamViewModel
    {
        public ICollection<LiveFeed> LiveFeeds { get; private set; }

        public ICollection<LiveFeed> PublicFeed { get; private set; }

        public ICollection<LiveFeed> OwnFeed { get; private set; }

        public tSetting Settings { get; set; }

        public List<string> FriendsList { get; set; }

        public List<JeepModels> Models { get; set; }

        public LiveStreamViewModel(ICollection<LivePost> lp, ICollection<LiveStream> ls)
        {
            this.LiveFeeds = new List<LiveFeed>();
            /* this is a "one by one" with no error checking if the counts are different */

            IEnumerator<LiveFeed> livePostEnum = null == lp ? null : lp.GetEnumerator();
            IEnumerator<LiveFeed> liveStreamEnum = null == ls ? null : ls.GetEnumerator();

            if (lp.Count == 0)
            {
               if(ls.Count != 0)
               {
                   foreach(var stream in ls)
                   {
                       this.LiveFeeds.Add(stream);
                   }
               }
            }
            else
            {
                if (null != livePostEnum)
                {
                    livePostEnum.MoveNext();
                }

                if (null != liveStreamEnum)
                {
                    while (liveStreamEnum.MoveNext() == true)
                    {
                        //LiveFeed lpCurrent = livePostEnum.Current;
                        //LiveFeed lsCurrent = null == liveStreamEnum ? null : liveStreamEnum.Current;
                        LiveFeed lsCurrent = liveStreamEnum.Current;
                        LiveFeed lpCurrent = null == livePostEnum ? null : livePostEnum.Current;
                        if (null != livePostEnum)
                        {
                            livePostEnum.MoveNext();
                        }

                        this.LiveFeeds.Add(lsCurrent);
                        if (null != lpCurrent)
                        {
                            this.LiveFeeds.Add(lpCurrent);
                        }
                    }
                }
            }
        }

        public LiveStreamViewModel(ICollection<LiveStream> ls)
        {
            IEnumerator<LiveFeed> liveStreamEnum = null == ls ? null : ls.GetEnumerator();

            if (null != liveStreamEnum)
            {
                liveStreamEnum.MoveNext();
            }
        }

        public LiveStreamViewModel()
        {
            this.Settings = new tSetting();

            this.FriendsList = new List<string>();

            this.Models = new List<JeepModels>();
        }
    }
}