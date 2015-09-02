using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArethruTwitchNotifier
{
    public delegate void NewStreamFoundEventHandler(StreamsInfo streamsInfo);

    public class StreamContainer
    {
        private static StreamContainer instance = null;

        public static StreamContainer Instance
        {
            get
            {
                if (instance == null)
                    instance = new StreamContainer();
                return instance;
            }
        }

        private StreamContainer() { }


        private StreamsInfo currentInfo = null;
        public StreamsInfo CurrentInfo { get { return currentInfo; } private set { currentInfo = value; } }

        public StreamsInfo TimeSortedCurrentInfo { get { return SortByTime(currentInfo); } }

        private DateTime timeRecieved;
        public DateTime TimeRecieved { get { return timeRecieved; } private set { timeRecieved = value; } }

        public NewStreamFoundEventHandler FoundNewStreamEvent;

        /// <summary>
        /// Calls RESTcall and blocks the Thread before updating currentInfo
        /// </summary>
        public void UpdateInfo()
        {
            var tempSI = RESTcall.GetLiveStreams();

            if (tempSI.isSucces)
            {
                CurrentInfo = tempSI;
            }
        }

        /// <summary>
        /// Updates currentInfo with the passed parameter variable
        /// </summary>
        /// <param name="si"></param>
        public void UpdateAndCompare(StreamsInfo si)
        {
            var tempSI = si;

            if (tempSI.isSucces)
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

            return new StreamsInfo() { Streams = sortedList, isSucces = true };
        }
    }
}
