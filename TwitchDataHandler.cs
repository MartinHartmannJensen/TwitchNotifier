using System;
using System.Collections.Generic;
using System.Linq;
using ArethruNotifier.Helix;

namespace ArethruNotifier {
    public class TwitchDataHandler {

        private Streams currentStreams = null;
        public Streams CurrentStreams { get { return currentStreams; } }
        private Follows currentFollows = null;
        public Follows CurrentFollows { get { return currentFollows; } }
        public Dictionary<string, Game> gameRegister;

        private NotifyCtr NotifyObj = ConfigMgnr.I.NotifyController;
        private Object threadLock = new Object();

        public enum UpdateMode {
            Compare,
            Force
        }

        public TwitchDataHandler() {
             gameRegister = new Dictionary<string, Game>();
        }

        /// <summary>
        /// Calls the twitch api, and displays a notification if updates are found.
        /// </summary>
        /// <param name="mode">'Force' is a basic update. The notification window will always be shown with the gotten info.
        /// 'Compare' is the smart mode that only shows a notification, if something new has occurred</param>
        public async void UpdateLive(UpdateMode mode) {
            // If no token assigned, don't do anything
            if (ConfigMgnr.I.UserToken.Equals("notoken")) {
                return;
            }
            //var tempF = await HelixAPI.GetFollows("68744599");
            var tempF = await HelixAPI.GetFollows(ConfigMgnr.I.UserToken);
            if (!tempF.IsOk) {
                return;
            }

            currentFollows = tempF;

            var preTempS = await HelixAPI.GetStreams(tempF.GenerateUserIds());
            if (!preTempS.IsOk) {
                return;
            }

            var tempS = await AddGames(preTempS);

            lock (threadLock) {
                if (mode == UpdateMode.Force) {
                    currentStreams = tempS;
                    NotifyObj.DisplayNotificationWindow(tempS);
                }
                else if (mode == UpdateMode.Compare) {
                    Compare(tempS);
                }
            }
        }

        private void Compare(Streams s) {
            if (currentStreams == null) {
                currentStreams = s;
                NotifyObj.DisplayNotificationWindow(s);
                return;
            }

            FavouriteGroup kingOfTheFill = null;
            FavouriteGroup contenderOfTheFill = null;
            bool foundNew = false;
            foreach (var item in s.Stream) {
                if (!currentStreams.Stream.Exists(x => x.Channel.Equals(item.Channel))) {
                    foundNew = true;
                    if (MiscOperations.TryGetFavourite(item.Channel, out contenderOfTheFill)) {
                        if (kingOfTheFill == null || kingOfTheFill.Priority < contenderOfTheFill.Priority) {
                            kingOfTheFill = contenderOfTheFill;
                        }
                    }
                }
            }

            if (foundNew && kingOfTheFill != null) {
                currentStreams = s;
                NotifyObj.DisplayNotificationWindow(s, kingOfTheFill);
            }
            else if (foundNew) {
                currentStreams = s;
                NotifyObj.DisplayNotificationWindow(s);
            }
        }

        public async System.Threading.Tasks.Task<Streams> AddGames(Streams s) {
            // check if register is missing a game
            var dummyStreamsObj = new Streams();
            dummyStreamsObj.Stream = new List<Stream>();
            foreach (var item in s.Stream) {
                if (!gameRegister.ContainsKey(item.GameId)) {
                    dummyStreamsObj.Stream.Add(item);
                }
            }

            // if missing add them
            if (dummyStreamsObj.Stream.Count > 0) {
                Games gam = await HelixAPI.GetGames(dummyStreamsObj.GenerateUserIds());
                if (!gam.IsOk) {
                    return s;
                }

                foreach (var item in gam.Game) {
                    gameRegister.Add(item.Id, item);
                }
            }

            // populate parameter object with game names
            foreach (var item in s.Stream) {
                item.Game = gameRegister[item.GameId].Name;
            }

            return s;
        }

        // Deprecated
        //        /// <summary>
        //        /// Compares the new StreamsInfo with the current one. Requests a notification window if new entries are found.
        //        /// </summary>
        //        private void CompareInfo(Streams si) {
        //            var newstreams = new List<Channel>();
        //            FavouriteGroup tempgroup = null;
        //            FavouriteGroup returngroup = null;

        //            // Check if it is a first-time update (currentInfo == null)
        //            if (currentInfo != null) {
        //                foreach (var item in si.Stream) {
        //                    if (!currentInfo.Stream.Exists(x => x.Channel.Name.Equals(item.Channel.Name))) {
        //                        newstreams.Add(item.Channel);
        //                    }
        //                }
        //            }
        //            else {
        //                newstreams = si.Stream.Select(x => x.Channel).ToList();
        //            }

        //            // If no new streams detected: Update existing info and escape the function
        //            if (newstreams.Count == 0) {
        //                currentInfo = si;
        //                return;
        //            }

        //            foreach (var item in newstreams) {
        //                if (MiscOperations.TryGetFavourite(item.Name, out tempgroup)) {
        //                    if (returngroup == null)
        //                        returngroup = tempgroup;
        //                    else if (returngroup.Priority < tempgroup.Priority)
        //                        returngroup = tempgroup;
        //                }
        //            }

        //            currentInfo = si;
        //            if (returngroup == null) {
        //                NotifyObj.DisplayNotificationWindow(si);
        //            }
        //            else {
        //                NotifyObj.DisplayNotificationWindow(si, returngroup);
        //            }
        //        }

        public Tuple<List<Follow>, List<Follow>> GetFollows() {
            List<Follow> onlineF = new List<Follow>();

            List<Follow> offlineF = new List<Follow>();

            foreach (var item in currentFollows.Follow) {
                if (currentStreams.Stream.Exists(x => x.Channel.Equals(item.Name))) {
                    onlineF.Add(item);
                }
                else {
                    offlineF.Add(item);
                }
            }

            onlineF = onlineF.OrderBy(x => x.Name).ToList();

            offlineF = offlineF.OrderBy(x => x.Name).ToList();

            return Tuple.Create(onlineF, offlineF);
        }
    }
}
