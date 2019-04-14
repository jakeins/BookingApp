import { Component, OnInit } from '@angular/core';
import { Breadcrumb } from '../../models/breadcrumb';
import { ActivatedRoute, Router, NavigationEnd, UrlSegment } from '@angular/router';
import { Logger } from '../../services/logger.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'breadcrumbs',
  templateUrl: './breadcrumbs.component.html',
  styleUrls: ['./breadcrumbs.component.css']
})
export class BreadcrumbsComponent implements OnInit {

  constructor(
    private router: Router,
    private route: ActivatedRoute
  ) { }

  breadcrumbsInternal: Breadcrumb[];
  breadcrumbsClean: Breadcrumb[];


  ngOnInit() {

    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(event => {
      this.extractBreadcrumbs();
    });

    //Logger.log("Initialize breadcrumbs.");
  }

  extractBreadcrumbs() {//unwind currrent route's components

    this.breadcrumbsInternal = [];

    let currentRoute: ActivatedRoute = this.route;

    let pathSegments: UrlSegment[] = [];

    let i = -1;
    while (currentRoute != null) {

      //Logger.log(currentRoute);

      pathSegments = pathSegments.concat(currentRoute.snapshot.url);

      if (currentRoute.component != undefined) {
        let componentName = currentRoute.component['name'];

        this.breadcrumbsInternal[++i] = null;

        if (componentName == 'AppComponent') {
          this.breadcrumbsInternal[i] = new Breadcrumb('<i class="fas fa-home fa-sm"></i>', '');
          this.formCleanBreadcrumbs();
          //Logger.log(this.breadcrumbsInternal);
        }
        else {
          const localIndex = i;
          currentRoute.data.subscribe(dataset => {

            //Logger.log(dataset);

            if (dataset['breadcrumbIgnore'] != true) {

              let bcLabel = dataset['breadcrumbLabel'];

              if (bcLabel == undefined)
                bcLabel = componentName.replace('Component','').split(/(?=[A-Z])/).join(" ");

              this.breadcrumbsInternal[localIndex] = new Breadcrumb(bcLabel, pathSegments.join('/'));

              //Logger.log(this.breadcrumbsInternal);

              this.formCleanBreadcrumbs();
            }

          });
        }
      }

      currentRoute = currentRoute.children.length > 0 ? currentRoute.children[0] : null;
    };

  }

  formCleanBreadcrumbs() {
    let tempBreadcrumbs: Breadcrumb[] = [];

    for (let key in this.breadcrumbsInternal)
      if (this.breadcrumbsInternal[key] != null)
        tempBreadcrumbs.push(new Breadcrumb(this.breadcrumbsInternal[key].title, this.breadcrumbsInternal[key].url))

    if (tempBreadcrumbs.length > 1) {
      this.breadcrumbsClean = tempBreadcrumbs;
      this.breadcrumbsClean[this.breadcrumbsClean.length - 1].url = null;
    }
    else
      this.breadcrumbsClean = undefined;
  }


}
