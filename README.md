# WebApplication

This web application is created to share some interesting topics (In this case, topic is some block of text with title). Unregistered users can only read topics, while registered users can create their own topics, like and comment all other topics. Also there is admin user, who can delete topics and their comments.

Backend - Rest api using Asp.Net Core with SOLID principles
It has 4 controllers:
1) Account controller for managing user account activity (login, logout, register etc.)
2) Topic controller for managing topics (create, read, delete)
3) Likes controller for managing likes of topic (like, dislike)
4) Comment controller for managing comments of topic (create, read, delete)

Hosting on Azure

Frontend - SPA angular 6 application 
It has 8 components:
1) Account component to see user information and change password
2) Change password component to change password
3) Create topic component to create topic
4) Forgot password component to handle password recovery
5) Login component to login user
6) Register component to register user
7) Start page component to show all topics
8) Topic detail component to show selected topic
hosting on heroku

Security is realized using Bearer JWT

Website - 
https://dublog.herokuapp.com/
