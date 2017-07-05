using System;
using System.Collections.Generic;
using System.Linq;

namespace ArethruNotifier {
    public class TwitchDataHandler {

        private StreamsInfo currentInfo = null;
        public StreamsInfo CurrentInfo { get { return currentInfo; } }

        private NotifyCtr NotifyObj = ConfigMgnr.I.NotifyController;
        private Object threadLock = new Object();

        public enum UpdateMode {
            Compare,
            Force
        }

        /// <summary>
        /// Calls the twitch api, and displays a notification if updates are found.
        /// </summary>
        /// <param name="mode">'Force' is a basic update. The notification window will always be shown with the gotten info.
        /// 'Compare' is the smart mode that only shows a notification, if something new has occurred</param>
        public async void UpdateLive(UpdateMode mode) {
            var tempSi = await APIcalls.GetLiveStreamsTask();

            lock (threadLock) {
                if (!tempSi.IsSuccess) {
                    return;
                }

                if (mode == UpdateMode.Force) {
                    currentInfo = tempSi;
                    NotifyObj.DisplayNotificationWindow(tempSi);
                }
                else if (mode == UpdateMode.Compare) {
                    CompareInfo(tempSi);
                }
            }
        }

        /// <summary>
        /// Compares the new StreamsInfo with the current one. Requests a notification window if new entries are found.
        /// </summary>
        private void CompareInfo(StreamsInfo si) {
            var newstreams = new List<Channel>();
            FavouriteGroup tempgroup = null;
            FavouriteGroup returngroup = null;

            // Check if it is a first-time update (currentInfo == null)
            if (currentInfo != null) {
                foreach (var item in si.Streams) {
                    if (!currentInfo.Streams.Exists(x => x.Channel.Name.Equals(item.Channel.Name))) {
                        newstreams.Add(item.Channel);
                    }
                }
            }
            else {
                newstreams = si.Streams.Select(x => x.Channel).ToList();
            }

            // If no new streams detected: Update existing info and escape the function
            if (newstreams.Count == 0) {
                currentInfo = si;
                return;
            }

            foreach (var item in newstreams) {
                if (MiscOperations.TryGetFavourite(item.Name, out tempgroup)) {
                    if (returngroup == null)
                        returngroup = tempgroup;
                    else if (returngroup.Priority < tempgroup.Priority)
                        returngroup = tempgroup;
                }
            }

            currentInfo = si;
            if (returngroup == null) {
                NotifyObj.DisplayNotificationWindow(si);
            }
            else {
                NotifyObj.DisplayNotificationWindow(si, returngroup);
            }
        }

        public Tuple<List<Follow>, List<Follow>> GetFollows() {
            var follows = APIcalls.GetFollowedChannels();

            if (!follows.IsSucces || currentInfo == null) {
                return null;
            }

            List<Follow> onlineF = new List<Follow>();

            List<Follow> offlineF = new List<Follow>();

            foreach (var item in follows.List) {
                if (currentInfo.Streams.Exists(x => x.Channel.Name.Equals(item.Channel.Name))) {
                    onlineF.Add(item);
                }
                else {
                    offlineF.Add(item);
                }
            }

            onlineF = onlineF.OrderBy(x => x.Channel.Name).ToList();

            offlineF = offlineF.OrderBy(x => x.Channel.Name).ToList();

            return Tuple.Create(onlineF, offlineF);
        }
    }
}
