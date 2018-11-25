import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { DataService } from '../Services/data.service';
import { Topic } from '../Models/Topic';

import { HttpResponse } from '@angular/common/http'
import { User } from "../Models/User";
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
    templateUrl: '../Htmls/startPage.component.html',
    providers: [DataService],
    styleUrls: ['../CSS/startPage.component.css'],
})
export class StartPageComponent implements OnInit {
    topic: Topic = new Topic();
    topics: Topic[];
    tableMode: boolean = true;
    isDataLoaded = false;

    constructor(private dataService: DataService, private spinner: NgxSpinnerService) {  }

    ngOnInit() {
        this.loadTopics();
        var user = new User();
    }

    loadTopics() {
        this.spinner.show();
        this.dataService.getTopics()
            .then((data: Topic[]) => {
                this.topics = data;
                this.isDataLoaded = true;
                this.spinner.hide();
            });
    }
}