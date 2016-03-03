using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyJeepTrader.Data;
using MyJeepTrader.Data.Models;

namespace MyJeepTrader.Web.ViewModels
{
    public class TimelineSettingsViewModel
    {
        public tTimelineSetting Settings { get; set; }
    }

    public class LiveStreamViewModel
    {
        public ICollection<LiveFeed> LiveFeeds { get; private set; }

        public LiveStreamViewModel(ICollection<LivePost> lp, ICollection<LiveStream> ls)
        {
            this.LiveFeeds = new List<LiveFeed>();
            /* this is a "one by one" with no error checking if the counts are different */

            IEnumerator<LiveFeed> livePostEnum = null == lp ? null : lp.GetEnumerator();
            IEnumerator<LiveFeed> liveStreamEnum = null == ls ? null : ls.GetEnumerator();

            if (null != liveStreamEnum)
            {
                liveStreamEnum.MoveNext();
            }

            if (null != livePostEnum)
            {
                while (livePostEnum.MoveNext() == true)
                {
                    LiveFeed lpCurrent = livePostEnum.Current;
                    LiveFeed lsCurrent = null == liveStreamEnum ? null : liveStreamEnum.Current;
                    if (null != liveStreamEnum)
                    {
                        liveStreamEnum.MoveNext();
                    }

                    this.LiveFeeds.Add(lpCurrent);
                    if (null != lsCurrent)
                    {
                        this.LiveFeeds.Add(lsCurrent);
                    }
                }
            }
        }

        public tTimelineSetting Settings { get; set; }

        public LiveStreamViewModel()
        {
            this.Settings = new tTimelineSetting();
        }
    }
}