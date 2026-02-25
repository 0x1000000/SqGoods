import { animate, query, style, transition, trigger } from '@angular/animations';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, Routes, UrlSegment } from '@angular/router';
import { ComponentState, ComponentStateDiff, initializeStateTracking, With } from 'ng-set-state';

type TabData = {
  label: string,
  href: string,
  component: any
};

type State = ComponentState<RouterTabsComponent>;
type NewState = ComponentStateDiff<RouterTabsComponent>;

@Component({
  selector: 'sq-app-router-tabs',
  templateUrl: './router-tabs.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [
    trigger('routeTransition', [
      transition('* <=> *', [
        query(':leave', [
          style({opacity: 1, transform: 'translateY(0)'}),
          animate('120ms ease-in', style({opacity: 0, transform: 'translateY(8px)'}))
        ], {optional: true}),
        query(':enter', [
          style({opacity: 0, transform: 'translateY(8px)'}),
          animate('220ms cubic-bezier(.22,.61,.36,1)', style({opacity: 1, transform: 'translateY(0)'}))
        ], {optional: true})
      ])
    ])
  ]
})
export class RouterTabsComponent {
  constructor(public router: Router, private readonly _cd: ChangeDetectorRef, activatedRoute: ActivatedRoute) {
    initializeStateTracking(this, {onStateApplied: () => this._cd.detectChanges()});
    const self = this;
    activatedRoute.url.subscribe(u => {
      let diff = false;
      if (self.path != null && self.path.length === u.length){
        for (let i = 0; i < self.path.length; i++){
          if (self.path[i].path !== u[i].path){
            diff = true;
            break;
          }
        }
      }
      else{
        diff = true;
      }

      if (diff){
        self.path = u;
      }
    });

    this.router.events.subscribe(e => {
      if (e instanceof NavigationEnd){
        this.routeAnimationState++;
        this._cd.markForCheck();
      }
    });
  }

  @Input()
  path: UrlSegment[]|null = null;

  tabs: TabData[] = [];

  selectedTabId = '';

  activatedComponent: any = null;

  routeAnimationState = 0;

  @With('path', 'router')
  static buildTabs(state: State): NewState {
    let tabs: TabData[];

    if (state.path == null){
      tabs = [];
    }
    else{
      const paths = state.path;
      const currentRoutes = getCurrentRoutes(state.router.config, paths, 0);

      if (currentRoutes != null){
        tabs = currentRoutes
          .filter(c => c.data?.title && c.component)
          .map(r => ({
            label: r.data!.title,
            href: state.path + '/' + r.path!,
            component: r.component!}));
      }
      else{
        tabs = [];
      }
    }

    return {tabs};

    function getCurrentRoutes(routes: Routes, paths: UrlSegment[], offset: number): Routes| null{
      if (paths.length - offset < 1 || paths[offset].path === ''){
        return routes;
      }
      const segment = paths[offset];

      const res = routes.find(r => r.path === segment.path);

      if (res == null || res.children == null){
        return null;
      }
      return getCurrentRoutes(res.children, paths, offset + 1);
    }
  }

  @With('tabs', 'activatedComponent')
  static setSelectedTabId(state: State): NewState {
    if (state.tabs != null && state.activatedComponent != null){
      const t = state.tabs.find(ti => ti.component === state.activatedComponent.constructor);
      if (t) {
        return {selectedTabId: t.href};
      }
    }
    return {selectedTabId: ''};
  }

  onLinkClick(url: string): void{
    if (this.router.url === url){
      return;
    }
    this.router.navigateByUrl(url);
  }

  onRouteActivate(component: any): void {
    this.activatedComponent = component;
  }
}
