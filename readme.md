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

#### SirenOfShame.Uwp.Server

Since SirenOfShame.Uwp.Background can only run on a Raspberry Pi, 
the SirenOfShame.Uwp.Server UWP library contains all logic for 
processing admin site requests with platform agnostic code.

#### SirenOfShame.Uwp.Tests

Unit tests for all of the various UWP projects.

#### SirenOfShame.Uwp.TestServer

A UWP App that duplicates the functionality in SirenOfShame.Uwp.Background 
on a PC rather than a Raspberry Pi.  This project can be used in conjunction
with SirenOfShame.Uwp.Web for testing the admin portal without deploying to
a Raspberry Pi.

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

To deploy to the pi run `gulp pideploy` which
will copy the Angular 2 based web app into SirenOfShame.Uwp.Background/wwwroot/

