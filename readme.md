# Summary

The Siren of Shame USB device historically 
only runs from a PC running 
the Siren of Shame [desktop app](https://github.com/AutomatedArchitecture/SirenOfShame).  Mac and
Linux users, or anyone wanting to run the monitor on an inexpensive dedicated device, were mostly out of luck.

For the brave there was [node-sos-device](https://github.com/AutomatedArchitecture/node-sos-device), 
which allows Siren of Shame to work from a 
Raspberry Pi.  However, that client didn't have a 
display, and lacked achievements, reputation, audible alerts, My CI, Team 
CI Pro and a host of other features provided by 
the desktop client.

Siren of Shame UWP is a cross platform replacement for the desktop siren of 
shame client. It monitors builds, tracks achievements and reputation, and powers the 
siren device, all from a Raspberry Pi running Windows IoT.  It is also a port of the Siren 
of Shame [desktop app](https://github.com/AutomatedArchitecture/SirenOfShame).

# Project Structure

There are a large number of projects to facilitate
flexibility with respect to running this project on a Raspberry Pi, on a Desktop PC, and eventually with Xamarin for Mac.

#### IotWeb.Common (Portable)

Portable (PCL) project for IotWeb dependency that provides a web sockets based web server for Windows IoT.

#### IotWeb.Server (Universal Windows)

UWP project for IotWeb dependency that provides a web sockets based web server for Windows IoT.

#### SirenOfShame.Uwp.Background

A UWP Background Project that runs _headless_ on
a Raspberry Pi.  It serves two primary purposes:

First, it runs a web server based on
[IotWeb](https://github.com/sensaura-public/iotweb).  This
web site serves as the admin portal for the app.  It
allows users to, for example, add/delete/change CI servers.  It 
serves up static files from `\wwwroot`, 
and uses web sockets to communicate in realtime with 
browsers.  The html and javascript content originates from the SirenOfShame.Uwp.Web 
project.  The SirenOfShame.Uwp.Server project contains all of
the server-side logic for the web server.

The second purpose of the SirenOfShame.Uwp.Background 
project is to monitor CI servers and process the results.  It
tracks achievements and reputation, tells the siren device when to
turn on or off, and notifies the UI to update itself.  All of this
logic lives in SirenOfShame.Uwp.Watcher.

#### SirenOfShame.Uwp.MessageRelay

MessageRelay receives messages from either
SirenOfShame.Uwp.Background or SirenOfShame.Uwp.Ui and then
relays the messages back out to other listeners.

#### SirenOfShame.Uwp.Server

Since SirenOfShame.Uwp.Background can only run on a Raspberry Pi, 
the SirenOfShame.Uwp.Server UWP library contains all logic for 
processing admin site requests with platform agnostic code.

#### SirenOfShame.Uwp.Tests

Unit tests for all of the various UWP projects.

#### SirenOfShame.Uwp.TestServer

A UWP App that duplicates the functionality in SirenOfShame.Uwp.Background 
on an x86 PC rather than a Raspberry Pi.  This project can be 
used in conjunction
with SirenOfShame.Uwp.Web for testing the admin portal without 
deploying to a Raspberry Pi thereby avoiding time consuming
deploys and speeding up development.

_However_, there is a limitation of UWP that must be overcome to 
get the web app to successfully connect with the
back end via WebSockets.  On UWP projects, it is disallowed
to hit a UWP endpoint with "localhost", "127.0.0.1" or anything
related.  To avoid that you can spin up a VM and browse to the
web project from the VM.  Then hit the web browser is using 
http://computername:8080 (or whatever) thus avoiding the limitation.

#### SirenOfShame.Uwp.Ui

Displays build information for a projector or touch display. Receives
data from SirenOfShame.Uwp.Background.

#### SirenOfShame.Uwp.Watcher

A Portable Class Library that contains all of the ported code from
the SoS Desktop App for monitoring CI servers, tracking reputation,
achievements, build events, and processing rules via the rules engine.

#### SirenOfShame.Uwp.Web

An ASP.Net Core web project that that is the origin
of the administrative web portal of the siren of 
shame raspberry pi project.

To deploy initially run `gulp deploy` (generally only required 
once).  Then run without debugging in Visual Studio.  After 
that `gulp watch` will continually monitor
and compile all typescript files.

To deploy to the pi run `gulp pideploy` which
will copy the Angular 2 based web app into SirenOfShame.Uwp.Background/wwwroot/

# Development Environment

There are two main ways to work:

1. On a Raspberry Pi, in 'production'
2. On a PC in 'mock' mode

The second option is trickier to set up, but has the fastest dev feedback cycle.

## 'Production' Environment Setup

For first time setup follow these instructions.  You can skip various steps
for subsequent deploys.

1. Deploy SirenOfShame.Uwp.MessageRelay so UI can communicate with Background App

    1. Set SirenOfShame.Uwp.MessageRelay as startup
    2. Solution Platform -> ARM
    3. Set Project Properties -> `Debug` -> `Remote Machine` to you're PI's IP address or WINS name
    4. Right click Deploy
    5. Should see "Deploy succeeded"

2. Run `gulp pideploy` from Siren.Uwp.Web (optionally from the Task Runner Explorer window) to ensure SirenOfShame.Uwp.Server contains latest

3. Open port 80 on the Raspberry Pi by running `netsh advfirewall firewall add rule name="Open Port 80" dir=in action=allow protocol=TCP localport=80` from 
"Run Command" in the Windows Device Portal

4. Deploy SirenOfShame.Uwp.Background to Raspberry Pi

   1. Set SirenOfShame.Uwp.Background as startup
   2. Solution Platform -> ARM
   3. Set Project Properties -> `Debug` -> `Remote Machine` to you're PI's IP address or WINS name
   4. Start With Debugging (F5)
   5. If no errors either Start Without Debugging Ctrl-F5 or start 'SirenOfShame.Uwp.Background' from the Raspberry PI's App Manager web interface

5. Navigate to http://[raspiurl]/ and add a Mock Server

6. Deploy SirenOfShame.Uwp.Ui

    1. Set SirenOfShame.Uwp.Ui as startup
    2. Solution Platform -> ARM
    3. Set Project Properties -> `Debug` -> `Remote Machine` to you're PI's IP address or WINS name
    4. Start With Debugging (F5)

## Mock Environment Setup

For first time setup follow these instructions to get Mock mode working.

#### Prerequisites:
UWP-based servers can not be hit via localhost, so:

* Set up a VM or other computer that can access your dev machine
* Open up port 8080 for your web server

#### Steps
1. Set SirenOfShame.Uwp.Web as startup project and Start Without Debugging.
2. Set SirenOfShame.Uwp.TestServer up as startup project and Start Without Debugging
3. Hit http://[mycomputer]:8080/ from another machine
4. Add a 'Mock' CI Server via the menu on the web page
5. Set SirenOfShame.Uwp.MessageRelay as startup project and right click + deploy
6. Set SirenOfShame.Uwp.Ui as startup project and Start it
7. Click 'Mock Server' from the web page to throw mock CI info into the UWP UI
