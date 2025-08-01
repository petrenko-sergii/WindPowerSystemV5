import { Component } from '@angular/core';
import { News } from './news';
import { NewsService } from './news.service';

@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrl: './news.component.scss'
})
export class NewsComponent {
  news?: News;

  constructor(private newsService: NewsService) {}

  get(id: string): void {
    this.newsService.get(id).subscribe({
      next: (data) => {
        this.news = data;
      },
      error: (err) => {
        console.error('Failed to fetch news:', err);
      }
    });
  }
}
