# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture Overview

DingoConfigurator is a C# WPF application for configuring automotive CAN devices including dingoPDM, dingoPDM-Max, CAN boards, and other components. The project uses a modular architecture with four main assemblies:

### Core Projects Structure
- **DingoConfigurator** (main WPF app): MVVM-based UI using MaterialDesign themes, manages device configuration and UI interactions
- **CanDevices** (class library): Device-specific implementations and business logic for PDMs, boards, keypads, etc.
- **CanInterfaces** (class library): Hardware interface abstractions for USB2CAN, PCAN, USB communication
- **CommsHandler** (class library): CAN communication protocol implementation and device management

### Key Architectural Patterns
- **MVVM Pattern**: ViewModels in `DingoConfigurator/ViewModels/`, Views in `DingoConfigurator/Views/`
- **Device Plugin Architecture**: Each device type implements `ICanDevice` interface, allowing for extensible device support
- **CAN Communication Layer**: Abstracted through `CanCommsHandler` which manages multiple device connections
- **Configuration Management**: `.dco` files store device configurations using JSON serialization

### Device Type Hierarchy
- All devices implement `ICanDevice` interface
- Device-specific classes: `DingoPdmCan`, `dingoPdmMaxCan`, `CanBoardCan`, `CanMsgLog`, `SoftButtonBox`
- Each device has associated ViewModels for different UI contexts (Main, Settings, Plots, Keypad1/2)

## Build Commands

**Build Solution:**
```bash
# Navigate to solution directory
cd DingoConfigurator

# Build using MSBuild (requires Visual Studio or Build Tools)
msbuild DingoConfigurator.sln /p:Configuration=Debug
# or
msbuild DingoConfigurator.sln /p:Configuration=Release

# Using dotnet (may require project file updates)
dotnet build DingoConfigurator.sln
```

**Run Application:**
```bash
# From solution directory
.\DingoConfigurator\bin\Debug\DingoConfigurator.exe
# or
.\DingoConfigurator\bin\Release\DingoConfigurator.exe
```

**Clean Build:**
```bash
msbuild DingoConfigurator.sln /t:Clean
msbuild DingoConfigurator.sln /p:Configuration=Debug
```

## Development Environment

- **Target Framework**: .NET Framework 4.8.1
- **UI Framework**: WPF with MaterialDesign themes
- **Plotting**: OxyPlot for real-time data visualization
- **Logging**: NLog with configuration in each project
- **Serialization**: System.Text.Json for device configuration

## Key Configuration Files

- **NLog.config**: Logging configuration in each project
- **App.config**: Application settings and assembly bindings
- **packages.config**: NuGet package references (legacy format)
- **.dco files**: Device configuration files (JSON format)

## Device Communication Flow

1. **Connection**: Select interface (USB2CAN/PCAN/USB) and establish CAN connection
2. **Device Discovery**: Automatic detection of connected devices via CAN messaging
3. **Configuration**: Read/write device parameters through CAN protocol
4. **Real-time Data**: Continuous monitoring and plotting of device telemetry

## UI Features and Functionality

### TreeView Navigation
- Main application uses hierarchical TreeView for device navigation
- Each device type shows sub-pages (Settings, Plots, Keypad1, Keypad2)
- Right-click context menus provide device-specific actions

### Keypad Visibility Management
**Location**: DingoPDM device context menu in TreeView
**Functionality**: Hide/show keypad sub-items in the tree structure
- **Checkable menu items**: "Keypad1" and "Keypad2" with checkbox states reflecting visibility
- **Bulk operations**: "Hide All Keypads" and "Show All Keypads" options
- **Implementation**: 
  - `Keypad.Visible` property (UI-only, marked with `[JsonIgnore]`)
  - `DingoPdmCan.VisibleSubPages` property filters keypads based on visibility
  - Two-way data binding between menu checkboxes and keypad visibility
  - Automatic TreeView updates via PropertyChanged notifications

### DataGrid Button Display
**Issue Resolution**: Fixed DataGrid in KeypadView to show all 20 buttons instead of just 12
- **Root Cause**: DataGrid was binding to `Buttons` property which only showed first 12 buttons
- **Solution**: Updated binding to use `VisibleButtons` property that respects `NumButtons` setting
- **Implementation**: `VisibleButtons` uses `AllButtons.Take(NumButtons)` to show correct number based on keypad model

### SoftButtonBox CAN Keypad Emulator
**Location**: SoftButtonBox device with comprehensive UI in `SoftButtonBoxView.xaml`
**Functionality**: Complete CAN keypad emulation supporting both Blink Marine and Grayhill keypads
- **Architecture**: Polymorphic design with factory pattern
  - `IKeypadEmulator` interface defining emulator contract
  - `KeypadEmulatorBase` abstract class with common functionality
  - `BlinkKeypadEmulator` for Blink Marine keypads (color LEDs, dials, complex encoding)
  - `GrayhillKeypadEmulator` for Grayhill keypads (simple on/off LEDs)
  - `KeypadEmulatorFactory` for creating appropriate emulator instances
- **CAN Message Handling**: Proper protocol implementation
  - **Button States**: BaseCanId + 0x180, packed bit format
  - **LED Control**: BaseCanId + 0x200, vendor-specific encoding (stacked vs padded formats)
  - **Dial States**: BaseCanId + 0x280/0x380, 16-bit rotary encoder values (Blink only)
  - **Heartbeat**: BaseCanId + 0x700, periodic "alive" messages
- **User Interface Features**:
  - Dynamic keypad model selection (dropdown with all 14 models)
  - Real-time button control with toggle behavior and visual feedback
  - Dial sliders for rotary encoder simulation (Blink13Key_2Dial, BlinkRacepad)
  - LED status display with color visualization for Blink keypads
  - Connection status monitoring and automatic message transmission (100ms intervals)
- **Model-Specific Behavior**:
  - **Blink Models**: Support 2-20 buttons, RGB LEDs, brightness control, backlight, dial LEDs
  - **Grayhill Models**: Support 6-20 buttons, simple on/off LEDs, no dials
  - Automatic UI adaptation based on selected model capabilities
- **Implementation Details**:
  - Integrates with existing `SoftButtonBox` class implementing `ICanDevice`
  - Timer-based message transmission via `GetTimerMessages()` and `GetTimerIntervalMs()`
  - Incoming message processing through `Read()` method and emulator `ProcessIncomingMessage()`
  - State management through `SetButtonState()`, `SetDialValue()`, and `Reset()` methods

## Important Notes

- Device configurations are saved as `.dco` files in JSON format
- CAN interfaces require specific drivers (PCAN, USB2CAN adapters)
- Each device type has specific ID ranges and communication protocols
- ViewModels are cached per device to maintain state when switching between devices
- Keypad visibility settings are UI-only and reset when configurations are reloaded
- SoftButtonBox emulator automatically adapts CAN message formats based on keypad model selection
- Blink keypad LED encoding uses two different formats: "stacked" (Blink12Key) vs "padded" (other models)