import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { SmzControlType, SmzForm, SmzFormsResponse, SmzPasswordControl, SmzTextControl } from 'ngx-smz-ui';
import { SmzAppLogo } from 'ngx-smz-ui';
import { ApplicationSelectors, AuthenticationActions } from 'ngx-smz-ui';
import { SmzLayoutsConfig } from 'ngx-smz-ui';
import { Actions, Select, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { UiSelectors } from 'ngx-smz-ui';
import { environment } from '@environments/environment';
import { tap } from 'rxjs/operators';

interface LoginData {
  username: string;
  password: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LoginComponent implements OnInit {
  @Select(UiSelectors.appContentLogo) public appLogo$: Observable<SmzAppLogo>;
  @Select(ApplicationSelectors.isWaitingRequest('login')) public isLoading$: Observable<boolean>;
  public form: SmzForm<LoginData>;
  public isProduction = environment.production;

  constructor(public readonly config: SmzLayoutsConfig, private store: Store, private actions$: Actions) {
    this.createForm();
  }

  public ngOnInit(): void {
  }

  public createForm(): void {
    const username: SmzTextControl = {
      propertyName: 'username', type: SmzControlType.TEXT, name: 'Username',
      validatorsPreset: { isRequired: true },
      template: { extraSmall: { row: 'col-12' } }
    };

    const password: SmzPasswordControl = {
      propertyName: 'password', type: SmzControlType.PASSWORD, name: 'Password',
      validatorsPreset: { isRequired: true },
      template: { extraSmall: { row: 'col-12' } }
    };

    this.form = {
      formId: 'form1',
      behaviors: { flattenResponse: false, submitOnEnter: true },
      groups: [
        {
          name: null,
          showName: true,
          children: [username, password],
          template: { extraSmall: { row: 'col-12' } }
        }
      ],
    };
  }

  public login(form: SmzFormsResponse<LoginData>): void {

    if (form.isValid) {
      // this.store.dispatch(new AuthenticationActions.RemoteLogin(form.data.username, form.data.password));
      setTimeout(() => {
        this.store.dispatch(new AuthenticationActions.RemoteLogin(form.data.username, form.data.password));
      }, 500);

    }

  }

  public forgotPassword(): void {
    throw new Error('Not Implemented');
  }
}