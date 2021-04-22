import { Component, OnInit } from '@angular/core';
import { SmzControlType, SmzForm, SmzFormsResponse, SmzPasswordControl, SmzTextControl } from 'ngx-smz-dialogs';
import { SmzAppLogo } from 'ngx-smz-ui';
import { AuthenticationActions } from 'ngx-rbk-utils';
import { SmzLayoutsConfig } from 'ngx-smz-ui';
import { Select, Store } from '@ngxs/store';
import { environment } from '@environments/environment';
import { Observable } from 'rxjs';
import { UiSelectors } from 'ngx-smz-ui';

interface LoginData {
  username: string;
  password: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  @Select(UiSelectors.appContentLogo) public appLogo$: Observable<SmzAppLogo>;
  public form: SmzForm<LoginData>;

  constructor(public readonly config: SmzLayoutsConfig, private store: Store) {
    this.createForm();
  }

  public ngOnInit(): void {


  }

  public createForm(): void {
    const username: SmzTextControl = {
      propertyName: 'username', type: SmzControlType.TEXT, name: 'Usuário',
      validatorsPreset: { isRequired: true },
      template: { extraSmall: { row: 'col-12' } }
    };

    const password: SmzPasswordControl = {
      propertyName: 'password', type: SmzControlType.PASSWORD, name: 'Senha',
      validatorsPreset: { isRequired: true },
      template: { extraSmall: { row: 'col-12' } }
    };

    this.form = {
      formId: 'form1',
      behaviors: { flattenResponse: false },
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
    this.store.dispatch(new AuthenticationActions.RemoteLogin(form.data.username, form.data.password, { applicationId: environment.serverUrl}));
  }

}