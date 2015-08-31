# Bottom sliding content presenter for Windows Phone Runtime 8.x

Introduction
============
This repository contains a control that provides an ability to slide content from the bottom of the screen by properties or manipulations.

Control has a lot of useful properties like PercentsOfScreenToReveal, BottomContentOffset and of course IsOpen.

We attached a basic example of use.  For more complex example you can look at the jakdojade.pl windows phone application where we implemented it.
Link for download https://www.microsoft.com/en-us/store/apps/jakdojadepl/9wzdncrfhz66

The code is released under the MIT/X11, so feel free to modify and share your changes with the world.

How to
======

This control can be used with both Windows and Windows Phone 8.1 but was tested only with Windows Phone. 

To use this control you have just to add the project to your solution and place control on your page. 
Don't forget to provide both - top and bottom - contents and implement IManipulatorEventListener to enable touch manipulations. 
For more info please look at the sample project.
