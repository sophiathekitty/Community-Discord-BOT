[![Build Status](https://travis-ci.org/petrspelos/Community-Discord-BOT.svg?branch=master)](https://travis-ci.org/petrspelos/Community-Discord-BOT)

# Miunie - the Co**MIUNIE**ty Discord Bot

## About Miunie

Hello, I'm Miunie!

I'm a commiunity bot for Discord written in C# using the [Discord .NET library](https://github.com/RogueException/Discord.Net).

## Getting Miunie

To host Miunie, you can just download the source code either by cloning the repository or downloading the ZIP version.

Miunie was developed using **.NET Core 2.0** with **Visual Studio Community 2017**. These tools are recommended for compilation of the project.

After running Miunie for the first time, an Error might display, saying your bot token might not be configured. 

If you're developing/testing, you might want to set your token as an argument. You can do that by going to your project settings (Solution Explorer > Right click [ProjectName] > Properties > Debug > Application arguments) and pasting your bot token in there. There is a `.gitignore` entry in place so that you don't accidentally commit your token to the project.

If you're not developing and want to use Miunie as a finished product, after getting the Token not configured message, find the directory from which Miunie is running. There should be a `resources` directory containing `config.json` this is a JSON structured text file, that you can edit using Notepad or some other text editor. Once open, edit the Token to use your bot's token. Save the file and either hit retry in Miunie or run the application again.

## How to contribute

### I'm new to Git, GitHub / This is my first project

If you're new, you can checkout the following tutorial:

[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/85s_-i4hHbM/0.jpg)](https://youtu.be/85s_-i4hHbM)

### I know what I'm doing in terms of Git, GitHub

* Generally, you are free to contribute and improve any feature you might like.
* Try to follow the style we are currently using and make sure your code is understandable and commented if need be.
