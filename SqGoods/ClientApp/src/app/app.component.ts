import { ChangeDetectionStrategy, Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

type TabData = {
  label: string,
  href: string
};

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {
  readonly title = 'SqGoods';

  readonly tabs: TabData[];

  selectedTabId: string|null = null;

  constructor(private router: Router){
    this.tabs = router.config.filter(c => c.data?.title).map(c => ({label: c.data!.title!, href: '/' + c.path!}));

    router.events.subscribe((e) => {
      if (e instanceof NavigationEnd) {
        const selectedTab = this.tabs.find(t => e.url.toLowerCase().startsWith(t.href));
        this.selectedTabId = selectedTab?.href ?? null;
      }
    });
  }

  onLinkClick(url: string): void{
    this.router.navigateByUrl(url);
  }

  trackByFn(index: number, t: TabData): string{
    return t.href;
  }
}
