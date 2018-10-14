![Thank you for contributing!](https://i.imgur.com/E5jgNX1.png)
## First Contribution
Welcome to this beginner-friendly guide on how to contribute to this repository! We're glad that you're considering contributing and welcome anyone to contribute regardless of git knowledge. 

We understand that making your first pull request can be very scary. What even is a _pull request_, right? What we're trying to say is that everyone makes mistakes, so don't worry about doing anything "wrong". If you're unsure about something or just have a question in general, feel free to ask on our [Discord Server](https://discord.gg/R9j8DJF) or in the appropriate comment section. We welcome **any contribution** no matter how big or small it may be.

## Resources

* [Git and GitHub for Poets](https://www.youtube.com/playlist?list=PLRqwX-V7Uu6ZF9C0YMKuns9sLDzK6zoiV) - _A youtube series that goes through basic concepts of git and GitHub._
* [First time contributing](https://egghead.io/courses/how-to-contribute-to-an-open-source-project-on-github) - _Free course walking you through your first contribution on GitHub._
* [Discord C# Community bot - How to contribute](https://youtu.be/85s_-i4hHbM) - _A video walkthrough on git basics and how to contribute to this repository._

## How to get started
If you're completely new to git and/or GitHub, we highly suggest checking out the links under [Resources](#resources).

Before starting, make sure that...
1. You have [git](https://git-scm.com/downloads) installed
2. You have a basic understanding of how git and GitHub works
3. (optional) You have [GitHub Desktop](https://desktop.github.com/) installed

## Instructions
_Try clicking the image if you don't understand a step. Still don't understand? Ask us on our [discord server](https://discord.gg/R9j8DJF)!_

### 1. Fork the repository
_Don't know what a fork is? (other than the kitchen utensil_ :wink:_) Take a look in [Resources](#resources) before continuing_ :smile:
[![Fork the repository](https://github-images.s3.amazonaws.com/help/bootcamp/Bootcamp-Fork.png)](https://guides.github.com/activities/forking/#fork)

### 2. Clone your forked repository
This can be done very easily with [GitHub Desktop](https://desktop.github.com/) but it works just as well in the command line. Choose whichever one you prefer.

#### For Github Desktop:
[![Clone with GitHub Desktop](https://services.github.com/on-demand/images/gifs/github-desktop/clone-repository-locally.gif)](https://services.github.com/on-demand/github-desktop/clone-repository-github-desktop)

#### For command line:
Clone it into the desired folder by using 
`git clone https://github.com/{your-username}/Community-Discord-BOT.git`

### 3. Make your changes
Now that you have the files locally on your PC you can do your changes on those files.

### 4. Staging your files
After modifying the files you want to modify, you need to tell git which changes you want to commit. 

#### For GitHub Desktop:
[![Including the files in the commit](https://i.imgur.com/mfWIwla.png)](https://services.github.com/on-demand/github-desktop/add-commits-github-desktop)

#### For command line:
* Change the working directory to the project's directory by using `cd Community-Discord-BOT.git`
* Add one file by using `git add {file-path}` **or** add all files by using `git add .`

### 5. Commit your changes
Now it's time to commit your staged files to git. This will not upload anything to GitHub, we are still working locally.

Here are some good things to think about when creating a commit:
* Always provide a short description of what your commit will change.
* Write the commit message in present tense
    * Correct: "_Add a file_", "_Modify a method_", "_Change the structure of DataClass_" .. etc
    * Incorrect: "_Added a file_", "_Modifying a method_", "_Changes the structure of DataClass_"

:bulb: _An easy way to see if the commit message is correct is by placing it in the sentence "This commit will \_\_\_\_\_\_\_\_\_\_\_\_."_

#### For GitHub Desktop:
_Please ignore the commit message here: it is not written in present tense._
[![Commiting files in GitHub Desktop](https://services.github.com/on-demand/images/gifs/github-desktop/making-commits-locally.gif)](https://services.github.com/on-demand/github-desktop/add-commits-github-desktop)

#### For command line:
Commit your staged files with a commit message using `git commit`. This will open up an editor, allowing you to include a message with your commit.

### 6. Push your changes
When you've done one (or more) commits and you want to upload your commits to your forked repository, you need to push the changes to GitHub. This will only upload them to **your own fork**, and therefore not affect the main project's files, so you don't have to worry about doing anything wrong :smile:

#### For GitHub Desktop:
[![Pushing to your repository](https://i.imgur.com/NydmYx0.png)](https://services.github.com/on-demand/github-desktop/add-commits-github-desktop)

:bulb: _The small number next to the grey arrow indicates how many commits you will be pushing at once._

#### For command line:
Push the changes to the remote using `git push`

### 7. Create a pull request
Now it's finally time to ask Miunie if she'd like to take your changes, or in other words, _**request** Miunie to **pull** your changes._

This is where your version will merge with the main version. 

* Navigate to the [original repository](https://github.com/discord-bot-tutorial/Community-Discord-BOT)

* Click "**Pull requests**" and then "**New pull request**"
![Opening a new pull requst](https://i.imgur.com/e7lN5wI.png)

* Click "**compare across forks.**"
![Clicking compare across forks](https://i.imgur.com/tEgjhRn.png)

* On the right hand side of the arrow, choose your forked repository as the **Head Repository**. This repository will be called `{your-username}/Community-Discord-BOT`.
![Choosing a Head Repository](https://i.imgur.com/JI0ONBd.png)

* Almost there! Hit the green button that says "**Create pull request**". This will open up a menu that gives you an opportunity to write a title for your pull request, as well as a comment. The comment should already be filled out with a template - you should fill this in. When you're done, hit the green button labeled "**Create pull request**" one more time, and you're done!
![Creating the pull request](https://i.imgur.com/sUELwWk.png)

### 8. Await review
You did it! Your pull request is now awaiting feedback. It's not uncommon for the reviewer to request some changes to your pull request before merging it with the main version. We value contribution and won't look down on you for "bad" code. The worst thing that can happen is that the reviewer gives you some tips on how to improve what you've written. See it as a learning experience!
