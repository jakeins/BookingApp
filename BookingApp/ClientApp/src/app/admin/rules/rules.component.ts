import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-rules',
  templateUrl: './rules.component.html',
  styleUrls: ['./rules.component.css']
})
export class RulesComponent implements OnInit {
  id: number;
  constructor(private activateRoute: ActivatedRoute) { }

  ngOnInit() {
    this.id = this.activateRoute.snapshot.params['id'];
  }
}
