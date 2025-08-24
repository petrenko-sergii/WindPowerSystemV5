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
        
        // For demonstration purposes when no real news with images exist
        if (this.newsWithImages.length === 0) {
          this.addSampleNewsWithImages();
        }
      },
      error: (err) => {
        console.error('Failed to fetch news:', err);
        // Add sample data when service fails for demonstration
        this.addSampleNewsWithImages();
      }
    });
  }

  private addSampleNewsWithImages(): void {
    // Sample news with placeholder images for carousel demonstration
    // Remove this method once real news data includes imageUrl fields
    const sampleNews: News[] = [
      {
        id: 'demo-1',
        title: 'Wind Energy Innovation',
        author: 'Tech News',
        chapters: ['Latest technology developments'],
        createdDt: new Date().toISOString(),
        comments: [],
        likes: 15,
        imageUrl: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iODAwIiBoZWlnaHQ9IjQwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjNGZiNmM3Ii8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtZmFtaWx5PSJBcmlhbCwgc2Fucy1zZXJpZiIgZm9udC1zaXplPSIyNCIgZmlsbD0iI2ZmZiIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPldpbmQgRW5lcmd5PC90ZXh0Pjwvc3ZnPg=='
      },
      {
        id: 'demo-2', 
        title: 'Sustainable Power Generation',
        author: 'Green Energy',
        chapters: ['Renewable energy trends'],
        createdDt: new Date().toISOString(),
        comments: [],
        likes: 23,
        imageUrl: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iODAwIiBoZWlnaHQ9IjQwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjNjc5N2JjIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtZmFtaWx5PSJBcmlhbCwgc2Fucy1zZXJpZiIgZm9udC1zaXplPSIyNCIgZmlsbD0iI2ZmZiIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPlBvd2VyIEdlbjwvdGV4dD48L3N2Zz4='
      },
      {
        id: 'demo-3',
        title: 'Clean Energy Future',
        author: 'Environmental Report',
        chapters: ['Future of clean energy'],
        createdDt: new Date().toISOString(),
        comments: [],
        likes: 31,
        imageUrl: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iODAwIiBoZWlnaHQ9IjQwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjOGI5ZGM1Ii8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtZmFtaWx5PSJBcmlhbCwgc2Fucy1zZXJpZiIgZm9udC1zaXplPSIyNCIgZmlsbD0iI2ZmZiIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPkNsZWFuIEVuZXJneTwvdGV4dD48L3N2Zz4='
      }
    ];
    
    this.newsWithImages = sampleNews;
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
