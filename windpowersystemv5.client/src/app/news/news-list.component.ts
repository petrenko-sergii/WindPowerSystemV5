import { Component, OnInit, OnDestroy } from '@angular/core';
import { News } from './news';
import { NewsService } from './news.service';

@Component({
  selector: 'app-news-list',
  templateUrl: './news-list.component.html',
  styleUrl: './news-list.component.scss'
})
export class NewsListComponent implements OnInit, OnDestroy {
  newsList: News[] = [];
  newsWithImages: News[] = [];
  currentSlideIndex = 0;
  autoSlideInterval: any;

  constructor(private newsService: NewsService) {}

  ngOnInit(): void {
    this.getAllNews();
    this.startAutoSlide();
  }

  ngOnDestroy(): void {
    if (this.autoSlideInterval) {
      clearInterval(this.autoSlideInterval);
    }
  }

  getAllNews(): void {
    this.newsService.getAll().subscribe({
      next: (data) => {
        this.newsList = data;
        // Filter news that have images for the carousel
        this.newsWithImages = data.filter(news => news.imageUrl && news.imageUrl.trim() !== '');
      },
      error: (err) => {
        console.error('Failed to fetch news:', err);
      }
    });
  }

  previousSlide(): void {
    if (this.newsWithImages.length > 0) {
      this.currentSlideIndex = (this.currentSlideIndex - 1 + this.newsWithImages.length) % this.newsWithImages.length;
    }
  }

  nextSlide(): void {
    if (this.newsWithImages.length > 0) {
      this.currentSlideIndex = (this.currentSlideIndex + 1) % this.newsWithImages.length;
    }
  }

  goToSlide(index: number): void {
    this.currentSlideIndex = index;
  }

  startAutoSlide(): void {
    this.autoSlideInterval = setInterval(() => {
      this.nextSlide();
    }, 5000); // Change slide every 5 seconds
  }

  onSlideClick(): void {
    // Stop auto-slide when user interacts
    if (this.autoSlideInterval) {
      clearInterval(this.autoSlideInterval);
      this.autoSlideInterval = null;
    }
  }
}
