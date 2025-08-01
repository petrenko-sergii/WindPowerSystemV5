import { NewsComment } from './news-comment';

export interface News {
  id: string;
  title: string;
  author: string;
  chapters: string[];
  createdDt: string; // ISO date string
  updatedDt?: string; 
  comments: NewsComment[];
  likes: number;
}
