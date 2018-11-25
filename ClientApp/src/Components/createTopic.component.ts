import { Component, OnInit } from '@angular/core';
import { DataService } from '../Services/data.service';
import { Topic } from '../Models/Topic';

import { HttpResponse } from '@angular/common/http'
import { ActivatedRoute, Router } from "@angular/router";
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
    templateUrl: '../Htmls/createTopic.component.html',
    providers: [DataService],
    styleUrls: ['../CSS/createTopic.component.css']
})
export class CreateTopicComponent implements OnInit {

    topic: Topic = new Topic();
    topics: Topic[];
    tableMode: boolean = true;
    name: string;
    body: string;

    constructor(private dataService: DataService, private route: ActivatedRoute, private router: Router, private spinner: NgxSpinnerService) { }

    ngOnInit() {

    }

    createTopic() {
        this.spinner.show();
        this.dataService.createTopic(this.topic).then(
            data => { this.router.navigate(['/']);
            this.spinner.hide();
        });
    }
}