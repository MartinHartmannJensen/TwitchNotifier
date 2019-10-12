using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArethruNotifier.Helix;

namespace ArethruNotifier {
    public class TwitchDataHandler {

        Streams currentStreams = null;
        public Streams CurrentStreams { get { return currentStreams; } }
        Follows currentFollows = null;
        public Follows CurrentFollows { get { return currentFollows; } }
        FavouriteGroup currentFavorites = null;
        public FavouriteGroup CurrentFavorites { get { return currentFavorites; } }
        public Dictionary<string, Game> gameRegister = new Dictionary<string, Game>();

        public enum UpdateResult {
            Nothing, Update, Favorite
        }

        /// <summary>
        /// Fetches a new list of Follows accessed at CurrentFollows
        /// </summary>
        public async void UpdateFollows() {
            var tempF = await HelixAPI.GetFollows(ConfigMgnr.I.UserToken);
            if (tempF.IsOk) {
                currentFollows = tempF;
            }
        }

        public async Task<UpdateResult> Update() {
            // If no token assigned, don't do anything
            if (ConfigMgnr.I.UserToken.Equals("notoken")) {
                return UpdateResult.Nothing;
            }

            if (currentFollows == null) {
                var tempF = await HelixAPI.GetFollows(ConfigMgnr.I.UserToken);
                if (!tempF.IsOk) {
                    return UpdateResult.Nothing;
                }
                currentFollows = tempF;
            }

            // Get live streams from follows
            var tempS = await HelixAPI.GetStreams(currentFollows.GenerateUserIds());
            if (!tempS.IsOk) {
                return UpdateResult.Nothing;
            }
            var s = await AddGames(tempS);

            // First check
            if (currentStreams == null) {
                currentStreams = s;
                return UpdateResult.Update;
            }

            // Compare
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

            // Results
            currentStreams = s;

            if (!foundNew) {
                return UpdateResult.Nothing;
            }
            if (kingOfTheFill != null) {
                currentFavorites = kingOfTheFill;
                return UpdateResult.Favorite;
            }
            return UpdateResult.Update;
        }


        private async Task<Streams> AddGames(Streams s) {
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

        public Tuple<List<Follow>, List<Follow>> GetFollowLists() {
            List<Follow> onlineF = new List<Follow>();
            List<Follow> offlineF = new List<Follow>();

            if (currentFollows == null) {
                return Tuple.Create(onlineF, offlineF);
            }

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
