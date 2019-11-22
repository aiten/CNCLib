import { Injectable, Inject, Pipe } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from "../models/user";


@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  constructor(private http: HttpClient,
    @Inject('BASE_URL') public baseUrl: string,
  ) {
  }
/*
    login(username: string, password: string) {
        return this.http.post<any>(`${this.baseUrl}/users/authenticate`, { username, password })
            .pipe(map(user => {
                // login successful if there's a user in the response
                if (user) {
                    // store user details and basic auth credentials in local storage 
                    // to keep user logged in between page refreshes
                    user.authdata = window.btoa(username + ':' + password);
                    localStorage.setItem('currentUser', JSON.stringify(user));
                }

                return user;
            }));
    }*/

  login(username: string, password: string): Promise<void> {

    console.log('Authentication:login');
    const params = new HttpParams()
      .set('userName', username)
      .set('password', password);

    const authentication$ = this.http
      .get<any>(`${this.baseUrl}api/user/isvaliduser`, { params })
      .toPromise()
      .then((response: Response) => {
        console.log('Authentication:login OK');
        var user = new User();
        user.id = 1;
        user.username = username;
        user.password = password;
        user.authData = window.btoa(username + ':' + password);
        localStorage.setItem('currentUser', JSON.stringify(user));
        console.log('Authentication:login OK');
      })
      .catch(this.handleErrorPromise);

/*
    // login successful if there's a user in the response
    if (userOk) {
      // store user details and basic auth credentials in local storage 
      // to keep user logged in between page refreshes
      //user.authdata = window.btoa(username + ':' + password);
      console.log('Authentication:login OK');
      localStorage.setItem('currentUser', JSON.stringify(username));

    }
*/
    return authentication$;
  }

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }

  logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('currentUser');
  }
}
