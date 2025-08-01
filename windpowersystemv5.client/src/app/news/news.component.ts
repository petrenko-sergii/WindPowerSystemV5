import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { News } from './news';
import { NewsService } from './news.service';

@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrl: './news.component.scss'
})
export class NewsComponent implements OnInit {
  news?: News;

  constructor(
    private newsService: NewsService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.get(id);
      }
    });
  }

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
