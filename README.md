# Duty Bot

This is a chat bot for Microsoft Teams build during the hackathon: **[HackTogether: The Microsoft Teams Global Hack](https://github.com/microsoft/hack-together-teams)**

![image](https://user-images.githubusercontent.com/45178151/238085882-1a8b3b43-f674-4b35-a737-5a7cd1cf2e0c.jpg)

It is a simple bot designed to help in a group chat or a team in side Microsoft Teams app. It can keep (in-memory for now) a schedule of duties. And when a keyword is detected in the chat it will check the schedule and plug in the conversation.  

Here are the features that are currently implemented:
* Input schedule with markdown:

  ![image](https://github.com/Obelixx/DutyBot/assets/10490848/84205055-9a1d-497d-a96b-128d6a728c06)

* Input schedule with table:

  ![image](https://github.com/Obelixx/DutyBot/assets/10490848/c72aab41-8306-4edb-b4d0-2e2ba9dba228)

* Answer on keywords "duty", "on air", "on watch":

  ![image](https://github.com/Obelixx/DutyBot/assets/10490848/e6a8a43d-0442-49e8-a532-7c8984ac57f8)

* Answer on keyword "duties":

  ![image](https://github.com/Obelixx/DutyBot/assets/10490848/2eef17d8-8f14-4f13-8f97-f48e6931989a)

* If the bot can find the user in the azure active directory (using Microsoft Graph API) it will @mention the user:

  ![image](https://github.com/Obelixx/DutyBot/assets/10490848/e734e5a2-6b69-4acc-a60c-83ee6447fe6f)

* Test functionalites "who is {principal name}"

  ![image](https://github.com/Obelixx/DutyBot/assets/10490848/6a6dc0bd-5b61-43b0-95cd-9ec3cea2c3e1)
