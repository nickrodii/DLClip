<div align="center">

# DLClip

**A lightweight GUI wrapper for FFmpeg and yt-dlp designed for efficiency.**<br>
*Download, trim, and process media files without using a command-line.*

[![.NET](https://img.shields.io/badge/.NET_8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![FFmpeg](https://img.shields.io/badge/FFmpeg-Tool-00599C?style=for-the-badge&logo=ffmpeg&logoColor=white)](https://ffmpeg.org/)
[![yt-dlp](https://img.shields.io/badge/yt--dlp-cli-c4302b?style=for-the-badge&logo=youtube&logoColor=white)](https://github.com/yt-dlp/yt-dlp)

</div>

---

## Overview

**DLClip** is a Windows desktop application built in C# with .NET and WPF that provides a user-friendly GUI for FFmpeg and yt-dlp, eliminating the use of the terminal, allowing many common Windows users utilize FFmpeg and yt-dlp's features.

**NOTE: This project is not finished and little to no features actually work within the program right now.**

## Key Features

* **Universal Downloader:** Seamlessly integrates with `yt-dlp` to import video or audio files from thousands of websites on the web.
* **Length trimming:** Length trim controls that allows the user to trim any video or audio file down to any length.
* **Quality Resizing:** Resolution, FPS, and bitrate scaling to decrease file sizes, with a custom option to allow any aspect ratio provided.
* **Format Conversion:** Format and codec conversion for changing the file format / extension of any media file.
* **Audio Extraction:** Extract audio from any video file with a single click.
* **Media Analysis:** Automatically probes input files to display codecs, bitrates, and framerates.
* **Auto-Configuration:** Detects system installations of FFmpeg and yt-dlp, or guides you through the setup to link the installations to the software.

## Getting Started

### Required Tools
DLClip is a GUI wrapper, which means that it uses these CLI tools to perform its tasks. The setup of the program provides a guide on installing these tools, but you can find them now here:
1.  **[FFmpeg](https://ffmpeg.org/download.html)** (for processing conversions and probing media info)
2.  **[yt-dlp](https://github.com/yt-dlp/yt-dlp)** (for importing from the web)

### Installation
DLClip is currently work-in-progress and very little features are available right now. There will be a proper way to download the program in an .exe or .msi format once the project is finished, 
but if you'd like to test out the program now, you can follow these steps:
1.  Clone the repository:
    ```bash
    git clone https://github.com/nickrodii/DLClip.git
    ```
2.  Open the solution in **Visual Studio**.
3.  Build and Run.
4.  **First Run:** The app will check for `ffmpeg` and `yt-dlp`. If they aren't in your PATH, you can point DLClip to their folders in the Settings menu.
