# Discord Community bot

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/petrspelos/Community-Discord-BOT/blob/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/ykcjkkhik31ssb2g/branch/master?svg=true)](https://ci.appveyor.com/project/discord-bot-tutorial/community-discord-bot/branch/master)
[![Coverage Status](https://coveralls.io/repos/github/discord-bot-tutorial/Community-Discord-BOT/badge.svg?branch=master)](https://coveralls.io/github/discord-bot-tutorial/Community-Discord-BOT?branch=master)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/27b59040bfab4a79b9d1fbfc41777f52)](https://www.codacy.com/app/petrspelos/Community-Discord-BOT?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=discord-bot-tutorial/Community-Discord-BOT&amp;utm_campaign=Badge_Grade)
[![Open Source Helpers](https://www.codetriage.com/petrspelos/community-discord-bot/badges/users.svg)](https://www.codetriage.com/petrspelos/community-discord-bot)

Hello, I'm Miunie the com**MIUNIE**ty bot. :wave:

![Miunie Example](https://media.giphy.com/media/4WFhEh2CtJoo3k5yfF/giphy.gif)

I'm a community bot for Discord written in C# using the [Discord .NET library](https://github.com/RogueException/Discord.Net).

The goal of this project is to create a solid general-purpose bot that can be easily extended and used in new and interesting ways.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See [deployment](#deployment) for notes on how to deploy the project on a live system.

### Prerequisites

* The recommended IDE is [Visual Studio 2017 Community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15).
* Using **Visual Studio Installer** make sure you installed `.NET Core cross-platform development`.
    * Visual Studio Installer should be automatically installed with VS 2017 Community.

### Installing

This is a step by step guide to get Miunie ready on your machine and ready for development.

**Getting the source**
1. [Fork the repository](https://help.github.com/articles/fork-a-repo/).
2. Navigate to your fork.
3. [Clone](https://help.github.com/articles/cloning-a-repository/) your fork to your local machine.

**Setting up the environment**

* The root directory of the project contains `CommunityBot.sln`, this is a Visual Studio solution file and you can open it with Visual Studio (see [prerequisites](#prerequisites)).

* After the solution is loaded, right-click the CommunityBot project through the Solution Explorer in Visual Studio _(It has a little C# in a green box icon by default)_ and go to Properties. Under **Debug**, you will see an `Application arguments:` field. You can paste your [bot token](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token) there. Application arguments are already added to `.gitignore` so you don't have to worry about accidentally making it public.

* Once you save your changes from the previous step, you can compile and run the application.
    * In Visual Studio, a common way of doing this is with the `F5` or `Ctrl + F5` shortcut.

* Try it out
    * The bot you have assigned with your token should now be online. Try to mention him and say hello!

```

@MyBot Hello

```
If you get a response back, everything is ready for development.

## Running the tests

To run Unit Tests in Visual Studio, you use the `Ctrl + R, A` shortcut or go to `Test > Run > All Tests`.

## Deployment

To publish a version of Miunie and run it on a machine, you can use [this tutorial](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish?tabs=netcore2x) that walks you through the process.

## Built With

* [.NET Core 2.0](https://docs.microsoft.com/en-us/dotnet/core/) - Platform used
* [Discord .NET](https://github.com/RogueException/Discord.Net) - Discord API wrapper library
* :heart: Love and Care :blue_heart:

## Contributing

Please read [CONTRIBUTING.md](https://github.com/petrspelos/Community-Discord-BOT/blob/master/CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

There is currently no versioning system used. Feel free to create a new Issue suggesting one.

## Authors

* **Petr Sedláček** - *Initial work* - [PetrSpelos](https://github.com/petrspelos)

See also the list of [contributors](https://github.com/petrspelos/Community-Discord-BOT/graphs/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* This project is possible thanks to the amazing Discord .NET library.
* Thank you to all people who contributed, especially those coming from my Discord tutorial.
* If you're still not sure about the way to contribute, there's a simple [tutorial video](https://youtu.be/85s_-i4hHbM) I made.
