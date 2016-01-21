﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ArethruNotifier
{
    public delegate void NewStreamFoundEventHandler(StreamsInfo streamsInfo);

    public class TwitchDataHandler
    {
        private static TwitchDataHandler instance = null;

        public static TwitchDataHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new TwitchDataHandler();
                return instance;
            }
        }

        private TwitchDataHandler() { }


        private StreamsInfo currentInfo = null;
        public StreamsInfo CurrentInfo { get { return currentInfo; } private set { currentInfo = value; } }

        public StreamsInfo TimeSortedCurrentInfo { get { return SortByTime(currentInfo); } }

        private DateTime timeRecieved;
        public DateTime TimeRecieved { get { return timeRecieved; } private set { timeRecieved = value; } }

        public NewStreamFoundEventHandler FoundNewStreamEvent;

        /// <summary>
        /// Calls RESTcall and blocks the Thread before updating currentInfo
        /// </summary>
        public bool UpdateInfo()
        {
            var tempSI = WebComm.GetLiveStreams();

            if (tempSI.IsSucces)
            {
                CurrentInfo = tempSI;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates currentInfo with the passed parameter variable
        /// </summary>
        /// <param name="si"></param>
        public void UpdateAndCompare(StreamsInfo si)
        {
            var tempSI = si;

            if (tempSI.IsSucces)
            {
                CompareInfo(tempSI);
            }
        }

        /// <summary>
        /// Compares the new StreamsInfo with the current one. Invokes NotifyEvent if new entries are found.
        /// </summary>
        private void CompareInfo(StreamsInfo si)
        {
            if (CurrentInfo == null)
            {
                CurrentInfo = si;
                TimeRecieved = DateTime.Now;
                if (FoundNewStreamEvent != null)
                    FoundNewStreamEvent(si);
                return;
            }

            foreach (var item in si.Streams)
            {
                if (!CurrentInfo.Streams.Exists(x => x.Channel.Name.Equals(item.Channel.Name)))
                {
                    CurrentInfo = si;
                    TimeRecieved = DateTime.Now;
                    if (FoundNewStreamEvent != null)
                        FoundNewStreamEvent(si);
                    return;
                }
            }
            CurrentInfo = si;
        }

        private StreamsInfo SortByTime(StreamsInfo si)
        {
            List<StreamsObj> sortedList = si.Streams.OrderByDescending(x => x.CreatedAt).ToList();

            return new StreamsInfo() { Streams = sortedList, IsSucces = true };
        }

        public Tuple<List<Follow>, List<Follow>> GetFollows()
        {
            var follows = WebComm.GetFollowedChannels();

            if (!follows.IsSucces)
            {
                return null;
            }

            if (CurrentInfo == null)
                if (!UpdateInfo())
                    return null;

            List<Follow> onlineF = new List<Follow>();

            List<Follow> offlineF = new List<Follow>();

            foreach (var item in follows.List)
            {
                if (TwitchDataHandler.Instance.CurrentInfo.Streams.Exists(x => x.Channel.Name.Equals(item.Channel.Name)))
                {
                    onlineF.Add(item);
                }
                else
                {
                    offlineF.Add(item);
                }
            }

            onlineF = onlineF.OrderBy(x => x.Channel.Name).ToList();

            offlineF = offlineF.OrderBy(x => x.Channel.Name).ToList();

            return Tuple.Create(onlineF, offlineF);
        }
    }
}