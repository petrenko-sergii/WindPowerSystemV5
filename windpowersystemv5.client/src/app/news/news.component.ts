import { Component, OnInit } from '@angular/core';
import { News } from './news';
import { NewsService } from './news.service';

@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrl: './news.component.scss'
})
export class NewsComponent implements OnInit {
  newsList: News[] = [];

  constructor(private newsService: NewsService) {}

  ngOnInit(): void {
    this.getAllNews();
  }

  getAllNews(): void {
    this.newsService.getAllNews().subscribe({
      next: (data) => {
        this.newsList = data;
      },
      error: (err) => {
        console.error('Failed to fetch news:', err);
      }
    });
  }
}
