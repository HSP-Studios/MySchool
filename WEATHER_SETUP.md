# Weather Feature Setup

The weekend weather display requires an OpenWeatherMap API key.

## Setup Instructions:

1. **Get a free API key:**
   - Go to [OpenWeatherMap](https://openweathermap.org/api)
   - Sign up for a free account
   - Navigate to API Keys section
   - Copy your API key

2. **Add the API key to the code:**
   - Open `MySchool\Classes\WeatherService.cs`
   - Replace `YOUR_OPENWEATHERMAP_API_KEY` with your actual API key on line 18:
     ```csharp
     private const string ApiKey = "your_actual_api_key_here";
     ```

3. **Enable Windows Location Services:**
   - Open Windows Settings
   - Go to Privacy & Security > Location
   - Enable "Location services"
   - Allow apps to access your location
   - Ensure MySchool.exe is allowed when prompted

## Features:

- Weather displays only on weekends (Saturday & Sunday)
- Uses Windows location services to get your current location
- Shows current temperature and weather description
- Background gradient changes based on weather conditions:
  - ?? **Clear**: Yellow to orange gradient
  - ?? **Cloudy**: Gray to blue-gray gradient
  - ??? **Rain**: Dark blue to blue gradient
  - ?? **Snow**: Light blue gradient
  - ?? **Thunderstorm**: Dark purple to gray gradient

## Troubleshooting:

- If weather shows "Unable to load weather", check:
  - Your API key is correctly set
  - Windows location services are enabled
  - You have an internet connection
  - The OpenWeatherMap API is accessible
