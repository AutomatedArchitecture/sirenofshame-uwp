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

#### Prerequisites:

* Install a standard Windows IoT image onto a Raspberry Pi using [IoT Core Dashboard](https://developer.microsoft.com/en-us/windows/iot/getstarted/prototype/setupdevice)

#### Steps:

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

#### Debugging Tips

If you get an error when doing an F5 deploy of the UI app to a Raspberry Pi like:

"Unable to activate windows store app the application appears to already be running"

Then try setting some other app as the startup app (e.g. IoTUAPOOBEForeground, it doesn't seem to do anything).

## Mock Environment Setup

For first time setup follow these instructions to get Mock mode working.

#### Prerequisites:
UWP-based servers can not be hit via localhost, so:

* Set up a VM or other computer that can access your dev machine
* Open up port 8080 for your web server

#### Steps
1. Set SirenOfShame.Uwp.Web as startup project and Start Without Debugging.  Note: if you navigate to the web server on from you dev machine the site will come up, but it will not be able to communicate with the back-end over web sockets (which happens to be on port 8001)
2. Set SirenOfShame.Uwp.TestServer up as startup project and Start Without Debugging
3. Hit http://[mycomputer]:8080/ from another machine
4. Add a 'Mock' CI Server via the menu on the web page
5. Set SirenOfShame.Uwp.MessageRelay as startup project and right click + deploy
6. Set SirenOfShame.Uwp.Ui as startup project and Start it
7. Click 'Mock Server' from the web page to throw mock CI info into the UWP UI

# Production Deploy

The siren of shame PI app gets published onto an .ffu image that can be burned to an 
SD card.  Generating an ffu image requires a separate machine and is somewhat involved.
Burning an ffu image is pretty straightfoward, but requires a PC.

## Prerequisites

* Install the Windows Assessment and Deployment Kit (ADK) and everything else listed here: [Get the tools needed to customize Windows IoT Core](https://docs.microsoft.com/en-us/windows-hardware/manufacture/iot/set-up-your-pc-to-customize-iot-core)
* Clone the [sos-uwp-pi branch](https://github.com/AutomatedArchitecture/iot-adk-addonkit/tree/sos-uwp-pi) of AutomatedArchitecture/iot-adk-addonkit

## To Generate FFU

1. First generate the appx from *SirenOfShame.Uwp.Ui* and *SirenOfShame.Uwp.Background* via right click -> Store -> Create App Packages (maybe send it to an USB Drive)
1. Copy the appx and cer files from the USB Drive to `\Source-arm\Packages\Appx.SosUi\` and `\Source-arm\Packages\Appx.SosBackground\`
1. Clean: Delete everything in the output directory `\Build\SirenOfShame`
1. Run `IoTCoreShell.cmd`, select ARM
1. One time only: 
   1. `installoemcerts`
   1. Build the Raspberry Pi Board Support Packages (e.g. `c:\BSP\build.cmd`) see Build a Raspberry Pi BSP in [Lab 1a](https://docs.microsoft.com/en-us/windows-hardware/manufacture/iot/create-a-basic-image)
1. Remember to remove external drives, in particular the SD Card you intend to burn to
1. `buildimage SirenOfShame test`
1. Veriy output file at Build\arm\SirenOfShame\test\Flash.ffu

## Burn the FFU

1. Insert an SD Card
1. Either burn the resulting ffu to the SD Card via GUI with the [IoT Core Dashboard](https://developer.microsoft.com/en-us/windows/iot/getstarted/prototype/setupdevice)
1. Or better `flashsd SirenOfShame test 1` where 1 is the drive number as determined by `diskmgmt.msc` (i.e. if it says **Disk 1**  for drive E, F, G, H, then use the value **1**)