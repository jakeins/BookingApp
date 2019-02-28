import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-error',
    templateUrl: './error.component.html'
})
export class ErrorComponent implements OnInit {

    message: string;

    constructor(private actRoute: ActivatedRoute) { }

    ngOnInit() {
        this.setMessage(+this.actRoute.snapshot.params['status-code']);
    }

    setMessage(statusCode: number) {
        switch (statusCode) {
          case 404:
            this.message = "404 Not Found";
            break;
          case 401:
            this.message = "401 Unauthorized";
            break;
          default:
            this.message = "404 Not Found";
            break;
        }
    }

}
