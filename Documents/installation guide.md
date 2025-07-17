# Desktop Portal — Installation Guide for IT Administrators

---

## Overview

This document provides step-by-step instructions to deploy and configure the Desktop Portal application on client machines within your organization.

---

## System Requirements

- **Operating System:** Windows 10 or later (64-bit recommended)
- **.NET Runtime:** .NET 6.0 Desktop Runtime or later installed
- **Disk Space:** Minimum 300 MB free space
- **Internet:** Required for accessing the configured portal URL

---

## Pre-Deployment Checklist

1. Verify target machines meet system requirements.
2. Ensure the farm portal website URL is accessible from the target network.
3. Confirm users have appropriate Windows accounts and permissions.

### 🔧 WebView2 Runtime Installation (REQUIRED)

The application uses **Microsoft Edge WebView2** for embedded browser functionality. This runtime must be present on the target machine.

#### ✅ Option 1: Evergreen WebView2 Installer (Recommended)

Download and install the Evergreen WebView2 Runtime once per machine:

- **Installer Link:** 
 [https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section](https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section) 
 > Choose the **"Evergreen Standalone Installer"** for **64-bit** systems.

#### ✅ Option 2: Let Windows Update Handle It

On most recent Windows 10+ machines, WebView2 comes preinstalled via Windows Update. 
You can confirm by checking for the presence of:

If not found, use the installer mentioned above.

#### 🔍 How to Verify Installation

Open a terminal (`cmd`) and run:

	%ProgramFiles(x86)%\Microsoft\EdgeWebView\Application\msedgewebview2.exe


---

## Installation Steps

1. **Distribute Application Files** 
  Copy the entire release folder containing the following to the target machine: 
  - `DesktopPortal.exe` (main executable) 
  - `icon.ico` (application icon, replaceable) 
  - Any other required files/folders in the release directory

2. **.NET Desktop Runtime Installation** 
  Ensure the target machine has .NET 6.0 Desktop Runtime installed. If not, download and install from: 
  [https://dotnet.microsoft.com/en-us/download/dotnet/6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

3. **Place Files in Desired Location** 
  It is recommended to place the folder on a local drive with read/write permissions for the user, e.g., `C:\Program Files\DesktopPortal` or `C:\Users\<User>\Apps\DesktopPortal`.

4. **Create Shortcuts** 
  Create desktop or start menu shortcuts pointing to `DesktopPortal.exe` for ease of access.

5. **(Optional) Configure App Settings** 
  Currently, the app URL, window title, and icon can be configured by replacing the `config.json` and `icon.ico` files in the release folder if required by your environment.

---

## Usage Notes for Users

- The application opens the configured portal URL in a dedicated window.
- Users can enable **Auto Login** to securely save their credentials using Windows Hello.
- Credentials are stored encrypted and accessible only to the logged-in Windows user.

---

## Maintenance

- To update the app, replace the executable and supporting files with the new release.
- To reset saved credentials, instruct users to toggle the **Auto Login** off and on again.
- Monitor the release notes for any updates or security patches.

---

## Troubleshooting

- **App does not start:** Verify .NET Desktop Runtime is installed and files are intact.
- **Portal not loading:** Check network connectivity and URL accessibility.
- **Auto Login fails:** Confirm Windows Hello is configured and functional on the device.

---

## Contact

For further assistance, contact the application developer or IT support team.

---

*End of Installation Guide*
