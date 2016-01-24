# ArethruNotifier

Program written for Windows. Designed to track when when a Twitch.tv streamer goes live, based on a users Twitch.tv follows.
Can be configured to run minimized in the background, displaying notifications and playing a sound whenever someone goes live.
Set n' forget, if you choose to make it start minimized with Windows.

#### [Download the Installer][Releases]

#### [Getting Started - A Quick Guide][Howto]


### How it works

Using the [Twitch API][API], this program contacts their servers periodically to check for updates. How often it checks, 
can be configured by the user.
It will need persmission to a twitch user profile. You can read about the different access scopes [here][scopes]. This program utilizes
the scope "user_read", and the permissions are displayed at authorization. There's only 2 types of information being fetched, though: 
a users followed streams and the streams currently live for the user. 
Nothing is stored anywhere, besides the user settings in a local config file.

Licensed under the MIT License

[Releases]: <https://github.com/MartinHartmannJensen/TwitchNotifier/releases>
[Howto]: <https://github.com/MartinHartmannJensen/TwitchNotifier/wiki#getting-started---a-quick-guide>
[scopes]: <https://github.com/justintv/Twitch-API/blob/master/authentication.md#scopes>
[API]: <https://github.com/justintv/Twitch-API>
