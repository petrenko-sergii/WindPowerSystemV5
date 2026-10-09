export interface CurrentWeather {
  city: string;
  country?: string;
  latitude: number;
  longitude: number;
  temperatureC: number;
  feelsLikeC: number;
  humidityPercent: number;
  precipitationMm: number;
  windSpeedKmh: number;
  conditions: string;
}

export interface ClothingItem {
  category: string;
  item: string;
  reason: string;
}

export interface ClothingAdvice {
  summary: string;
  items: ClothingItem[];
  umbrellaNeeded: boolean;
  sunProtectionNeeded: boolean;
}

export interface WeatherClothing {
  weather: CurrentWeather;
  advice: ClothingAdvice;
}
