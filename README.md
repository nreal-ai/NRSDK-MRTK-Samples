# NRSDK-MRTK-Samples
A sample project providing compatibility with NRSDK features such as hand tracking, to Microsoft's Mixed reality Toolkit (MRTK).


## Overview

This github repo ([https://github.com/nreal-ai/NRSDK-MRTK](https://github.com/nreal-ai/NRSDK-MRTK)) contains extension that adds compatibility for NRSDK, including hand tracking and controller support, to Microsoft's open source Mixed Reality Toolkit ([MRTK](https://docs.microsoft.com/en-us/windows/mixed-reality/develop/unity/mrtk-getting-started)) for Unity. It also contains two open-source sample apps from Microsoft's Mixed Reality Design Labs - Hand Tracking Hub (originally as MRTK examples hub) and Surfaces, which demonstrates NRSDK's hand tracking algorithmic capabilities with MRTK's input system.

**Environment:**

* Unity 2020.3.X
* MRTK 2.8.0
* NRSDK 1.9.3

## Examples and Scene Settings

This repo includes two projects, each containing several pre-configured scenes:

* **Assets/HandTrackingDemo**
  * HandTrackingHub (Manager scene)
  * HandTrackingHubMainMenu
  * Clipping
  * ColorPicker
  * Dock
  * HandInteraction
  * HandMenu
  * HandMenuLayout
  * Joystick
  * NonNativeKeyboard
  * PressableButton
  * ScrollingObjectCollection
* **Assets/Surfaces**
  * StartupScene (Manager scene)
  * Bubble
  * Ephemeral
  * Flock
  * Goop
  * Growbies
  * Lava
  * Lighting
  * Volume
  * Xylophone

These scenes do not require additional configuration and serve as a blueprint for Nreal's MRTK integration. **HandTrackingHub** and **StartupScene** works as manager scene (set via MRTK's Scene System) of each project respectively.

## Features

This following MRTK Features are supported:

Supported Features in MRTK

* Hand Tracking
* Hand Meshing
* Hand Skeleton / Gestures
* Nreal Phone Controller / Dev Kit Controller
* Gaze Input (aka Eye Tracking)

Unsupported Features in MRTK

* Planes Detection
* Native Keyboard
* Voice Input (Using Google speech to text)
* Scene Meshing

##
