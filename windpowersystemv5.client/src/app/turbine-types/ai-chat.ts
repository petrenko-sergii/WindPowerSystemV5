export interface TurbineTypeLink {
  id: number;
  manufacturer: string;
  model: string;
  capacity: number;
  turbineQty: number;
  url: string;
}

export interface AiChatAnswer {
  answer: string;
  turbineTypes: TurbineTypeLink[];
}
