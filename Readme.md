# RefreshAuthService

This is the basic implementation of the OAuth 2.0 standard in **ASP .Net Framework**. This is my first `C#` project and hopefully I met all the coding standards.

As far as I know, I didn't find any direct implementation of `refresh_token` implementation online using the Visual Studio Web API Template with `Individual Authentication`

## Routes

This is project is created using Visual Studio's WebAPI Template and then added refresh token grant on top of it.
For creating a client:

- `POST` : `api/client` with `clientId` and `clientSecret`

You can register a user by

- `POST` : `api/account/register` with `email`, `password` and `confirmPassword`

For getting an `access_token` and `refresh_token`:

- `POST` : `/token` with `userName`(same as `email`), `password` and `grant_type : password`.

This should provide you with a `access_token` and `refresh_token`
After that you can use the `refresh_token` to get new `access_token` by:

- `POST` : `/token` with `grant_type : refresh_token` and `refresh_token: <YOUR_REFRESH_TOKEN>`

This is a very basic implementation of OAuth 2.0 Refresh Token Grants

## References

- I cannot stress how helpful the Blog from Taiseer was during this project. He has made a similar project ~6 years back and explained everything in detail. The blog is a Five part series on how to make **"Token Based Authentication using ASP .NET Web API"**. Check out the [first part here](https://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/). For the part focussing on [Refresh Token check it out here](https://bitoftech.net/2014/07/16/enable-oauth-refresh-tokens-angularjs-app-using-asp-net-web-api-2-owin/)
- The YouTube series by [Kudvenkat](https://www.youtube.com/user/kudvenkat) is a great entry point for detailed knowledge about Web APIs. Check the [Complete Playlist here](https://www.youtube.com/playlist?list=PL6n9fhu94yhW7yoUOGNOfHurUE6bpOO2b). There is a text version also [available on his blog](https://csharp-video-tutorials.blogspot.com/2016/09/aspnet-web-api-tutorial-for-beginners.html).
- Lastly this StackOverFlow [Question](https://stackoverflow.com/questions/20637674/owin-security-how-to-implement-oauth2-refresh-tokens), [Other Question](https://stackoverflow.com/questions/26755573/how-to-implement-oauth2-server-in-asp-net-mvc-5-and-web-api-2) is the only available questions on the whole StackOverFlow which is helpful. I can't believe there is no other question about it. Anyways, most of the answers and comments also link to Taiseer's blog.

## Found Any Bug/ Have a Suggestion?

A PR or any issue is always welcome to this repo. I'm a newbie in the C# world so please forgive me if my code was upto industry standards.

Please open an Issue if you have any doubts or suggestions.
