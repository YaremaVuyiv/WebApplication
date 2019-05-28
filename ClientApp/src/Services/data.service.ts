﻿import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Topic } from '../Models/Topic';

import { Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
//import 'rxjs/add/operator/toPromise';

@Injectable()
export class DataService {

    private url = "http://localhost:62886/api/topics";
    headers: Headers;
    options: RequestOptions;

    constructor(private http: Http) {
        let authToken = localStorage.getItem('token');
        this.headers = new Headers({
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'application/json',
            'Accept': 'q=0.8;application/json;q=0.9',
            'Authorization': `Bearer ${authToken}`,
        });
        this.options = new RequestOptions({ headers: this.headers });
    }

    getTopics() {
        return this.http.get(this.url).toPromise().then(response => { return response.json(); });
    }

    getTopicById(id: number) {
        return this.http.get(this.url + '/' + id).toPromise().then(response => { return response.json(); });
    }

    createTopic(topic: Topic) {
        return this.http.post(this.url, topic, this.options).toPromise().then(response => { return response; });
    }

    deleteTopic(id: number) {
        return this.http.delete(this.url + '/' + id, this.options).toPromise().then(response => { return response; });
    }
}