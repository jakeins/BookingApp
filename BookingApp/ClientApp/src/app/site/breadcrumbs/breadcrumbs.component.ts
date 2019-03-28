import { Component, OnInit } from '@angular/core';
import { Breadcrumb } from '../../models/breadcrumb';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
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

  breadcrumbs: Breadcrumb[];


  ngOnInit() {

    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(event => {
      this.resetBreadcrumbs();
    });

    Logger.log("Initialize breadcrumbs.");
  }

  resetBreadcrumbs() {

    let currentRoute: ActivatedRoute = this.route;

    Logger.log(this.route);

    while (currentRoute != null) {

      if (currentRoute.component != undefined) {
        let componentName = currentRoute.component['name'];

        if (componentName == "AppComponent") {
          Logger.log('home');
        }
        else {
          currentRoute.data.subscribe(dataset => {

            if (dataset["breadcrumbIgnore"] != 'true') {

              let x = dataset["breadcrumbLabel"];

              if (x == undefined)
                x = componentName;

              Logger.log(x);
            }

          });
        }



        

        //let breadcrumb = new Breadcrumb(currentRoute.data._value["breadcrumbLabel"], currentRoute.snapshot.url.toString());
        //breadcrumb
        //breadcrumbs

        //Logger.log(breadcrumb);


      }

      if (currentRoute.children.length > 0)
        currentRoute = currentRoute.children[0];
      else
        currentRoute = null;

    };


    

  }

}
