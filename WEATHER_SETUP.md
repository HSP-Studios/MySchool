# Weather Feature Setup

The weekend weather display uses the free Open-Meteo API - **no API key required!**

## Setup Instructions:

### Option 1: Automatic Location (Recommended)

1. **Enable Windows Location Services:**
   - Open Windows Settings
   - Go to Privacy & Security > Location
   - Enable "Location services"
   - Allow apps to access your location

2. **Grant Permission in MySchool:**
   - Open MySchool app
   - Go to Settings
   - Click "Request Location Permission"
   - Allow when Windows prompts

### Option 2: Manual Location

1. **Find Your Coordinates:**
   - Go to [Google Maps](https://maps.google.com)
   - Right-click on your location
   - Copy the coordinates (e.g., -27.4698, 153.0251)

2. **Set in MySchool:**
   - Open MySchool app
   - Go to Settings
   - Click "Set Manual Location"
   - Enter latitude and longitude
   - Optionally add a location name
   - Click "Save Location"

## Features:

- Weather displays only on weekends (Saturday & Sunday)
- Shows current temperature and weather description
- Uses Open-Meteo API (free, no API key needed)
- Background gradient changes based on weather conditions:
  - ?? **Clear**: Yellow to orange gradient
  - ?? **Cloudy**: Gray to blue-gray gradient
  - ??? **Rain/Drizzle**: Dark blue to blue gradient
  - ?? **Snow**: Light blue gradient
  - ?? **Thunderstorm**: Dark purple to gray gradient

## Location Settings:

- **Device Location**: Automatically uses your current location (requires Windows location services)
- **Manual Location**: Set a specific location by coordinates (doesn't require location services)
- You can switch between methods at any time in Settings

## Troubleshooting:

- **Weather shows "Unable to load weather":**
  - Check your internet connection
  - If using device location, ensure Windows location services are enabled
  - If using manual location, verify coordinates are correct (latitude: -90 to 90, longitude: -180 to 180)
  
- **"Loading..." never changes:**
  - The Open-Meteo API might be temporarily unavailable
  - Check your firewall isn't blocking api.open-meteo.com

## Data Source:

Weather data provided by [Open-Meteo.com](https://open-meteo.com) - a free weather API with no API key required.
